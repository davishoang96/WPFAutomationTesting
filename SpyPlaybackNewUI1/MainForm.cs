using Gu.Wpf.UiAutomation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BotsinaWPF.SpyPlaybackObjects;
using BotsinaWPF.Ultils;
using SimplePlayBack.Actions;
using System.Windows.Automation;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using BotsinaWPF.Actions;
using log4net;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using static BotsinaWPF.Ultils.Message;

namespace BotsinaWPF
{
    public partial class MainForm : Form
    {
        
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        ClearTextBox ctbox = new ClearTextBox();
        string ProcessName = "";//nhớ sửa lại bỏ vào AUT path
        Gu.Wpf.UiAutomation.Application App;
        IReadOnlyList<UiElement> ElementList;
        Gu.Wpf.UiAutomation.Window MainWindow;
        SpyObject[] SpyObjectList;
        PlaybackObject[] PlaybackObjectList;
        static ScriptFile[] scriptFiles;
        Users theUser = new Users();
        private ProcessForm processForm = null;
        Process thisProc = Process.GetCurrentProcess();
        Process targetProc = null;

        public MainForm()
        {

            InitializeComponent();
            log.Info("PROGRAM STARTED");
            string JsonPath = theUser.CreateScriptFolder() + @"TestScript.json";
            string result;
            ResultPanelPush.ReadOnly = true;
            ConsolePanelPush.ReadOnly = true;



            try
            {
                if (!File.Exists(JsonPath))
                {
                    log.Info("CREATE NEW TEST SCRIPT");
                    using (StreamWriter writer = new StreamWriter(theUser.CreateScriptFolder() + @"TestScript.json"))
                    {
                        rtxtScript.Text = "";
                    }
                }
                else
                {
                    using (StreamReader streamReader = new StreamReader(theUser.CreateScriptFolder() + @"TestScript.json"))
                    {
                        result = streamReader.ReadToEnd();
                        rtxtScript.Text = result;
                    }

                    comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView2.AllowUserToAddRows = false;
                    dataGridView2.AllowUserToResizeRows = false;
                    foreach (DataGridViewColumn col in dataGridView2.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;


                    dataGridView1.RowHeadersVisible = false;
                    dataGridView2.RowHeadersVisible = false;
                    dataGridView1.AllowUserToResizeRows = false;
                    textBox2.ReadOnly = true;
                    textBox2.Enabled = false;
                    (dataGridView1.Columns[0] as DataGridViewCheckBoxColumn).TrueValue = true;
                    (dataGridView1.Columns[0] as DataGridViewCheckBoxColumn).FalseValue = false;
                    log.Info("TEST SCRIPT LOADED");
                    textBox1.Enabled = false;
                    comboBox1.Enabled = false;

                }

            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
            }
        }

        #region UiElement Functions
        public IReadOnlyList<UiElement> ElementClass(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, type));
        }
        public IReadOnlyList<UiElement> SearchbyFramework(string type)
        {
            return MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, type));
        }
        #endregion

        // Spy Button
        private void button1_Click(object sender, EventArgs e)
        {
            var curtime = DateTime.Now;
            
            try
            {
                log.Info("BEGIN SPY");
                ConsolePanelPush.AppendText(curtime + " - " + "BEGIN SPY");
                ConsolePanelPush.AppendText(Environment.NewLine);
                Process targetProcess = WindowInteraction.GetProcess(ProcessName);
                targetProc = targetProcess;
                WindowInteraction.FocusWindow(targetProcess);
                App = Gu.Wpf.UiAutomation.Application.Attach(targetProcess.Id);
                MainWindow = App.MainWindow;
                ElementList = MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, "WPF"));
                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = true;

                SpyObjectList = new SpyObject[ElementList.Count];
                int SpyObjectIndex = 0;
                textBox1.Enabled = false;
                comboBox1.Enabled = false;
                dataGridView1.Enabled = true;
                dataGridView2.Enabled = true;
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
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[i].Clone();
                    row.Cells[1].Value = SpyObjectList[SpyObjectIndex].index;
                    row.Cells[2].Value = SpyObjectList[SpyObjectIndex].automationId;
                    row.Cells[3].Value = SpyObjectList[SpyObjectIndex].name;
                    row.Cells[4].Value = SpyObjectList[SpyObjectIndex].type;
                    dataGridView1.Rows.Add(row);
                    SpyObjectIndex++;
                }
                log.Info("SPY DONE");
                ConsolePanelPush.AppendText(curtime + " - " + "SPY DONE");
                ConsolePanelPush.AppendText(Environment.NewLine);
                textBox1.Enabled = true;
                comboBox1.Enabled = true;
                WindowInteraction.FocusWindowNormal(thisProc);
                dataGridView1.AllowUserToAddRows = false;
                textBox1.Text = "";
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                log.Error("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
                ConsolePanelPush.AppendText("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
                ConsolePanelPush.AppendText(Environment.NewLine);
                WindowInteraction.FocusWindow(thisProc);
            }

        }



        public void savescript()
        {
            
            string JsonPath = theUser.CreateScriptFolder() + @"TestScript.json";
            string JsonContent = rtxtScript.Text;
            File.WriteAllText(JsonPath, JsonContent);
        }

        // Button Save jsonscript
        private void btnQuickSave2_Click(object sender, EventArgs e)
        {
            if (clbTestScriptList.Items.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("There is no test script file!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int i = 0;
            foreach (var scriptFile in scriptFiles)
            {
                if (scriptFile.Name == clbTestScriptList.SelectedItem.ToString())
                {
                    File.WriteAllText(scriptFiles[i].Path, rtxtScript.Text);
                    break;
                }
                i++;
            }
            System.Windows.Forms.MessageBox.Show("Save succeed!");
        }

        public void playback()
        {
            var curtime = DateTime.Now;
            ExceptionCode excode = new ExceptionCode();
            ResultPanelPush.Clear();
            //ResultPanel.Items.Clear();
            try
            {
                log.Info("BEGIN PLAYBACK");
                Process targetProcess = WindowInteraction.GetProcess(ProcessName);
                WindowInteraction.FocusWindow(targetProcess);
                App = Gu.Wpf.UiAutomation.Application.Attach(targetProcess.Id);
                MainWindow = App.MainWindow;
                ElementList = SearchbyFramework("WPF");

                //savescript();
                readJson();

                for (int i = 0; i < PlaybackObjectList.Count(); i++)
                {
                    curtime = DateTime.Now;
                    WindowInteraction.FocusWindow(targetProcess);
                    if (MainWindow.ModalWindows.Count > 0)
                        ElementList = SearchbyFramework("WPF");

                    // Check if Json type is valid
                    if (PlaybackObjectList[i].type == "Sendkey" || PlaybackObjectList[i].type == "Button" ||
                       PlaybackObjectList[i].type == "TextBox" || PlaybackObjectList[i].type == "WaitEnable" ||
                       PlaybackObjectList[i].type == "ComboBox" || PlaybackObjectList[i].type == "ComboBoxEdit" ||
                       PlaybackObjectList[i].type == "DataGrid" || PlaybackObjectList[i].type == "RadioButton")
                    {

                        if (PlaybackObjectList[i].action == "Click" || PlaybackObjectList[i].action == "DoubleClick" ||
                           PlaybackObjectList[i].action == "Select" || PlaybackObjectList[i].action == "SetText" ||
                           PlaybackObjectList[i].action == "Wait" || PlaybackObjectList[i].action == "UnSelect")
                        {

                            switch (PlaybackObjectList[i].type)
                            {
                                case "Sendkey":
                                    AbsAction pressEnter = new PressEnter();
                                    pressEnter.PlaybackObject = PlaybackObjectList[i];
                                    pressEnter.DoExecute();
                                    break;

                                case "Button":
                                    AbsAction buttonaction = new ButtonAction();
                                    buttonaction.PlaybackObject = PlaybackObjectList[i];
                                    buttonaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    buttonaction.DoExecute();
                                    if (buttonaction.Result == true)
                                    {
                                        log.Info("PASSED at item id: " + buttonaction.PlaybackObject.index + " --- " + "ClassType: " + buttonaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + buttonaction.PlaybackObject.index + " --- " + "ClassType: " + buttonaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);

                                    }
                                    else
                                    {
                                        log.Fatal("FAILED at item id: " + buttonaction.PlaybackObject.index + " --- " + "ClassType: " + buttonaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "FAILED  at item id: " + buttonaction.PlaybackObject.index + " --- " + "ClassType: " + buttonaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }

                                    break;
                                case "TextBox":
                                    AbsAction textboxaction = new TextBoxAction();
                                    textboxaction.PlaybackObject = PlaybackObjectList[i];
                                    textboxaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    textboxaction.DoExecute();
                                    if (textboxaction.Result == true)
                                    {
                                        log.Info("PASSED at item id: " + textboxaction.PlaybackObject.index + " --- " + "ClassType: " + textboxaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + textboxaction.PlaybackObject.index + " --- " + "ClassType: " + textboxaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    else
                                    {
                                        log.Fatal("FAILED at item id: " + textboxaction.PlaybackObject.index + " --- " + "ClassType: " + textboxaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + textboxaction.PlaybackObject.index + " --- " + "ClassType: " + textboxaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    break;

                                case "WaitEnable":
                                    AbsAction WaitEnable = new WaitEnable();
                                    WaitEnable.PlaybackObject = PlaybackObjectList[i];
                                    WaitEnable.DoExecute();

                                    break;
                                case "ComboBox":
                                    AbsAction comboboxaction = new ComboBoxAction();
                                    comboboxaction.PlaybackObject = PlaybackObjectList[i];
                                    comboboxaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    comboboxaction.DoExecute();

                                    break;
                                case "ComboBoxEdit":
                                    AbsAction comboboxeditaction = new ComboBoxEditAction();
                                    comboboxeditaction.PlaybackObject = PlaybackObjectList[i];
                                    comboboxeditaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    comboboxeditaction.DoExecute();

                                    if (comboboxeditaction.Result == false)
                                    {
                                        log.Fatal("FAILED at item id: " + comboboxeditaction.PlaybackObject.index + " --- " + "ClassType: " + comboboxeditaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + comboboxeditaction.PlaybackObject.index + " --- " + "ClassType: " + comboboxeditaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    else
                                    {
                                        log.Info("PASSED at item id: " + comboboxeditaction.PlaybackObject.index + " --- " + "ClassType: " + comboboxeditaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + comboboxeditaction.PlaybackObject.index + " --- " + "ClassType: " + comboboxeditaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }

                                    break;

                                case "DataGrid":
                                    AbsAction datagridaction = new DataGridAction();
                                    datagridaction.PlaybackObject = PlaybackObjectList[i];
                                    datagridaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    datagridaction.DoExecute();
                                    if (datagridaction.Result == false)
                                    {
                                        log.Fatal("FAILED at item id: " + datagridaction.PlaybackObject.index + " --- " + "ClassType: " + datagridaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + datagridaction.PlaybackObject.index + " --- " + "ClassType: " + datagridaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    else
                                    {
                                        log.Info("PASSED at item id: " + datagridaction.PlaybackObject.index + " --- " + "ClassType: " + datagridaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + datagridaction.PlaybackObject.index + " --- " + "ClassType: " + datagridaction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    break;

                                case "RadioButton":
                                    AbsAction RadioButtonAction = new RadioButtonAction();
                                    RadioButtonAction.PlaybackObject = PlaybackObjectList[i];
                                    RadioButtonAction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    RadioButtonAction.DoExecute();
                                    if (RadioButtonAction.Result == false)
                                    {
                                        ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + RadioButtonAction.PlaybackObject.index + " --- " + "ClassType: " + RadioButtonAction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    else
                                    {
                                        ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + RadioButtonAction.PlaybackObject.index + " --- " + "ClassType: " + RadioButtonAction.UiElement.ClassName);
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            log.Fatal("FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID TYPE: " + PlaybackObjectList[i].action);
                            ResultPanelPush.AppendText(curtime + " - " + "FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID TYPE: " + PlaybackObjectList[i].action);
                            ResultPanelPush.AppendText(Environment.NewLine);
                        }

                    }
                    else
                    {
                        log.Fatal("FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID TYPE: " + PlaybackObjectList[i].type);
                        ResultPanelPush.AppendText(curtime + " - " + "FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID TYPE: " + PlaybackObjectList[i].type);
                        ResultPanelPush.AppendText(Environment.NewLine);
                    }
                    if (MainWindow.ModalWindows.Count > 0)
                        ElementList = SearchbyFramework("WPF");
                }

                log.Info("PLAYBACK DONE");
                
                WindowInteraction.FocusWindowNormal(thisProc);
            }
            catch (Exception ex)
            {
                if (ex.HResult == excode.AUT_NOT_FOUND)
                {
                    curtime = DateTime.Now;
                    log.Error("AUT NOT FOUND");
                    ConsolePanelPush.AppendText(curtime + " - " + "AUT NOT FOUND");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.CANNOT_FOCUS_ON_AUT)
                {
                    curtime = DateTime.Now;
                    log.Error("CANNOT FOCUS ON AUT OR INPUT WAS NOT ENABLE");
                    ConsolePanelPush.AppendText(curtime + " - " + "CANNOT FOCUS ON AUT OR INPUT WAS NOT ENABLE");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.INVALID_SCRIPT)
                {
                    curtime = DateTime.Now;
                    log.Error("CANNOT USE THE CURRENT SCRIPT ON THIS SCREEN");
                    ConsolePanelPush.AppendText(curtime + " - " + "CANNOT USE THE CURRENT SCRIPT ON THIS SCREEN");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.SCRIPT_ERROR)
                {
                    curtime = DateTime.Now;
                    log.Error("SCRIPT FORMAT IS INVALID");
                    ConsolePanelPush.AppendText(curtime + " - " + "SCRIPT FORMAT IS INVALID");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.AUT_QUIT_DURING_OP)
                {
                    curtime = DateTime.Now;
                    log.Error("AUT QUIT DURING PLAYBACK OPERATION");
                    ConsolePanelPush.AppendText(curtime + " - " + "AUT QUIT DURING PLAYBACK OPERATION");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.INVALID_INDEX)
                {
                    curtime = DateTime.Now;
                    log.Error("INPUT VALUE WAS OUT OF RANGE");
                    ConsolePanelPush.AppendText(curtime + " - " + "INPUT VALUE WAS OUT OF RANGE");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.OBJECT_NULL)
                {
                    curtime = DateTime.Now;
                    log.Error("CANNOT FOUND PLAYBACK SCRIPT");
                    ConsolePanelPush.AppendText(curtime + " - " + "CANNOT FOUND PLAYBACK SCRIPT");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else
                {
                    curtime = DateTime.Now;
                    log.Error(ex.Message);
                    ConsolePanelPush.AppendText(curtime + " - " + ex.HResult + " --- " + ex.Message);
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                WindowInteraction.FocusWindowNormal(thisProc);
            }
        }

        // Json Playback button
        private void btnPlayback2_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(() => ClearTextBox.ClearValue(ProcessName));
            t1.Start();
            t1.Join();

            playback();
        }

        // Playback button on test steps table // Add log 
        private void btnPlayback_Click(object sender, EventArgs e)
        {

            Thread t1 = new Thread(() => ClearTextBox.ClearValue(ProcessName));
            t1.Start();
            t1.Join();

            playbackTestSteps();
        }
        public void playbackTestSteps()
        {
            ExceptionCode excode = new ExceptionCode();
            var curtime = DateTime.Now;
            Process targetProcess = WindowInteraction.GetProcess(ProcessName);
            WindowInteraction.FocusWindow(targetProcess);
            //ResultPanel.Items.Clear();
            ResultPanelPush.Clear();

            try
            {
                int pbindex = 0;
                PlaybackObjectList = new PlaybackObject[dataGridView2.Rows.Count];
                ElementList = SearchbyFramework("WPF");
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    PlaybackObjectList[pbindex] = new PlaybackObject();
                    PlaybackObjectList[pbindex].index = (int)row.Cells[1].Value;
                    PlaybackObjectList[pbindex].automationId = (string)row.Cells[2].Value;
                    PlaybackObjectList[pbindex].name = (string)row.Cells[3].Value;
                    PlaybackObjectList[pbindex].type = (string)row.Cells[4].Value;
                    PlaybackObjectList[pbindex].action = (string)row.Cells[5].Value;
                    if (PlaybackObjectList[pbindex].action == "Select")
                    {
                        PlaybackObjectList[pbindex].text = "";
                        PlaybackObjectList[pbindex].itemIndex = int.Parse(row.Cells[6].Value.ToString());
                    }
                    else if (PlaybackObjectList[pbindex].action == "SetText")
                    {
                        PlaybackObjectList[pbindex].text = (string)row.Cells[6].Value; ;
                        PlaybackObjectList[pbindex].itemIndex = -1;
                    }
                    pbindex++;
                }
                for (int i = 0; i < PlaybackObjectList.Count(); i++)
                {
                    int flag = 0;
                    if (MainWindow.ModalWindows.Count > 0)
                    {
                        flag = 1;
                        ElementList = MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, "WPF"));
                    }
                    switch (PlaybackObjectList[i].type)
                    {
                        case "Button":
                            AbsAction ButtonAction = new SimplePlayBack.Actions.ButtonAction();
                            ButtonAction.PlaybackObject = PlaybackObjectList[i];
                            ButtonAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            ButtonAction.DoExecute();
                            if (ButtonAction.Result == true)
                            {
                                log.Info("PASSED at item id: " + ButtonAction.PlaybackObject.index + " --- " + "ClassType: " + ButtonAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + ButtonAction.PlaybackObject.index + " --- " + "ClassType: " + ButtonAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            else
                            {
                                log.Fatal("FAILED at item id: " + ButtonAction.PlaybackObject.index + " --- " + "ClassType: " + ButtonAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + ButtonAction.PlaybackObject.index + " --- " + "ClassType: " + ButtonAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }

                            break;
                        case "RadioButton":
                            AbsAction RadioButtonAction = new SimplePlayBack.Actions.RadioButtonAction();
                            RadioButtonAction.PlaybackObject = PlaybackObjectList[i];
                            RadioButtonAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            RadioButtonAction.DoExecute();
                            if (RadioButtonAction.Result == false)
                            {
                                ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + RadioButtonAction.PlaybackObject.index + " --- " + "ClassType: " + RadioButtonAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            else
                            {
                                ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + RadioButtonAction.PlaybackObject.index + " --- " + "ClassType: " + RadioButtonAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }



                            break;
                        case "TextBox":
                            AbsAction TextBoxAction = new SimplePlayBack.Actions.TextBoxAction();
                            TextBoxAction.PlaybackObject = PlaybackObjectList[i];
                            TextBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            TextBoxAction.DoExecute();
                            if (TextBoxAction.Result == true)
                            {
                                log.Info("PASSED at item id: " + TextBoxAction.PlaybackObject.index + " --- " + "ClassType: " + TextBoxAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + TextBoxAction.PlaybackObject.index + " --- " + "ClassType: " + TextBoxAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            else
                            {
                                log.Fatal("FAILED at item id: " + TextBoxAction.PlaybackObject.index + " --- " + "ClassType: " + TextBoxAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + TextBoxAction.PlaybackObject.index + " --- " + "ClassType: " + TextBoxAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            break;

                        case "ComboBox":
                            AbsAction ComboBoxAction = new SimplePlayBack.Actions.ComboBoxAction();
                            ComboBoxAction.PlaybackObject = PlaybackObjectList[i];
                            ComboBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            ComboBoxAction.DoExecute();

                            break;
                        case "ComboBoxEdit":
                            AbsAction ComboBoxEditAction = new SimplePlayBack.Actions.ComboBoxEditAction();
                            ComboBoxEditAction.PlaybackObject = PlaybackObjectList[i];
                            ComboBoxEditAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            ComboBoxEditAction.DoExecute();
                            if (ComboBoxEditAction.Result == false)
                            {
                                log.Fatal("FAILED at item id: " + ComboBoxEditAction.PlaybackObject.index + " --- " + "ClassType: " + ComboBoxEditAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + ComboBoxEditAction.PlaybackObject.index + " --- " + "ClassType: " + ComboBoxEditAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            else
                            {
                                log.Info("PASSED at item id: " + ComboBoxEditAction.PlaybackObject.index + " --- " + "ClassType: " + ComboBoxEditAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + ComboBoxEditAction.PlaybackObject.index + " --- " + "ClassType: " + ComboBoxEditAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }


                            break;
                        case "DataGrid":
                            AbsAction DataGridAction = new SimplePlayBack.Actions.DataGridAction();
                            DataGridAction.PlaybackObject = PlaybackObjectList[i];
                            DataGridAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            DataGridAction.DoExecute();
                            if (DataGridAction.Result == false)
                            {
                                log.Fatal("FAILED at item id: " + DataGridAction.PlaybackObject.index + " --- " + "ClassType: " + DataGridAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "FAILED at item id: " + DataGridAction.PlaybackObject.index + " --- " + "ClassType: " + DataGridAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            else
                            {
                                log.Info("PASSED at item id: " + DataGridAction.PlaybackObject.index + " --- " + "ClassType: " + DataGridAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(curtime + " - " + "PASSED at item id: " + DataGridAction.PlaybackObject.index + " --- " + "ClassType: " + DataGridAction.UiElement.ClassName);
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }

                            break;
                        default:
                            break;
                    }
                    if (flag == 1)
                    {
                        flag = 0;
                        ElementList = MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, "WPF"));
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == excode.AUT_NOT_FOUND)
                {
                    ConsolePanelPush.AppendText(curtime + " - " + "AUT NOT FOUND");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.CANNOT_FOCUS_ON_AUT)
                {
                    ConsolePanelPush.AppendText(curtime + " - " + "CANNOT FOCUS ON AUT OR INPUT WAS NOT ENABLE");
                    ConsolePanelPush.AppendText(Environment.NewLine);

                }
                else if (ex.HResult == excode.INVALID_SCRIPT)
                {
                    ConsolePanelPush.AppendText(curtime + " - " + "CANNOT USE THE CURRENT SCRIPT ON THIS SCREEN");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.SCRIPT_ERROR)
                {
                    ConsolePanelPush.AppendText(curtime + " - " + "SCRIPT FORMAT IS INVALID");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.AUT_QUIT_DURING_OP)
                {
                    ConsolePanelPush.AppendText(curtime + " - " + "AUT QUIT DURING PLAYBACK OPERATION");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.INVALID_INDEX)
                {
                    ConsolePanelPush.AppendText(curtime + " - " + "INPUT VALUE WAS OUT OF RANGE");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.OBJECT_NULL)
                {
                    ConsolePanelPush.AppendText(curtime + " - " + "CANNOT FOUND PLAYBACK SCRIPT");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else
                {
                    ConsolePanelPush.AppendText(curtime + " - " + ex.HResult + " --- " + ex.Message);
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
            }
            WindowInteraction.FocusWindowNormal(thisProc);

        }
        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView2.AllowUserToAddRows = true;
            int count = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                if (chk.Value == chk.TrueValue)
                {
                    count++;
                }
            }
            int[] rowIndex = new int[count];
            int index = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                if (chk.Value == chk.TrueValue)
                {
                    rowIndex[index] = (int)row.Cells[1].Value;
                    index++;
                }
            }
            int contIndex = dataGridView2.Rows.Count;
            for (int i = 0; i < count; i++)
            {

                DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[i].Clone();

                if (dataGridView2.Rows.Count > 0)
                {
                    row.Cells[0].Value = contIndex;
                }
                else
                {
                    row.Cells[0].Value = i + 1;
                }
                row.Cells[1].Value = SpyObjectList[rowIndex[i]].index;
                row.Cells[2].Value = SpyObjectList[rowIndex[i]].automationId;
                row.Cells[3].Value = SpyObjectList[rowIndex[i]].name;
                row.Cells[4].Value = SpyObjectList[rowIndex[i]].type;
                ((DataGridViewComboBoxCell)row.Cells[5]).Items.Clear();
                switch ((string)row.Cells[4].Value)
                {
                    case "Button":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        break;
                    case "RadioButton":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        break;
                    case "TextBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        break;
                    case "ComboBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        break;
                    case "ComboBoxEdit":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        break;
                    case "DataGrid":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("UnSelect");
                        break;
                    default:
                        break;
                }
                dataGridView2.Rows.Add(row);
                contIndex++;
                row.Cells[0].ReadOnly = true;
                row.Cells[2].ReadOnly = true;
                row.Cells[3].ReadOnly = true;
                row.Cells[4].ReadOnly = true;
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {

                DataGridViewCheckBoxCell cb = (DataGridViewCheckBoxCell)row.Cells[0];
                if (cb != null && cb.Value == cb.TrueValue)
                {
                    cb.Value = false;
                    cb.Value = cb.FalseValue;
                }
            }
            dataGridView2.AllowUserToAddRows = false;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int i = 1;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Selected == true)
                {
                    dataGridView2.Rows.RemoveAt(row.Index);
                }
            }

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                row.Cells[0].Value = i;
                i++;
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

            string text = textBox1.Text.ToLower();
            dataGridView1.Rows.Clear();
            dataGridView1.AllowUserToAddRows = true;
            for (int i = 0; i < SpyObjectList.Count(); i++)
            {
                string search = SpyObjectList[i].automationId.ToLower();
                string search1 = SpyObjectList[i].name.ToLower();
                if ((search.Contains(text) == true || search1.Contains(text) == true) && (comboBox1.SelectedItem.ToString() == "All" || (comboBox1.SelectedItem.ToString() == "Interactive Controls" && (SpyObjectList[i].type == "ComboBox" || SpyObjectList[i].type == "ComboBoxEdit" || SpyObjectList[i].type == "DataGrid" || SpyObjectList[i].type == "TextBox" || SpyObjectList[i].type == "Button" || SpyObjectList[i].type == "RadioButton"))) && SpyObjectList[i].automationId.Contains("PART") != true)
                {
                    int rowId = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowId];
                    row.Cells[1].Value = SpyObjectList[i].index;
                    row.Cells[2].Value = SpyObjectList[i].automationId;
                    row.Cells[3].Value = SpyObjectList[i].name;
                    row.Cells[4].Value = SpyObjectList[i].type;
                }
                else if ((search.Contains(text) == true || search1.Contains(text) == true) && (SpyObjectList[i].type == comboBox1.SelectedItem.ToString()) && SpyObjectList[i].automationId.Contains("PART") != true)
                {

                    int rowId = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowId];
                    row.Cells[1].Value = SpyObjectList[i].index;
                    row.Cells[2].Value = SpyObjectList[i].automationId;
                    row.Cells[3].Value = SpyObjectList[i].name;
                    row.Cells[4].Value = SpyObjectList[i].type;
                }

            }
            dataGridView1.AllowUserToAddRows = false;

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView1.AllowUserToAddRows = true;
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Button":

                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if (SpyObjectList[i].type == "Button" && SpyObjectList[i].automationId.Contains("PART") != true)
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[1].Value = SpyObjectList[i].index;
                            row.Cells[2].Value = SpyObjectList[i].automationId;
                            row.Cells[3].Value = SpyObjectList[i].name;
                            row.Cells[4].Value = SpyObjectList[i].type;
                        }
                    }
                    break;
                case "RadioButton":
                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if (SpyObjectList[i].type == "RadioButton")
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[1].Value = SpyObjectList[i].index;
                            row.Cells[2].Value = SpyObjectList[i].automationId;
                            row.Cells[3].Value = SpyObjectList[i].name;
                            row.Cells[4].Value = SpyObjectList[i].type;
                        }
                    }
                    break;
                case "TextBox":
                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if (SpyObjectList[i].type == "TextBox" && SpyObjectList[i].automationId.Contains("PART") != true)
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[1].Value = SpyObjectList[i].index;
                            row.Cells[2].Value = SpyObjectList[i].automationId;
                            row.Cells[3].Value = SpyObjectList[i].name;
                            row.Cells[4].Value = SpyObjectList[i].type;
                        }
                    }
                    break;
                case "ComboBox":
                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if (SpyObjectList[i].type == "ComboBox")
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[1].Value = SpyObjectList[i].index;
                            row.Cells[2].Value = SpyObjectList[i].automationId;
                            row.Cells[3].Value = SpyObjectList[i].name;
                            row.Cells[4].Value = SpyObjectList[i].type;
                        }
                    }
                    break;
                case "ComboBoxEdit":
                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if (SpyObjectList[i].type == "ComboBoxEdit")
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[1].Value = SpyObjectList[i].index;
                            row.Cells[2].Value = SpyObjectList[i].automationId;
                            row.Cells[3].Value = SpyObjectList[i].name;
                            row.Cells[4].Value = SpyObjectList[i].type;
                        }
                    }
                    break;
                case "DataGrid":
                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if (SpyObjectList[i].type == "DataGrid")
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[1].Value = SpyObjectList[i].index;
                            row.Cells[2].Value = SpyObjectList[i].automationId;
                            row.Cells[3].Value = SpyObjectList[i].name;
                            row.Cells[4].Value = SpyObjectList[i].type;
                        }
                    }
                    break;
                case "Interactive Controls":

                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if ((SpyObjectList[i].type == "ComboBox" || SpyObjectList[i].type == "ComboBoxEdit" || SpyObjectList[i].type == "DataGrid" || SpyObjectList[i].type == "TextBox" || SpyObjectList[i].type == "Button" || SpyObjectList[i].type == "RadioButton") && SpyObjectList[i].automationId.Contains("PART") != true)
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[1].Value = SpyObjectList[i].index;
                            row.Cells[2].Value = SpyObjectList[i].automationId;
                            row.Cells[3].Value = SpyObjectList[i].name;
                            row.Cells[4].Value = SpyObjectList[i].type;
                        }
                    }
                    break;
                case "All":
                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        int rowId = dataGridView1.Rows.Add();
                        DataGridViewRow row = dataGridView1.Rows[rowId];
                        row.Cells[1].Value = SpyObjectList[i].index;
                        row.Cells[2].Value = SpyObjectList[i].automationId;
                        row.Cells[3].Value = SpyObjectList[i].name;
                        row.Cells[4].Value = SpyObjectList[i].type;
                    }
                    break;
                default:
                    break;
            }
            dataGridView1.AllowUserToAddRows = false;
        }


        private void InspectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighlightForm HLightForm = new HighlightForm();
            HLightForm.Show();

        }

        private void btnCreateTestSteps_Click(object sender, EventArgs e)
        {
            if (ValidateJSON(rtxtScript.Text) == false)
            {
                System.Windows.Forms.MessageBox.Show("Test script is not valid Json!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            readJson();
            dataGridView2.AllowUserToAddRows = true;
            int count = PlaybackObjectList.Count();

            for (int i = 0; i < count; i++)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[0].Clone();
                if (i == 0)
                    dataGridView2.Rows.Clear();
                row.Cells[0].Value = i + 1;
                row.Cells[1].Value = PlaybackObjectList[i].index;
                row.Cells[2].Value = PlaybackObjectList[i].automationId;
                row.Cells[3].Value = PlaybackObjectList[i].name;
                row.Cells[4].Value = PlaybackObjectList[i].type;
                //((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                ((DataGridViewComboBoxCell)row.Cells[5]).Items.Clear();
                switch (row.Cells[4].Value.ToString())
                {
                    case "Button":
                        //((DataGridViewComboBoxCell)row.Cells[5]).Value = "Click";
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        if (PlaybackObjectList[i].action == "Click")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                        //    ((DataGridViewComboBoxCell)row.Cells[5]).Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                        break;

                    case "RadioButton":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        if (PlaybackObjectList[i].action == "Click")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                        //    ((DataGridViewComboBoxCell)row.Cells[5]).Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                        break;
                    case "TextBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        if (PlaybackObjectList[i].action == "SetText")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                        //if (row.Cells[5].Value.ToString() == "SetText")
                        //    System.Windows.Forms.MessageBox.Show(row.Cells[5].Value.ToString());
                        break;
                    // ((DataGridViewComboBoxCell)row.Cells[5]).Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                    case "ComboBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        switch (PlaybackObjectList[i].action)
                        {
                            case "SetText":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                                break;
                            case "Select":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[1];
                                break;
                            default:
                                break;
                        }
                        break;

                    case "ComboBoxEdit":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        switch (PlaybackObjectList[i].action)
                        {
                            case "SetText":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                                break;
                            case "Select":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[1];
                                break;
                            default:
                                break;
                        }
                        break;

                    case "DataGrid":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("UnSelect");
                        switch (PlaybackObjectList[i].action)
                        {
                            case "Select":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                                break;
                            case "UnSelect":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[1];
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if (PlaybackObjectList[i].text != "")
                    row.Cells[6].Value = PlaybackObjectList[i].text;
                else
                    row.Cells[6].Value = PlaybackObjectList[i].itemIndex;
                dataGridView2.Rows.Add(row);
            }
            dataGridView2.AllowUserToAddRows = false;
        }

        string JsonPath;
        private void btnCreateTestScript_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("There is no data!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //int i = 0;
            //foreach (var scriptFile in scriptFiles)
            //{
            //    if (scriptFile.Name == lbxScriptList.SelectedItem.ToString())
            //    {
            //        JsonPath = scriptFiles[i].Path;
            //        break;
            //    }
            //    i++;
            //}
            //if (PlaybackObjectList == null)
            //System.Windows.Forms.MessageBox.Show(dataGridView2.Rows.Count.ToString());
            PlaybackObjectList = new PlaybackObject[dataGridView2.Rows.Count];
            int pbindex = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                //System.Windows.Forms.MessageBox.Show(row.Cells[1].Value.ToString());
                PlaybackObjectList[pbindex] = new PlaybackObject();
                PlaybackObjectList[pbindex].index = Convert.ToInt32(row.Cells[1].Value);
                PlaybackObjectList[pbindex].automationId = Convert.ToString(row.Cells[2].Value);
                PlaybackObjectList[pbindex].name = Convert.ToString(row.Cells[3].Value);
                PlaybackObjectList[pbindex].type = Convert.ToString(row.Cells[4].Value);
                PlaybackObjectList[pbindex].action = Convert.ToString(row.Cells[5].Value);
                int n;
                bool isNumeric = int.TryParse(Convert.ToString(row.Cells[6].Value), out n);
                if (isNumeric)
                {
                    PlaybackObjectList[pbindex].itemIndex = Convert.ToInt32(row.Cells[6].Value);
                    PlaybackObjectList[pbindex].text = "";
                }
                else
                {
                    PlaybackObjectList[pbindex].text = Convert.ToString(row.Cells[6].Value);
                    PlaybackObjectList[pbindex].itemIndex = -1;
                }

                string json = JsonConvert.SerializeObject(PlaybackObjectList[pbindex], Formatting.Indented);
                if (dataGridView2.Rows.Count == 1)
                {
                    //System.IO.File.WriteAllText(JsonPath, "[{\"Controller\":" + json + "}]");
                    rtxtScript.Text = "[{\"Controller\":" + json + "}]";
                    break;
                }
                if (pbindex == 0)
                    //System.IO.File.WriteAllText(JsonPath, "[{\"Controller\":" + json);
                    rtxtScript.Text = "[{\"Controller\":" + json;
                else if (pbindex == dataGridView2.Rows.Count - 1)
                    //System.IO.File.AppendAllText(JsonPath, "},{\"Controller\":" + json + "}]");
                    rtxtScript.Text = "},{\"Controller\":" + json + "}]";
                else
                    //System.IO.File.AppendAllText(JsonPath, "},{\"Controller\":" + json);
                    rtxtScript.Text = "},{\"Controller\":" + json;
                pbindex++;
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Table doesn't have any data!");
            }
            else
            {
                DataGridView grid = dataGridView2;
                try
                {
                    int totalRows = grid.Rows.Count;
                    int idx = grid.SelectedCells[0].OwningRow.Index;
                    if (idx == 0)
                        return;
                    int col = grid.SelectedCells[0].OwningColumn.Index;
                    DataGridViewRowCollection rows = grid.Rows;
                    DataGridViewRow row = rows[idx];
                    rows.Remove(row);
                    rows.Insert(idx - 1, row);
                    grid.ClearSelection();
                    grid.Rows[idx - 1].Cells[col].Selected = true;
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        dataGridView2.Rows[i].Cells[0].Value = i + 1;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Table doesn't have any data!");
            }
            else
            {
                DataGridView grid = dataGridView2;
                try
                {
                    int totalRows = grid.Rows.Count;
                    int idx = grid.SelectedCells[0].OwningRow.Index;
                    if (idx == totalRows - 1)
                        return;
                    int col = grid.SelectedCells[0].OwningColumn.Index;
                    DataGridViewRowCollection rows = grid.Rows;
                    DataGridViewRow row = rows[idx];
                    rows.Remove(row);
                    rows.Insert(idx + 1, row);
                    grid.ClearSelection();
                    grid.Rows[idx + 1].Cells[col].Selected = true;
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        dataGridView2.Rows[i].Cells[0].Value = i + 1;
                    }
                }
                catch
                {
                    throw;
                }
            }

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            //string JsonPath = theUser.CreateScriptFolder() + @"TestScript.json";
            //rtxtScript.Text = File.ReadAllText(JsonPath);
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var editingControl = this.dataGridView2.EditingControl as DataGridViewComboBoxEditingControl;
            if (editingControl != null)
                editingControl.DroppedDown = true;
        }
        void ProcessForm_OnDataAvailable(object sender, EventArgs e)
        {
            //Event handler for when FormB fires off the event
            ProcessName = processForm.ProcessName;
        }

        private void InitializeProcessForm()
        {
            this.processForm = new ProcessForm();

            //FormA subscribes to FormB's event
            processForm.OnDataAvailable += new EventHandler(ProcessForm_OnDataAvailable);
        }
        private void btnAttach_Click(object sender, EventArgs e)
        {
            InitializeProcessForm();
            processForm.ShowDialog();
            textBox2.Text = ProcessName;
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.SaveFileDialog ExportDialog = new System.Windows.Forms.SaveFileDialog();
            ExportDialog.Title = "Save Test Script File";
            ExportDialog.Filter = "JSON files (*.json)|*.json";
            ExportDialog.RestoreDirectory = true;

            //ExportDialog.InitialDirectory = @"C:\Users\vinhttn.tv\Desktop\";
            if (ExportDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(File.Create(ExportDialog.FileName));
                writer.WriteLine(rtxtScript.Text);
                writer.Dispose();
            }
        }

        private void importToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ExceptionCode excode = new ExceptionCode();

            System.Windows.Forms.OpenFileDialog ImportDialog = new System.Windows.Forms.OpenFileDialog();
            ImportDialog.Title = "Open Test Script File";
            ImportDialog.Filter = "JSON files (*.json)|*.json";
            ImportDialog.Multiselect = true;
            ImportDialog.RestoreDirectory = true;

            //ImportDialog.InitialDirectory = @"C:\";
            try
            {
                if (ImportDialog.ShowDialog() == DialogResult.OK)
                {
                    clbTestScriptList.Items.Clear();
                    //lbxScriptList.SelectionMode = SelectionMode.MultiExtended;
                    scriptFiles = new ScriptFile[ImportDialog.FileNames.Length];
                    int length = ImportDialog.FileNames.Length;
                    //System.Windows.Forms.MessageBox.Show(ImportDialog.SafeFileNames[0].ToString());
                    string result;
                    for (int i = 0; i < length; i++)
                    {
                        //StreamReader reader = new StreamReader(File.OpenRead(ImportDialog.FileNames[i]));
                        //dynamic controls = JsonConvert.DeserializeObject(result);
                        scriptFiles[i] = new ScriptFile();
                        scriptFiles[i].Name = ImportDialog.SafeFileNames[i];
                        scriptFiles[i].Path = ImportDialog.FileNames[i];

                        result = File.ReadAllText(scriptFiles[i].Path);
                        if (ValidateJSON(result) == false)
                        {
                            log.Error("SCRIPT FORMAT IS INVALID");
                            System.Windows.Forms.MessageBox.Show(scriptFiles[i].Name + " : SCRIPT FORMAT IS INVALID", "ERROR!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            clbTestScriptList.Items.Add(scriptFiles[i].Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        public static bool ValidateJSON(string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }

        public void readJson()
        {
            #region oldreadjson
            //Read Json from a test script file
            //if (lbxScriptList.Items.Count == 0)
            //{
            //    System.Windows.Forms.MessageBox.Show("There is no test script file!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //int i = 0;
            //foreach (var scriptFile in scriptFiles)
            //{
            //    if (scriptFile.Name == lbxScriptList.SelectedItem.ToString())
            //    {
            //        readText = File.ReadAllText(scriptFiles[i].Path);
            //        break;
            //    }
            //    i++;
            //}
            #endregion

            string readText = rtxtScript.Text;
            dynamic controls = JsonConvert.DeserializeObject(readText);
            try
            {
                int pbindex = 0;
                PlaybackObjectList = new PlaybackObject[controls.Count];
                foreach (var control in controls)
                {
                    PlaybackObjectList[pbindex] = new PlaybackObject();
                    PlaybackObjectList[pbindex].index = control.Controller.index;
                    PlaybackObjectList[pbindex].automationId = control.Controller.automationId;
                    PlaybackObjectList[pbindex].name = control.Controller.name;
                    PlaybackObjectList[pbindex].type = control.Controller.type;
                    PlaybackObjectList[pbindex].action = control.Controller.action;
                    PlaybackObjectList[pbindex].text = control.Controller.text;
                    PlaybackObjectList[pbindex].itemIndex = control.Controller.itemIndex;
                    //System.Windows.Forms.MessageBox.Show(PlaybackObjectList[pbindex].index.ToString());
                    pbindex++;
                }
            }
            catch (Exception ex)
            {
                ConsolePanelPush.AppendText("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
                ConsolePanelPush.AppendText(Environment.NewLine);
                WindowInteraction.FocusWindowNormal(thisProc);
            }
        }


  
        public void CheckKeyWord(string word, Color color, int StartIndex)
        {
            if (this.ResultPanelPush.Text.Contains(word))
            {
                int index = -1;
                int selectStart = this.ResultPanelPush.SelectionStart;
                while ((index = this.ResultPanelPush.Text.IndexOf(word, (index + 1))) != -1)
                {
                    this.ResultPanelPush.Select((index + StartIndex), word.Length);
                    this.ResultPanelPush.SelectionColor = color;
                    this.ResultPanelPush.Select(selectStart, 0);
                    this.ResultPanelPush.SelectionColor = Color.Black;
                }
            }
        }


        private void clbTestScriptList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbTestScriptList.SelectedItem == null)
            {
                return;
            }
            int i = 0;
            //clbTestScriptList.Refresh();
            foreach (var scriptFile in scriptFiles)
            {
                if (scriptFile.Name == clbTestScriptList.SelectedItem.ToString())
                {
                    rtxtScript.Text = File.ReadAllText(scriptFiles[i].Path);
                    break;
                }
                i++;
            }
        }

        private void btnMoveUpCLB_Click(object sender, EventArgs e)
        {
            // Checking selected item
            if (clbTestScriptList.SelectedItem == null || clbTestScriptList.SelectedIndex <= 0)
                return; // No selected item - nothing to do
            // add a duplicate item up in the listbox
            int oldIndex = clbTestScriptList.SelectedIndex + 1;
            clbTestScriptList.Items.Insert(clbTestScriptList.SelectedIndex - 1, clbTestScriptList.SelectedItem);
            // make it the current item
            clbTestScriptList.SelectedIndex = (clbTestScriptList.SelectedIndex - 2);
            if (clbTestScriptList.GetItemCheckState(oldIndex) == CheckState.Checked)
                clbTestScriptList.SetItemChecked(clbTestScriptList.SelectedIndex, true);
            // delete the old occurrence of this item
            clbTestScriptList.Items.RemoveAt(clbTestScriptList.SelectedIndex + 2);
        }

        private void btnMoveDownCLB_Click(object sender, EventArgs e)
        {
            // Checking selected item
            if (clbTestScriptList.SelectedItem == null || clbTestScriptList.SelectedIndex >= clbTestScriptList.Items.Count - 1)
                return; // No selected item - nothing to do
            int IndexToRemove = clbTestScriptList.SelectedIndex;
            // add a duplicate item down in the listbox
            clbTestScriptList.Items.Insert(clbTestScriptList.SelectedIndex + 2, clbTestScriptList.SelectedItem);
            // make it the current item
            clbTestScriptList.SelectedIndex = (clbTestScriptList.SelectedIndex + 2);
            if (clbTestScriptList.GetItemCheckState(IndexToRemove) == CheckState.Checked)
                clbTestScriptList.SetItemChecked(clbTestScriptList.SelectedIndex, true);
            // delete the old occurrence of this item
            clbTestScriptList.Items.RemoveAt(IndexToRemove);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clbTestScriptList.SelectedItem = 2;

            foreach (var item in clbTestScriptList.CheckedItems)
            {
                //System.Windows.Forms.MessageBox.Show(clbTestScriptList.Items.IndexOf(item).ToString());
                //clbTestScriptList.SelectedItem = clbTestScriptList.Items.IndexOf(item);
                //.Sleep(2000);
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            var curtime = DateTime.Now;
            if (!string.IsNullOrEmpty(PublicMembers.theMessage))
            {
                
                ConsolePanelPush.AppendText(PublicMembers.theCurtime + " - " + PublicMembers.theMessage + " HAS BEEN ATTACHED");
                ConsolePanelPush.AppendText(Environment.NewLine);
                PublicMembers.theMessage = null;
            }

            if(!string.IsNullOrEmpty(ClearTextBox.theMessage))
            {
                ConsolePanelPush.AppendText(curtime + " - " + "PLAYBACK DONE");
                ConsolePanelPush.AppendText(Environment.NewLine);

                ConsolePanelPush.AppendText(ClearTextBox.theMessage);
                ConsolePanelPush.AppendText(Environment.NewLine);
                ClearTextBox.theMessage = null;
            }

        }

        private void ResultPanelPush_TextChanged(object sender, EventArgs e)
        {
            this.CheckKeyWord("PASSED", Color.Green, 0);
            this.CheckKeyWord("FAILED", Color.Red, 0);
            this.CheckKeyWord("item id", Color.OrangeRed, 0);
            this.CheckKeyWord("ClassType", Color.Blue, 0);
        }

    }
}
