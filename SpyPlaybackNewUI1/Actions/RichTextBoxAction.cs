using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Threading;

namespace SpyandPlaybackTestTool.Actions
{
    internal class RichTextBoxAction : AbsAction
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public override void DoExecute()
        {
            switch (PlaybackObject.action)
            {
                case "SetText":
                    try
                    {
                        bool CapsLockOn = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
                        if (CapsLockOn == true)
                        {
                            //Console.WriteLine(CapsLock);
                            const int KEYEVENTF_EXTENDEDKEY = 0x1;
                            const int KEYEVENTF_KEYUP = 0x2;
                            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
                            keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                            (UIntPtr)0);
                        }
                        //(UiElement.AutomationElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern).SetValue(PlaybackObject.text);
                        UiElement.AsTextBox().Text = PlaybackObject.text;
                 
                        
                        Result = true;
                    }
                    //}
                    catch (Exception)
                    {
                        Result = false;
                    }
                    break;
            }
        }
    }
}
