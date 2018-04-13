using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpyandPlaybackTestTool.Ultils
{
    internal class WindowInteraction
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MAXIMIZE = 3;

        private const int SW_MINIMIZE = 6;

        public static void FocusWindow(Process targetWindow)
        {
            IntPtr hWnd = targetWindow.MainWindowHandle;
            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
                ShowWindow(hWnd, SW_MAXIMIZE);
            }
        }

        public static void FocusWindowNormal(Process targetWindow)
        {
            IntPtr hWnd = targetWindow.MainWindowHandle;
            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
                ShowWindow(hWnd, 9);
            }
        }

        public static Process GetProcess(string ProcessName)
        {
            try
            {
                Process[] process = Process.GetProcessesByName(ProcessName);
                return process[0];
            }
            catch
            {
                //MessageBox.Show(ex.Message);

                return null;
            }
        }
    }
}