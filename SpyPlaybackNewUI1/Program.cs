using System;
using System.Threading;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        private static string appGuid = "F92D8ED7-27E8-493C-9733-FDE096DA9984";

        [STAThread]
        private static void Main()
        {
            

            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Program already running" , "WARNING!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new Form1());
            }
        }
    }
}