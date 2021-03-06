﻿using MultiKeyboardHook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    internal partial class UnelevatedHookForm : Form
    {
        public EventWaitHandle WaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        public Queue<Message> MessageQueue = new Queue<Message>();

        public UnelevatedHookForm(Process parent)
        {
            Program.UnelevatedHookForm = this;

            InitializeComponent();

            Application.ApplicationExit += (sender, e) => { Program.Hook?.Dispose(); };

            if (!Program.Hook.Install(Handle, false))
            {
                MessageBox.Show("Hook installation failed.", "Multi Device Keybinds (Subprocess)", MessageBoxButtons.OK, MessageBoxIcon.Error);

                parent.Kill();
                
                Environment.Exit(0);

                return;
            }

            new Thread(() =>
            {
                parent.WaitForExit();
                
                Environment.Exit(0);
            })
            {
                IsBackground = true,
                Name = "Exit with parent thread"
            }.Start();

            NamedPipeClientStream client = new NamedPipeClientStream(".", $"MultiDeviceKeybinds_{parent.Id}", PipeDirection.InOut, PipeOptions.Asynchronous);

            client.Connect();

            Console.WriteLine($"[SUBPROCESS] Current TransmissionMode: {client.TransmissionMode}.");

            try
            {
                Program.PipeReader = new StreamReader(client);
                Program.PipeWriter = new StreamWriter(client);

                Console.Write("[SUBPROCESS] Wait for sync... ");

                while (Program.PipeReader.ReadLine() is string output && !output.Equals("PING", StringComparison.InvariantCultureIgnoreCase)) { }

                Program.PipeWriter.AutoFlush = true;

                Program.PipeWriter.WriteLine("PONG");

                client.WaitForPipeDrain();

                Console.WriteLine("Synced.");

                MainForm.PipeIO("SUBPROCESS", client);
            }
            catch (IOException e)
            {
                Console.WriteLine($"[SUBPROCESS] Error: {e}");
            }
        }

        private void UnelevatedHookForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == RawInputHook.WM_HOOK)
            {
                Program.PipeWriter?.WriteLine(JsonConvert.SerializeObject(m));

                //WaitHandle.WaitOne();
                if (!WaitHandle.WaitOne(RawInputHook.MaxWaitingTime))
                {
                    base.WndProc(ref m);

                    return;
                }

                lock (MessageQueue)
                {
                    for (int i = 0; i < MessageQueue.Count; i++)
                    {
                        Message msg = MessageQueue.ElementAt(i);

                        if (m.HWnd == msg.HWnd && m.Msg == msg.Msg && m.LParam == msg.LParam && m.WParam == msg.WParam)
                        {
                            while (i-- >= 0) MessageQueue.Dequeue();

                            m.Result = msg.Result;

                            break;
                        }
                    }
                }

                return;
            }

            base.WndProc(ref m);
        }
    }
}
