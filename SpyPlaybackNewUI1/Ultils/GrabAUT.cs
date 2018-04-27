using Gu.Wpf.UiAutomation;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Automation;

namespace SpyandPlaybackTestTool.Ultils
{
    internal class GrabAUT
    {
        public static Gu.Wpf.UiAutomation.Application App { get; set; }

        public static Gu.Wpf.UiAutomation.Window MainWindow { get; set; }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string theMessage { get; set; }

        #region UiElement Functions

        /// <summary>
        /// Search UI by class
        /// </summary>
        /// <param name="type">Button, MessageBox, DataGrid, ComboBox...etc</param>
        /// <returns></returns>
        public static IReadOnlyList<UiElement> ElementClass(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, type));
        }

        /// <summary>
        /// Search all UI by framework
        /// </summary>
        /// <param name="type">WPF or Win32</param>
        /// <returns></returns>
        public static IReadOnlyList<UiElement> SearchbyFramework(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, type));
        }

        public static void GetMainWindow()
        {
            try
            {
                Process AttachProcess = WindowInteraction.GetProcess(ProcessForm.targetproc);
                
                App = Application.Attach(AttachProcess.Id);

                App.GetMainWindow(TimeSpan.FromSeconds(5));

                MainWindow = App.MainWindow;

            }
            catch
            {
                //T1.Join();
                throw new Exception("Cannot get MainWindow");
            }
           
        }

        

        #endregion UiElement Functions
    }
}