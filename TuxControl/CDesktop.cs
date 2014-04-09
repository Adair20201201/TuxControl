using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TuxControl
{
    class CDesktop
    {
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetThreadDesktop(uint dwThreadId);
    
    
    }
}
