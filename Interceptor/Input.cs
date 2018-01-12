﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Interceptor
{
    public class Input
    {
        public IntPtr Context { get; private set; }

        private Thread callbackThread;

        /// <summary>
        /// Determines whether the driver traps no keyboard events, all events, or a range of events in-between (down only, up only...etc). Set this before loading otherwise the driver will not filter any events and no keypresses can be sent.
        /// </summary>
        public KeyboardFilterMode KeyboardFilterMode { get; set; }

        /// <summary>
        /// Determines whether the driver traps no events, all events, or a range of events in-between. Set this before loading otherwise the driver will not filter any events and no mouse clicks can be sent.
        /// </summary>
        public MouseFilterMode MouseFilterMode { get; set; }

        public bool IsLoaded { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds after each key stroke down and up. Pressing a key requires both a key stroke down and up. A delay of 0 (inadvisable) may result in no keys being apparently pressed. A delay of 20 - 40 milliseconds makes the key presses visible.
        /// </summary>
        public int KeyPressDelay { get; set; }

        /// <summary>
        /// Gets or sets the delay in milliseconds after each mouse event down and up. 'Clicking' the cursor (whether left or right) requires both a mouse event down and up. A delay of 0 (inadvisable) may result in no apparent click. A delay of 20 - 40 milliseconds makes the clicks apparent.
        /// </summary>
        public int ClickDelay { get; set; }

        public int ScrollDelay { get; set; }

        public event EventHandler<KeyPressedEventArgs> OnKeyPressed;
        public event EventHandler<MousePressedEventArgs> OnMousePressed;

        public int DeviceID { get; private set; } /* Very important; which device the driver sends events to */

        public Input()
        {
            Context = IntPtr.Zero;

            KeyboardFilterMode = KeyboardFilterMode.None;
            MouseFilterMode = MouseFilterMode.None;

            KeyPressDelay = 1;
            ClickDelay = 1;
            ScrollDelay = 15;
        }

        ~Input()
        {
            Unload();
        }

        /// <summary>
        /// Attempts to load the driver. You may get an error if the C++ library 'interception.dll' is not in the same folder as the executable and other DLLs. MouseFilterMode and KeyboardFilterMode must be set before Load() is called. Calling Load() twice has no effect if already loaded.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool Load()
        {
            if (IsLoaded) return false;

            Context = InterceptionDriver.CreateContext();

            if (Context != IntPtr.Zero)
            {
                callbackThread = new Thread(new ThreadStart(DriverCallback))
                {
                    Priority = ThreadPriority.Highest,
                    IsBackground = true
                };
                callbackThread.Start();

                IsLoaded = true;

                return true;
            }
            else
            {
                IsLoaded = false;

                return false;
            }
        }

        /// <summary>
        /// Safely unloads the driver. Calling Unload() twice has no effect.
        /// </summary>
        public void Unload()
        {
            if (!IsLoaded) return;

            if (Context != IntPtr.Zero)
            {
                callbackThread.Abort();
                InterceptionDriver.DestroyContext(Context);

                IsLoaded = false;
            }
        }

        private void DriverCallback()
        {
            InterceptionDriver.SetFilter(Context, InterceptionDriver.IsKeyboard, (int)KeyboardFilterMode);
            InterceptionDriver.SetFilter(Context, InterceptionDriver.IsMouse, (int)MouseFilterMode);

            Stroke stroke = new Stroke();

            while (InterceptionDriver.Receive(Context, DeviceID = InterceptionDriver.Wait(Context), ref stroke, 1) > 0)
            {
                if (InterceptionDriver.IsMouse(DeviceID) > 0 && OnMousePressed != null)
                {
                    MousePressedEventArgs args = new MousePressedEventArgs() { X = stroke.Mouse.X, Y = stroke.Mouse.Y, State = stroke.Mouse.State, Rolling = stroke.Mouse.Rolling };
                    OnMousePressed(this, args);

                    if (args.Handled) continue;

                    stroke.Mouse.X = args.X;
                    stroke.Mouse.Y = args.Y;
                    stroke.Mouse.State = args.State;
                    stroke.Mouse.Rolling = args.Rolling;
                }
                else if (InterceptionDriver.IsKeyboard(DeviceID) > 0 && OnKeyPressed != null)
                {
                    KeyPressedEventArgs args = new KeyPressedEventArgs()
                    {
                        DeviceID = DeviceID,
                        Key = stroke.Key.Code,
                        State = stroke.Key.State
                    };
                    OnKeyPressed(this, args);

                    if (args.Handled) continue;

                    stroke.Key.Code = args.Key;
                    stroke.Key.State = args.State;

                    InterceptionDriver.Send(Context, args.DeviceID, ref stroke, 1);
                }
                else
                {
                    InterceptionDriver.Send(Context, DeviceID, ref stroke, 1);
                }
            }

            Unload();
            throw new Exception("Interception.Receive() failed for an unknown reason. The driver has been unloaded.");
        }

        public void SendKey(InterceptionKeys key, KeyState state)
        {
            SendKey(key, state, DeviceID);
        }

        public void SendKey(InterceptionKeys key, KeyState state, int device)
        {
            Stroke stroke = new Stroke();
            KeyStroke keyStroke = new KeyStroke
            {
                Code = key,
                State = state
            };

            stroke.Key = keyStroke;

            InterceptionDriver.Send(Context, device, ref stroke, 1);

            if (KeyPressDelay > 0) Thread.Sleep(KeyPressDelay);
        }

        /// <summary>
        /// Warning: Do not use this overload of SendKey() for non-letter, non-number, or non-ENTER keys. It may require a special KeyState of not KeyState.Down or KeyState.Up, but instead KeyState.E0 and KeyState.E1.
        /// </summary>
        public void SendKey(InterceptionKeys key)
        {
            SendKey(key, KeyState.Down);

            if (KeyPressDelay > 0) Thread.Sleep(KeyPressDelay);

            SendKey(key, KeyState.Up);
        }

        public void SendKeys(params InterceptionKeys[] keys)
        {
            foreach (InterceptionKeys key in keys) SendKey(key);
        }

        /// <summary>
        /// Warning: Only use this overload for sending letters, numbers, and symbols (those to the right of the letters on a U.S. keyboard and those obtained by pressing shift-#). Do not send special keys like Tab or Control or Enter.
        /// </summary>
        /// <param name="text"></param>
        public void SendText(string text)
        {
            foreach (char letter in text)
            {
                var tuple = CharacterToKeysEnum(letter);

                if (tuple.Item2) SendKey(InterceptionKeys.LeftShift, KeyState.Down); // We need to press shift to get the next character

                SendKey(tuple.Item1);

                if (tuple.Item2) SendKey(InterceptionKeys.LeftShift, KeyState.Up);
            }
        }

        /// <summary>
        /// Converts a character to a Keys enum and a 'do we need to press shift'.
        /// </summary>
        private Tuple<InterceptionKeys, bool> CharacterToKeysEnum(char c)
        {
            switch (Char.ToLower(c))
            {
                case 'a':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.A, false);
                case 'b':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.B, false);
                case 'c':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.C, false);
                case 'd':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.D, false);
                case 'e':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.E, false);
                case 'f':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.F, false);
                case 'g':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.G, false);
                case 'h':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.H, false);
                case 'i':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.I, false);
                case 'j':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.J, false);
                case 'k':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.K, false);
                case 'l':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.L, false);
                case 'm':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.M, false);
                case 'n':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.N, false);
                case 'o':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.O, false);
                case 'p':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.P, false);
                case 'q':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Q, false);
                case 'r':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.R, false);
                case 's':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.S, false);
                case 't':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.T, false);
                case 'u':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.U, false);
                case 'v':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.V, false);
                case 'w':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.W, false);
                case 'x':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.X, false);
                case 'y':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Y, false);
                case 'z':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Z, false);
                case '1':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.One, false);
                case '2':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Two, false);
                case '3':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Three, false);
                case '4':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Four, false);
                case '5':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Five, false);
                case '6':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Six, false);
                case '7':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Seven, false);
                case '8':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Eight, false);
                case '9':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Nine, false);
                case '0':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Zero, false);
                case '-':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.DashUnderscore, false);
                case '+':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.PlusEquals, false);
                case '[':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.OpenBracketBrace, false);
                case ']':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CloseBracketBrace, false);
                case ';':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SemicolonColon, false);
                case '\'':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SingleDoubleQuote, false);
                case ',':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CommaLeftArrow, false);
                case '.':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.PeriodRightArrow, false);
                case '/':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.ForwardSlashQuestionMark, false);
                case '{':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.OpenBracketBrace, true);
                case '}':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CloseBracketBrace, true);
                case ':':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SemicolonColon, true);
                case '\"':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.SingleDoubleQuote, true);
                case '<':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.CommaLeftArrow, true);
                case '>':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.PeriodRightArrow, true);
                case '?':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.ForwardSlashQuestionMark, true);
                case '\\':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.BackslashPipe, false);
                case '|':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.BackslashPipe, true);
                case '`':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Tilde, false);
                case '~':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Tilde, true);
                case '!':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.One, true);
                case '@':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Two, true);
                case '#':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Three, true);
                case '$':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Four, true);
                case '%':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Five, true);
                case '^':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Six, true);
                case '&':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Seven, true);
                case '*':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Eight, true);
                case '(':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Nine, true);
                case ')':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Zero, true);
                case ' ':
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.Space, true);
                default:
                    return new Tuple<InterceptionKeys, bool>(InterceptionKeys.ForwardSlashQuestionMark, true);
            }
        }

        public void SendMouseEvent(MouseState state)
        {
            Stroke stroke = new Stroke();
            MouseStroke mouseStroke = new MouseStroke
            {
                State = state
            };

            /*if (state == MouseState.ScrollUp)
            {
                mouseStroke.Rolling = 120;
            }
            else if (state == MouseState.ScrollDown)
            {
                mouseStroke.Rolling = -120;
            }*/

            mouseStroke.Rolling = (short)(state == MouseState.ScrollUp ? 120 : state == MouseState.ScrollDown ? -120 : mouseStroke.Rolling);

            stroke.Mouse = mouseStroke;

            InterceptionDriver.Send(Context, 12, ref stroke, 1);
        }

        public void SendLeftClick()
        {
            SendMouseEvent(MouseState.LeftDown);

            Thread.Sleep(ClickDelay);

            SendMouseEvent(MouseState.LeftUp);
        }

        public void SendRightClick()
        {
            SendMouseEvent(MouseState.RightDown);

            Thread.Sleep(ClickDelay);

            SendMouseEvent(MouseState.RightUp);
        }

        public void ScrollMouse(ScrollDirection direction)
        {
            switch (direction)
            {
                case ScrollDirection.Down:
                    SendMouseEvent(MouseState.ScrollDown);

                    break;
                case ScrollDirection.Up:
                    SendMouseEvent(MouseState.ScrollUp);

                    break;
            }
        }

        /// <summary>
        /// Warning: This function, if using the driver, does not function reliably and often moves the mouse in unpredictable vectors. An alternate version uses the standard Win32 API to get the current cursor's position, calculates the desired destination's offset, and uses the Win32 API to set the cursor to the new position.
        /// </summary>
        public void MoveMouseBy(int deltaX, int deltaY, bool useDriver = false)
        {
            if (useDriver)
            {
                Stroke stroke = new Stroke();
                MouseStroke mouseStroke = new MouseStroke();

                mouseStroke.X = deltaX;
                mouseStroke.Y = deltaY;

                stroke.Mouse = mouseStroke;
                stroke.Mouse.Flags = MouseFlags.MoveRelative;

                InterceptionDriver.Send(Context, 12, ref stroke, 1);
            }
            else
            {
                var currentPos = Cursor.Position;
                Cursor.Position = new Point(currentPos.X + deltaX, currentPos.Y - deltaY); // Coordinate system for y: 0 begins at top, and bottom of screen has the largest number
            }
        }

        /// <summary>
        /// Warning: This function, if using the driver, does not function reliably and often moves the mouse in unpredictable vectors. An alternate version uses the standard Win32 API to set the cursor's position and does not use the driver.
        /// </summary>
        public void MoveMouseTo(int x, int y, bool useDriver = false)
        {
            if (useDriver)
            {
                Stroke stroke = new Stroke();
                MouseStroke mouseStroke = new MouseStroke();

                mouseStroke.X = x;
                mouseStroke.Y = y;

                stroke.Mouse = mouseStroke;
                stroke.Mouse.Flags = MouseFlags.MoveAbsolute;

                InterceptionDriver.Send(Context, 12, ref stroke, 1);
            }
            else
            {
                Cursor.Position = new Point(x, y);
            }
        }
    }
}