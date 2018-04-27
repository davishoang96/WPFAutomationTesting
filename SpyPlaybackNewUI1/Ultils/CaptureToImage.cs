using Gu.Wpf.UiAutomation;
using log4net;
using SpyandPlaybackTestTool.SpyPlaybackObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool.Ultils
{
    internal class CaptureToImage
    {
        //public static string ProcessName { get; set; }//nhớ sửa lại bỏ vào AUT path
        private static Gu.Wpf.UiAutomation.Application App;

        private static IReadOnlyList<UiElement> ElementList;
        private static Gu.Wpf.UiAutomation.Window MainWindow;

        private static SpyObject[] SpyObjectList;

        private static Users theUser = new Users();
        private static Process thisProc = Process.GetCurrentProcess();

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region UiElement Functions

        public IReadOnlyList<UiElement> ElementClass(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, type));
        }

        public IReadOnlyList<UiElement> SearchbyFramework(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, type));
        }

        #endregion UiElement Functions

        public static void DoCapture(string Proc)
        {
            var curtime = DateTime.Now;

            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var imagePath = path + @"\Botsina\SpyObjects\";

            System.IO.FileInfo file = new System.IO.FileInfo(imagePath);

            if (file.Directory.Exists)
            {
                DirectoryInfo dir = new DirectoryInfo(imagePath);

                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }
            }
            else
            {
                file.Directory.Create();
            }

            try
            {
                //this.Hide();

                log.Info("BEGIN CAPTURE TO FILE");

                file.Directory.Create();

                Process targetProcess = WindowInteraction.GetProcess(Proc);

                App = Gu.Wpf.UiAutomation.Application.Attach(targetProcess.Id);
                MainWindow = App.MainWindow;
                ElementList = MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, "WPF"));
                SpyObjectList = new SpyObject[ElementList.Count];
                int SpyObjectIndex = 0;

                SendKeys.Send("{PRTSC}");

                Image img = Clipboard.GetImage();

                for (int i = 0; i < ElementList.Count; i++)
                {
                    SpyObjectList[SpyObjectIndex] = new SpyObject();
                    SpyObjectList[SpyObjectIndex].index = SpyObjectIndex;
                    if (ElementList[i].AutomationId == "" && SpyObjectIndex - 1 > 0 && ElementList[i - 1].Name != "" && ElementList[i].Name == "")
                        SpyObjectList[SpyObjectIndex].automationId = (ElementList[i - 1].Name + "_" + ElementList[i].ClassName).Replace(" ", "_").Replace(":", "");
                    else
                        SpyObjectList[SpyObjectIndex].automationId = ElementList[i].AutomationId;
                    SpyObjectList[SpyObjectIndex].name = ElementList[i].Name;
                    SpyObjectList[SpyObjectIndex].type = ElementList[i].ClassName;

                    if ((SpyObjectList[SpyObjectIndex].type == "ComboBox" ||
                        SpyObjectList[SpyObjectIndex].type == "ComboBoxEdit" ||
                        SpyObjectList[SpyObjectIndex].type == "DataGrid" ||
                        SpyObjectList[SpyObjectIndex].type == "TextBox" ||
                        SpyObjectList[SpyObjectIndex].type == "Button" ||
                        SpyObjectList[SpyObjectIndex].type == "RadioButton" ||
                        SpyObjectList[SpyObjectIndex].type == "AutoCompleteCombobox")

                        && SpyObjectList[SpyObjectIndex].automationId.Contains("PART") != true)
                    {
                        if (ElementList[SpyObjectIndex].Bounds.IsEmpty == false && ElementList[SpyObjectIndex].IsOffscreen == false)
                        {
                            ElementList[SpyObjectIndex].CaptureToFile(imagePath + SpyObjectIndex + ".png");
                            log.Info(ElementList[SpyObjectIndex].ClassName);
                        }
                    }

                    SpyObjectIndex++;
                }
                img.Save(imagePath + "thisScreen" + ".png");
                log.Info("DONE CAPTURE TO FILE");

                //this.Show();
            }
            catch (Exception ex)
            {
                log.Error("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
                //this.Show();
            }
        }
    }
}