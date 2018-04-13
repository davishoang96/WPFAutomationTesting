using Gu.Wpf.UiAutomation;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Automation;

namespace SpyandPlaybackTestTool.Ultils
{
    internal class DoSpy
    {
        /// <summary>
        /// Wrapper for WPF Application
        /// </summary>
        public static Gu.Wpf.UiAutomation.Application App { get; set; }

        /// <summary>
        /// Get MainWindow of the AUT
        /// </summary>
        public static Gu.Wpf.UiAutomation.Window MainWindow { get; set; }

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string theMessage { get; set; }

        #region UiElement Functions

        public static IReadOnlyList<UiElement> ElementClass(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, type));
        }

        public static void GetMainWindow()
        {
            try
            {
                Process AttachProcess = WindowInteraction.GetProcess(ProcessForm.targetproc);
                App = Application.Attach(AttachProcess.Id);
                MainWindow = App.MainWindow;

            }
            catch
            {
                throw new Exception("Cannot found MainWindow");
            }
           
        }

        public static IReadOnlyList<UiElement> SearchbyFramework(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, type));
        }

        #endregion UiElement Functions
    }
}