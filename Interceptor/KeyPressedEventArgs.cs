﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Interceptor
{
    public class KeyPressedEventArgs : EventArgs
    {
        public int DeviceID { get; set; }

        public InterceptionKeys Key { get; set; }
        public KeyState State { get; set; }

        public bool Handled { get; set; }
    }
}
