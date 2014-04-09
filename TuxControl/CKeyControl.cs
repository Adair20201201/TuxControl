using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TuxControl
{
    class CKeyControl
    {

        #region WinAPI
        /// <summary>
        /// The event type contained in the union field
        /// </summary>
        enum SendInputEventType : int
        {
            /// <summary>
            /// Contains Mouse event data
            /// </summary>
            InputMouse,
            /// <summary>
            /// Contains Keyboard event data
            /// </summary>
            InputKeyboard,
            /// <summary>
            /// Contains Hardware event data
            /// </summary>
            InputHardware
        }


        /// <summary>
        /// The mouse data structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct MouseInputData
        {
            /// <summary>
            /// The x value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
            /// otherwise it is a delta from the last position
            /// </summary>
            public int dx;
            /// <summary>
            /// The y value, if ABSOLUTE is passed in the flag then this is an actual X and Y value
            /// otherwise it is a delta from the last position
            /// </summary>
            public int dy;
            /// <summary>
            /// Wheel event data, X buttons
            /// </summary>
            public uint mouseData;
            /// <summary>
            /// ORable field with the various flags about buttons and nature of event
            /// </summary>
            public MouseEventFlags dwFlags;
            /// <summary>
            /// The timestamp for the event, if zero then the system will provide
            /// </summary>
            public uint time;
            /// <summary>
            /// Additional data obtained by calling app via GetMessageExtraInfo
            /// </summary>
            public IntPtr dwExtraInfo;
        }

        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;

        /// <summary>
        /// Used in mouseData if XDOWN or XUP specified
        /// </summary>
        [Flags]
        enum MouseDataFlags : uint
        {
            /// <summary>
            /// First button was pressed or released
            /// </summary>
            XBUTTON1 = 0x0001,
            /// <summary>
            /// Second button was pressed or released
            /// </summary>
            XBUTTON2 = 0x0002
        }

        /// <summary>
        /// The flags that a MouseInput.dwFlags can contain
        /// </summary>
        [Flags]
        enum MouseEventFlags : uint
        {
            /// <summary>
            /// Movement occured
            /// </summary>
            MOUSEEVENTF_MOVE = 0x0001,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_LEFTUP = 0x0004,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_RIGHTUP = 0x0010,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            /// <summary>
            /// button down (pair with an up to create a full click)
            /// </summary>
            MOUSEEVENTF_XDOWN = 0x0080,
            /// <summary>
            /// button up (pair with a down to create a full click)
            /// </summary>
            MOUSEEVENTF_XUP = 0x0100,
            /// <summary>
            /// Wheel was moved, the value of mouseData is the number of movement values
            /// </summary>
            MOUSEEVENTF_WHEEL = 0x0800,
            /// <summary>
            /// Map X,Y to entire desktop, must be used with MOUSEEVENT_ABSOLUTE
            /// </summary>
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            /// <summary>
            /// The X and Y members contain normalised Absolute Co-Ords. If not set X and Y are relative
            /// data to the last position (i.e. change in position from last event)
            /// </summary>
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        /// <summary>
        /// Captures the union of the three three structures.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdhardwareInputUnion
        {
            /// <summary>
            /// The Mouse Input Data
            /// </summary>
            [FieldOffset(0)]
            public MouseInputData mi;

            /// <summary>
            /// The Keyboard input data
            /// </summary>
            [FieldOffset(0)]
            public KEYBDINPUT ki;

            /// <summary>
            /// The hardware input data
            /// </summary>
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        /// <summary>
        /// The Data passed to SendInput in an array.
        /// </summary>
        /// <remarks>Contains a union field type specifies what it contains </remarks>
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            /// <summary>
            /// The actual data type contained in the union Field
            /// </summary>
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        /// <summary>
        /// SendsInput using Win32 API Function
        /// </summary>
        /// <param name="nInputs">The number input structures in the array.</param>
        /// <param name="pInputs">The pointer to array of input structures.</param>
        /// <param name="cbSize">Size of the structure in the array.</param>
        /// <returns>The function returns the number of events that it successfully 
        /// inserted into the keyboard or mouse input stream. If the function returns zero, 
        /// the input was already blocked by another thread. To get extended error information, 
        /// call GetLastError.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        #endregion


        public void pressKey(ushort key)
        {
            INPUT[] inp = new INPUT[1];
            inp[0].type = SendInputEventType.InputKeyboard;
            KEYBDINPUT kin = new KEYBDINPUT();
            kin.wVk = key;
            inp[0].mkhi.ki = kin;

            
            unsafe
            {
                inp[0].mkhi.ki.dwFlags = 0;
                SendInput((uint)1, ref inp[0], sizeof(INPUT));
 
            }
            
        }
        public void releaseKey(ushort key)
        {
            INPUT[] inp = new INPUT[1];
            inp[0].type = SendInputEventType.InputKeyboard;
            KEYBDINPUT kin = new KEYBDINPUT();
            kin.wVk = key;
            inp[0].mkhi.ki = kin;

            unsafe
            {
                inp[0].mkhi.ki.dwFlags = 2;
                SendInput((uint)1, ref inp[0], sizeof(INPUT));
            }
            
        }
    
    }
}
