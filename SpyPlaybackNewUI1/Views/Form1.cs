using Gu.Wpf.UiAutomation;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpyandPlaybackTestTool.Actions;
using SpyandPlaybackTestTool.SpyPlaybackObjects;
using SpyandPlaybackTestTool.Ultils;
using SpyandPlaybackTestTool.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool
{
    public partial class Form1 : Form
    {
        #region Variables

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// GU.WPF UiElement
        /// </summary>
        private IReadOnlyList<UiElement> ElementList;


        /// <summary>
        /// Store UiElement of the AUT 
        /// </summary>
        private SpyObject[] SpyObjectList;

        /// <summary>
        /// Store playback steps
        /// </summary>
        private PlaybackObject[] PlaybackObjectList;
        //private ScriptFile[] scriptFiles;
            
        /// <summary>
        /// Store lists of playback scripts
        /// </summary>
        private List<ScriptFile> scriptFiles = new List<ScriptFile>();
        private Process thisProc = Process.GetCurrentProcess();

        public static Process AUTPROC;

        /// <summary>
        /// Check if AUT exited
        /// </summary>
        public bool checkExited;

        /// <summary>
        /// Playback percentage for ProgressForm
        /// </summary>
        public static int playbackprogress;

        /// <summary>
        /// Playback is on your off
        /// </summary>
        public static bool playbackstatus;


        /// <summary>
        /// Get, set playback is success or not
        /// </summary>
        public static bool playbacksuccess;

        /// <summary>
        /// Use for Stop playback while running
        /// </summary>
        public static bool stop_playback;

        /// <summary>
        /// Check if scenario playback is ON. 
        /// Prevent PlaybackTestScript focuses this process when finished a test script in scenario array
        /// </summary>
        public bool ScenarioStatus;

        /// <summary>
        /// Progress Form instance
        /// </summary>
        ProgressForm pf = new ProgressForm();

        #endregion Variables

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log.Info("PROGRAM STARTED");
            
            this.KeyPreview = true;

            ResultPanelPush.ReadOnly = true;
            ConsolePanelPush.ReadOnly = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;

            (dataGridView1.Columns[0] as DataGridViewCheckBoxColumn).TrueValue = true;
            (dataGridView1.Columns[0] as DataGridViewCheckBoxColumn).FalseValue = false;

            //dataGridView2.AllowUserToResizeRows = false;
            foreach (DataGridViewColumn col in dataGridView2.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.RowHeadersVisible = false;
            
            //dataGridView2.Columns[1].Visible = false;

            redcircleTip.Visible = true;
            greencircleTip.Visible = false;

            textBox1.Enabled = false;
            comboBox1.Enabled = false;

            //this.MaximumSize = new Size(XX, YY);
            this.MinimumSize = new Size(1280, 720);
            
            //pf.Show();
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            ProcessForm nw = new ProcessForm();
            nw.Show();
           
        }

        #region Realtime AUT checking

        /// <summary>
        /// Realtime checking AUT if it's alive or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (ProcessForm.isAttached.Equals(false))
            {
                ProcessForm.targetproc = null;
                AUTPROC = null;
                toolStripStatusLabel1.Text = "";
            }
            else if (ProcessForm.isAttached.Equals(true))
            {
                AUTPROC = WindowInteraction.GetProcess(ProcessForm.targetproc);
                //WindowInteraction.FocusWindow(thisProc);
                toolStripStatusLabel1.Text = ProcessForm.targetproc;
                redcircleTip.Visible = false;
                greencircleTip.Visible = true;
    
                timer1.Start();
            }
        }


        /// <summary>
        /// Realtime checking AUT if it's alive or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AUTPROC.HasExited)
            {
                toolStripStatusLabel1.Text = "";
                ProcessForm.targetproc = null;
                ProcessForm.isAttached = false;
                AUTPROC = null;
                redcircleTip.Visible = true;
                greencircleTip.Visible = false;
  
                timer1.Stop();
                
                System.Windows.Forms.MessageBox.Show("The AUT: " + ProcessForm.targetproc + " has been terminated.", "Warning!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region SPY
        // Spy Button
        private void button1_Click(object sender, EventArgs e)
        {
            //Spy("normal");
            Thread T_SPY = new Thread(() => Spy("normal"));
            T_SPY.Priority = ThreadPriority.Highest;
            T_SPY.IsBackground = true;
            T_SPY.Start();
            T_SPY.Join();
        }

        /// <summary>
        /// SPY FUNCTION
        /// </summary>
        /// <param name="mode">normal or respy</param>
        public void Spy(string mode)
        {
            ExceptionCode excode = new ExceptionCode();

            if (ProcessForm.targetproc == null)
            {
                System.Windows.Forms.MessageBox.Show("Please attach AUT process to execute Spy function!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                if (mode == "normal")
                {
                    log.Info("BEGIN SPY");
                    DoSpy.GetMainWindow();
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "BEGIN SPY" + Environment.NewLine);
                }
                if (mode == "respy")
                {
                    log.Info("BEGIN RESPY");
                    DoSpy.GetMainWindow();
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "BEGIN RESPY" + Environment.NewLine);
                }

                ElementList = DoSpy.SearchbyFramework("WPF");

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

                if (mode == "normal")
                {
                    log.Info("DONE SPY");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "DONE SPY" + Environment.NewLine);
                }
                if (mode == "respy")
                {
                    log.Info("DONE RESPY");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "DONE RESPY" + Environment.NewLine);
                }

                textBox1.Enabled = true;
                comboBox1.Enabled = true;
                //WindowInteraction.FocusWindowNormal(thisProc);
                dataGridView1.AllowUserToAddRows = false;
                textBox1.Text = "";
                comboBox1.SelectedIndex = 0;

                // Capture to File
                //CaptureToImage.DoCapture(ProcessName);
                if (mode == "normal")
                {
                    WindowInteraction.FocusWindow(thisProc);
                }
                if (mode == "respy")
                {
                    WindowInteraction.FocusWindow(AUTPROC);
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == excode.AUT_NOT_FOUND)
                {
                    log.Error(DateTime.Now + " - AUT NOT FOUND");
                    ConsolePanelPush.AppendText(DateTime.Now + " - AUT NOT FOUND");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                    WindowInteraction.FocusWindowNormal(thisProc);
                }
                else if (ex.HResult == excode.NOT_WPF_PROGRAM)
                {
                    log.Error("CANNOT SPY THIS PROGRAM");
                    ConsolePanelPush.AppendText(DateTime.Now + " - CANNOT SPY THIS PROGRAM");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                    WindowInteraction.FocusWindowNormal(thisProc);
                }
                else if (ex.HResult == excode.OBJECT_NULL)
                {
                    log.Error("AUT NOT FOUND");
                    ConsolePanelPush.AppendText(DateTime.Now + " - AUT NOT FOUND" + Environment.NewLine);
                    // ConsolePanelPush.AppendText();
                    WindowInteraction.FocusWindowNormal(thisProc);
                }
                else
                {
                    log.Error("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
                    ConsolePanelPush.AppendText("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
                    ConsolePanelPush.AppendText(Environment.NewLine);
                    WindowInteraction.FocusWindowNormal(thisProc);
                }
            }
        }

        #endregion

        #region PLAYBACK TEST SCRIPT
        //Json Playback button
        public void btnPlaybackTestScript_Click(object sender, EventArgs e)
        {
            pf.Show();
            playbackprogress = 0;
            playbackstatus = true;
            stop_playback = false;

            if (ProcessForm.targetproc == null)
            {
                System.Windows.Forms.MessageBox.Show("Please attach AUT process to execute Playback Test Script function!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (ValidateJSON(rtxtScript.Text) == false)
            {
                System.Windows.Forms.MessageBox.Show("Test Script format is invalid Json!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Thread t1 = new Thread(() => ClearTextBox.ClearValue(ProcessForm.targetproc));
            t1.Start();
            t1.Join();

            readJson();

            ElementList = DoSpy.SearchbyFramework("WPF");

            //WindowInteraction.FocusWindow(targetProc);
            ResultPanelPush.Clear();
            ConsolePanelPush.AppendText(DateTime.Now + " - " + "BEGIN PLAYBACK");
            log.Info("BEGIN PLAYBACK");
            ConsolePanelPush.AppendText(Environment.NewLine);

            Thread t2 = new Thread(() => PlaybackTestScript());
            t2.Start();

            ConsolePanelPush.AppendText(DateTime.Now + " - " + "DONE PLAYBACK");
            log.Info("DONE PLAYBACK");
            ConsolePanelPush.AppendText(Environment.NewLine);
            //WindowInteraction.FocusWindowNormal(thisProc);
        }

        /// <summary>
        /// PLAYBACK TEST SCRIPT FUNCTION
        /// </summary>
        public void PlaybackTestScript()
        {
            ExceptionCode excode = new ExceptionCode();
            try
            {
                int percentage = 100 / PlaybackObjectList.Count();

                for (int i = 0; i < PlaybackObjectList.Count(); i++)
                {
                    if(stop_playback.Equals(true))
                    {
                        WindowInteraction.FocusWindow(thisProc);
                        return;
                    }

                    WindowInteraction.FocusWindow(AUTPROC);
                    if (DoSpy.MainWindow.ModalWindows.Count > 0)
                        ElementList = DoSpy.SearchbyFramework("WPF");

                    // Check if Json type is valid
                    if (PlaybackObjectList[i].type == "SendKey" || PlaybackObjectList[i].type == "Button" ||
                       PlaybackObjectList[i].type == "TextBox" || PlaybackObjectList[i].type == "WaitEnable" ||
                       PlaybackObjectList[i].type == "ComboBox" || PlaybackObjectList[i].type == "ComboBoxEdit" ||
                       PlaybackObjectList[i].type == "DataGrid" || PlaybackObjectList[i].type == "RadioButton" || PlaybackObjectList[i].type == "AutoCompleteCombobox" ||
                       PlaybackObjectList[i].type == "SendKeyorWaitEnable" || PlaybackObjectList[i].type == "CheckBox" || PlaybackObjectList[i].type == "TabItem" ||
                       PlaybackObjectList[i].type == "PasswordBox" || PlaybackObjectList[i].type == "RichTextBox")
                    {
                        if (PlaybackObjectList[i].action == "Click" || PlaybackObjectList[i].action == "DoubleClick" ||
                           PlaybackObjectList[i].action == "Select" || PlaybackObjectList[i].action == "SetText" ||
                           PlaybackObjectList[i].action == "WaitEnable" || PlaybackObjectList[i].action == "Unselect" ||
                           PlaybackObjectList[i].action == "SendKey")
                        {
                            switch (PlaybackObjectList[i].type)
                            {
                                case "Button":
                                    AbsAction buttonaction = new ButtonAction();
                                    buttonaction.PlaybackObject = PlaybackObjectList[i];
                                    buttonaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    buttonaction.DoExecute();
                                    if (buttonaction.Result == true)
                                        PlaybackLogger(buttonaction.PlaybackObject.index, buttonaction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(buttonaction.PlaybackObject.index, buttonaction.UiElement.ClassName, false);
                                    break;

                                case "TextBox":
                                    AbsAction textboxaction = new TextBoxAction();
                                    textboxaction.PlaybackObject = PlaybackObjectList[i];
                                    textboxaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    textboxaction.DoExecute();
                                    if (textboxaction.Result == true)
                                        PlaybackLogger(textboxaction.PlaybackObject.index, textboxaction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(textboxaction.PlaybackObject.index, textboxaction.UiElement.ClassName, false);
                                    break;

                                case "RichTextBox":
                                    AbsAction RichTextBoxAction = new RichTextBoxAction();
                                    RichTextBoxAction.PlaybackObject = PlaybackObjectList[i];
                                    RichTextBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    RichTextBoxAction.DoExecute();
                                    if (RichTextBoxAction.Result == true)
                                        PlaybackLogger(RichTextBoxAction.PlaybackObject.index, RichTextBoxAction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(RichTextBoxAction.PlaybackObject.index, RichTextBoxAction.UiElement.ClassName, false);
                                    break;

                                case "PasswordBox":
                                    AbsAction PasswordBoxAction = new PasswordBoxAction();
                                    PasswordBoxAction.PlaybackObject = PlaybackObjectList[i];
                                    PasswordBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    PasswordBoxAction.DoExecute();
                                    if (PasswordBoxAction.Result == true)
                                        PlaybackLogger(PasswordBoxAction.PlaybackObject.index, PasswordBoxAction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(PasswordBoxAction.PlaybackObject.index, PasswordBoxAction.UiElement.ClassName, false);
                                    break;

                                case "ComboBox":
                                    AbsAction comboboxaction = new ComboBoxAction();
                                    comboboxaction.PlaybackObject = PlaybackObjectList[i];
                                    comboboxaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    comboboxaction.DoExecute();
                                    if (comboboxaction.Result == true)
                                        PlaybackLogger(comboboxaction.PlaybackObject.index, comboboxaction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(comboboxaction.PlaybackObject.index, comboboxaction.UiElement.ClassName, false);
                                    break;

                                case "AutoCompleteCombobox":
                                    AbsAction ATcomboboxaction = new ComboBoxEditAction();
                                    ATcomboboxaction.PlaybackObject = PlaybackObjectList[i];
                                    ATcomboboxaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    ATcomboboxaction.DoExecute();
                                    if (ATcomboboxaction.Result == true)
                                        PlaybackLogger(ATcomboboxaction.PlaybackObject.index, ATcomboboxaction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(ATcomboboxaction.PlaybackObject.index, ATcomboboxaction.UiElement.ClassName, false);
                                    break;

                                case "ComboBoxEdit":
                                    AbsAction comboboxeditaction = new ComboBoxEditAction();
                                    comboboxeditaction.PlaybackObject = PlaybackObjectList[i];
                                    comboboxeditaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    comboboxeditaction.DoExecute();
                                    if (comboboxeditaction.Result == true)
                                        PlaybackLogger(comboboxeditaction.PlaybackObject.index, comboboxeditaction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(comboboxeditaction.PlaybackObject.index, comboboxeditaction.UiElement.ClassName, false);

                                    break;

                                case "DataGrid":
                                    AbsAction datagridaction = new DataGridAction();
                                    datagridaction.PlaybackObject = PlaybackObjectList[i];
                                    datagridaction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    datagridaction.DoExecute();
                                    if (datagridaction.Result == true)
                                        PlaybackLogger(datagridaction.PlaybackObject.index, datagridaction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(datagridaction.PlaybackObject.index, datagridaction.UiElement.ClassName, false);
                                    break;

                                case "RadioButton":
                                    AbsAction RadioButtonAction = new RadioButtonAction();
                                    RadioButtonAction.PlaybackObject = PlaybackObjectList[i];
                                    RadioButtonAction.UiElement = ElementList[PlaybackObjectList[i].index];
                                    RadioButtonAction.DoExecute();
                                    if (RadioButtonAction.Result == true)
                                        PlaybackLogger(RadioButtonAction.PlaybackObject.index, RadioButtonAction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(RadioButtonAction.PlaybackObject.index, RadioButtonAction.UiElement.ClassName, false);
                                    break;

                                case "CheckBox":
                                    AbsAction CheckBoxAction = new CheckBoxAction();
                                    CheckBoxAction.PlaybackObject = PlaybackObjectList[i];
                                    CheckBoxAction.UiElement = ElementList[CheckBoxAction.PlaybackObject.index];
                                    CheckBoxAction.DoExecute();
                                    if (CheckBoxAction.Result == true)
                                        PlaybackLogger(CheckBoxAction.PlaybackObject.index, CheckBoxAction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(CheckBoxAction.PlaybackObject.index, CheckBoxAction.UiElement.ClassName, false);
                                    break;

                                case "TabItem":
                                    AbsAction TabItemAction = new TabItemAction();
                                    TabItemAction.PlaybackObject = PlaybackObjectList[i];
                                    TabItemAction.UiElement = ElementList[TabItemAction.PlaybackObject.index];

                                    TabItemAction.DoExecute();
                                    if (TabItemAction.Result == true)
                                        PlaybackLogger(TabItemAction.PlaybackObject.index, TabItemAction.UiElement.ClassName, true);
                                    else
                                        PlaybackLogger(TabItemAction.PlaybackObject.index, TabItemAction.UiElement.ClassName, false);
                                    break;

                                case "SendKeyorWaitEnable":
                                    if (PlaybackObjectList[i].action == "SendKey")
                                    {
                                        AbsAction SendKeyAction = new SpyandPlaybackTestTool.Actions.SendKey();
                                        SendKeyAction.PlaybackObject = PlaybackObjectList[i];
                                        SendKeyAction.DoExecute();

                                        log.Info("Key " + PlaybackObjectList[i].text + " HAS BEEN SENT");
                                        ResultPanelPush.AppendText(DateTime.Now + " - " + "Key " + PlaybackObjectList[i].text + " HAS BEEN SENT");
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    else if (PlaybackObjectList[i].action == "WaitEnable")
                                    {
                                        AbsAction WaitEnable = new WaitEnable();
                                        WaitEnable.PlaybackObject = PlaybackObjectList[i];
                                        WaitEnable.DoExecute();

                                        log.Info("WATIED FOR " + int.Parse(WaitEnable.PlaybackObject.text) / 1000 + "s");
                                        ResultPanelPush.AppendText(DateTime.Now + " - " + "WATIED FOR " + int.Parse(WaitEnable.PlaybackObject.text) / 1000 + "s");
                                        ResultPanelPush.AppendText(Environment.NewLine);
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            log.Fatal("FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID ACTION: " + PlaybackObjectList[i].action);
                            ResultPanelPush.AppendText(DateTime.Now + " - " + "FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID ACTION: " + PlaybackObjectList[i].action);
                            ResultPanelPush.AppendText(Environment.NewLine);
                        }
                    }
                    else
                    {
                        log.Fatal("FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID TYPE: " + PlaybackObjectList[i].type);
                        ResultPanelPush.AppendText(DateTime.Now + " - " + "FAILED at " + "JSON INDEX " + PlaybackObjectList[i].index + " - " + "HAS AN INVALID TYPE: " + PlaybackObjectList[i].type);
                        ResultPanelPush.AppendText(Environment.NewLine);
                    }
                    if (DoSpy.MainWindow.ModalWindows.Count > 0)
                        ElementList = DoSpy.SearchbyFramework("WPF");
                    playbackprogress += percentage;
                }
                playbackstatus = false;

                if(ScenarioStatus.Equals(true))
                {
                    
                }
                else
                WindowInteraction.FocusWindow(thisProc);
                log.Info("");
            }
            catch (Exception ex)
            {
                if (ex.HResult == excode.AUT_NOT_FOUND)
                {
                    log.Error("AUT NOT FOUND");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "AUT NOT FOUND");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.CANNOT_FOCUS_ON_AUT)
                {
                    log.Error("CANNOT FOCUS ON AUT OR INPUT WAS NOT ENABLE");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "CANNOT FOCUS ON AUT OR INPUT WAS NOT ENABLE");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.INVALID_SCRIPT)
                {
                    log.Error("CANNOT USE THE CURRENT SCRIPT ON THIS SCREEN");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "CANNOT USE THE CURRENT SCRIPT ON THIS SCREEN");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.SCRIPT_ERROR)
                {
                    log.Error("SCRIPT FORMAT IS INVALID");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "SCRIPT FORMAT IS INVALID");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.AUT_QUIT_DURING_OP)
                {
                    log.Error("AUT QUIT DURING PLAYBACK OPERATION");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "AUT QUIT DURING PLAYBACK OPERATION");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.INVALID_INDEX)
                {
                    log.Error("INPUT VALUE WAS OUT OF RANGE");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "INPUT VALUE WAS OUT OF RANGE");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.OBJECT_NULL)
                {
                    log.Error("CANNOT FOUND PLAYBACK SCRIPT");
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "CANNOT FOUND PLAYBACK SCRIPT");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else
                {
                    log.Error(ex.Message);
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + ex.HResult + " --- " + ex.Message);
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                WindowInteraction.FocusWindow(thisProc);
            }
        }

        #endregion


        #region PLAYBACK TEST STEP
        // Playback button on test steps table // Add log
        private void btnPlaybackTestSteps_Click(object sender, EventArgs e)
        {
            if (ProcessForm.targetproc == null)
            {
                System.Windows.Forms.MessageBox.Show("Please attach AUT process to execute Playback Test Steps function!", "WARNING!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (dataGridView2.Rows.Count <= 0)
            {
                System.Windows.Forms.MessageBox.Show("There is no data!", "WARNING!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            playbackprogress = 0;
            playbackstatus = true;
            stop_playback = false;
  
            pf.TopMost = true;

            if (Settings.ShowProgressBar.Equals(true))
                pf.Show();
            else
                pf.Hide();


            Thread T_ClearValue = new Thread(() => ClearTextBox.ClearValue(ProcessForm.targetproc));
            T_ClearValue.Start();
            T_ClearValue.Join();

            Thread T_PlaybackTestSteps = new Thread(()=>PlaybackTestSteps());
            T_PlaybackTestSteps.Priority = ThreadPriority.Highest;
            T_PlaybackTestSteps.Start();
            T_PlaybackTestSteps.IsBackground = true;
        
             
        }

        /// <summary>
        /// PLAYBACK TEST STEPS
        /// </summary>
        public void PlaybackTestSteps()
        {
            ExceptionCode excode = new ExceptionCode();
            
            
            

            ResultPanelPush.Clear();
            ConsolePanelPush.AppendText(DateTime.Now + " - BEGIN PLAYBACK" + Environment.NewLine);
 
            try
            {
                int pbindex = 0;
                
                PlaybackObjectList = new PlaybackObject[dataGridView2.Rows.Count];

                ElementList = DoSpy.SearchbyFramework("WPF");
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    PlaybackObjectList[pbindex] = new PlaybackObject();
                    PlaybackObjectList[pbindex].index = (int)row.Cells[1].Value;
                    PlaybackObjectList[pbindex].automationId = (string)row.Cells[2].Value;
                    PlaybackObjectList[pbindex].name = (string)row.Cells[3].Value;
                    PlaybackObjectList[pbindex].type = (string)row.Cells[4].Value;
                    PlaybackObjectList[pbindex].action = (string)row.Cells[5].Value;
                    if (PlaybackObjectList[pbindex].action == "Select" || PlaybackObjectList[pbindex].action == "Unselect")
                    {
                        PlaybackObjectList[pbindex].text = "";
                        if (PlaybackObjectList[pbindex].type != "CheckBox")
                            PlaybackObjectList[pbindex].itemIndex = int.Parse(row.Cells[6].Value.ToString());
                    }
                    else if (PlaybackObjectList[pbindex].action == "SetText" ||
                        PlaybackObjectList[pbindex].action == "WaitEnable" ||
                        PlaybackObjectList[pbindex].action == "SendKey" ||
                        PlaybackObjectList[pbindex].action == "IsExist" ||
                        PlaybackObjectList[pbindex].action == "IsNotExist" ||
                        PlaybackObjectList[pbindex].action == "IsEmpty" ||
                        PlaybackObjectList[pbindex].action == "IsEqual" ||
                        PlaybackObjectList[pbindex].action == "IsReadOnly" ||
                        PlaybackObjectList[pbindex].action == "IsEnabled" ||
                        PlaybackObjectList[pbindex].action == "IsSelected")
                    {
                        PlaybackObjectList[pbindex].text = (string)row.Cells[6].Value; ;
                        PlaybackObjectList[pbindex].itemIndex = -1;
                    }

                    pbindex++;
                }

                // Percentage for ProgressBar
                int percentage = 100 / PlaybackObjectList.Count();

                for (int i = 0; i < PlaybackObjectList.Count(); i++)
                {

                    if(stop_playback.Equals(true))
                    {
                        WindowInteraction.FocusWindow(thisProc);
                        return;
                    }

                    int flag = 0;
                    if (DoSpy.MainWindow.ModalWindows.Count > 0)
                    {
                        flag = 1;
                        ElementList = DoSpy.SearchbyFramework("WPF");
                    }

                    // Add more playback actions here
                    switch (PlaybackObjectList[i].type)
                    {
                        case "Button":
                            AbsAction ButtonAction = new ButtonAction();
                            ButtonAction.PlaybackObject = PlaybackObjectList[i];
                            ButtonAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            ButtonAction.DoExecute();
                            if (ButtonAction.Result == true)
                            {
                                PlaybackLogger(ButtonAction.PlaybackObject.index, ButtonAction.UiElement.ClassName, true);
                            }
                            else
                            {
                                PlaybackLogger(ButtonAction.PlaybackObject.index, ButtonAction.UiElement.ClassName, false);
                            }

                            break;

                        case "RadioButton":
                            AbsAction RadioButtonAction = new RadioButtonAction();
                            RadioButtonAction.PlaybackObject = PlaybackObjectList[i];
                            RadioButtonAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            RadioButtonAction.DoExecute();
                            if (RadioButtonAction.Result == true)
                                PlaybackLogger(RadioButtonAction.PlaybackObject.index, RadioButtonAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(RadioButtonAction.PlaybackObject.index, RadioButtonAction.UiElement.ClassName, false);
                            break;

                        case "TextBox":
                            AbsAction TextBoxAction = new TextBoxAction();
                            TextBoxAction.PlaybackObject = PlaybackObjectList[i];
                            TextBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            TextBoxAction.DoExecute();
                            if (TextBoxAction.Result == true)
                                PlaybackLogger(TextBoxAction.PlaybackObject.index, TextBoxAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(TextBoxAction.PlaybackObject.index, TextBoxAction.UiElement.ClassName, false);
                            break;

                        case "RichTextBox":
                            AbsAction RichTextBoxAction = new RichTextBoxAction();
                            RichTextBoxAction.PlaybackObject = PlaybackObjectList[i];
                            RichTextBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            RichTextBoxAction.DoExecute();
                            if (RichTextBoxAction.Result == true)
                                PlaybackLogger(RichTextBoxAction.PlaybackObject.index, RichTextBoxAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(RichTextBoxAction.PlaybackObject.index, RichTextBoxAction.UiElement.ClassName, false);
                            break;

                        case "PasswordBox":
                            AbsAction PasswordBoxAction = new PasswordBoxAction();
                            PasswordBoxAction.PlaybackObject = PlaybackObjectList[i];
                            PasswordBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            PasswordBoxAction.DoExecute();
                            if (PasswordBoxAction.Result == true)
                                PlaybackLogger(PasswordBoxAction.PlaybackObject.index, PasswordBoxAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(PasswordBoxAction.PlaybackObject.index, PasswordBoxAction.UiElement.ClassName, false);
                            break;

                        case "ComboBox":
                            AbsAction ComboBoxAction = new SpyandPlaybackTestTool.Actions.ComboBoxAction();
                            ComboBoxAction.PlaybackObject = PlaybackObjectList[i];
                            ComboBoxAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            ComboBoxAction.DoExecute();
                            if (ComboBoxAction.Result == true)
                                PlaybackLogger(ComboBoxAction.PlaybackObject.index, ComboBoxAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(ComboBoxAction.PlaybackObject.index, ComboBoxAction.UiElement.ClassName, false);
                            break;

                        case "ComboBoxEdit":
                            AbsAction ComboBoxEditAction = new SpyandPlaybackTestTool.Actions.ComboBoxEditAction();
                            ComboBoxEditAction.PlaybackObject = PlaybackObjectList[i];
                            ComboBoxEditAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            ComboBoxEditAction.DoExecute();
                            if (ComboBoxEditAction.Result == true)
                                PlaybackLogger(ComboBoxEditAction.PlaybackObject.index, ComboBoxEditAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(ComboBoxEditAction.PlaybackObject.index, ComboBoxEditAction.UiElement.ClassName, false);
                            break;

                        case "AutoCompleteCombobox":
                            AbsAction ATComboBoxEditAction = new ComboBoxEditAction();
                            ATComboBoxEditAction.PlaybackObject = PlaybackObjectList[i];
                            ATComboBoxEditAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            ATComboBoxEditAction.DoExecute();
                            if (ATComboBoxEditAction.Result == true)
                                PlaybackLogger(ATComboBoxEditAction.PlaybackObject.index, ATComboBoxEditAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(ATComboBoxEditAction.PlaybackObject.index, ATComboBoxEditAction.UiElement.ClassName, false);
                            break;

                        case "DataGrid":
                            AbsAction DataGridAction = new SpyandPlaybackTestTool.Actions.DataGridAction();
                            DataGridAction.PlaybackObject = PlaybackObjectList[i];
                            DataGridAction.UiElement = ElementList[PlaybackObjectList[i].index];
                            DataGridAction.DoExecute();
                            if (DataGridAction.Result == true)
                                PlaybackLogger(DataGridAction.PlaybackObject.index, DataGridAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(DataGridAction.PlaybackObject.index, DataGridAction.UiElement.ClassName, false);
                            break;

                        case "SendKeyorWaitEnable":
                            if (PlaybackObjectList[i].action == "SendKey")
                            {
                                AbsAction SendKeyAction = new SpyandPlaybackTestTool.Actions.SendKey();
                                SendKeyAction.PlaybackObject = PlaybackObjectList[i];
                                SendKeyAction.DoExecute();

                                log.Info("Key " + PlaybackObjectList[i].text + " HAS BEEN SENT");
                                ResultPanelPush.AppendText(DateTime.Now + " - " + "Key " + PlaybackObjectList[i].text + " HAS BEEN SENT");
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            else if (PlaybackObjectList[i].action == "WaitEnable")
                            {
                                AbsAction WaitEnable = new WaitEnable();
                                WaitEnable.PlaybackObject = PlaybackObjectList[i];
                                WaitEnable.DoExecute();

                                log.Info("WATIED FOR " + int.Parse(WaitEnable.PlaybackObject.text) / 1000 + "s");
                                ResultPanelPush.AppendText(DateTime.Now + " - " + "WATIED FOR " + int.Parse(WaitEnable.PlaybackObject.text) / 1000 + "s");
                                ResultPanelPush.AppendText(Environment.NewLine);
                            }
                            break;

                        case "CheckBox":
                            AbsAction CheckBoxAction = new CheckBoxAction();
                            CheckBoxAction.PlaybackObject = PlaybackObjectList[i];
                            CheckBoxAction.UiElement = ElementList[CheckBoxAction.PlaybackObject.index];
                            CheckBoxAction.DoExecute();
                            if (CheckBoxAction.Result == true)
                                PlaybackLogger(CheckBoxAction.PlaybackObject.index, CheckBoxAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(CheckBoxAction.PlaybackObject.index, CheckBoxAction.UiElement.ClassName, false);
                            break;

                        case "TabItem":
                            AbsAction TabItemAction = new TabItemAction();
                            TabItemAction.PlaybackObject = PlaybackObjectList[i];
                            TabItemAction.UiElement = ElementList[TabItemAction.PlaybackObject.index];

                            TabItemAction.DoExecute();
                            if (TabItemAction.Result == true)
                                PlaybackLogger(TabItemAction.PlaybackObject.index, TabItemAction.UiElement.ClassName, true);
                            else
                                PlaybackLogger(TabItemAction.PlaybackObject.index, TabItemAction.UiElement.ClassName, false);
                            break;

                        default:
                            break;
                    }

                    if (flag == 1)
                    {
                        flag = 0;
                        ElementList = DoSpy.SearchbyFramework("WPF");
                    }
                    playbackprogress += percentage;
                }

                playbackstatus = false;
                playbacksuccess = true;
                playbackprogress = 100;
   
                pf.TopMost = false;

                Spy("respy");
                ConsolePanelPush.AppendText(DateTime.Now + " - DONE PLAYBACK");
                ConsolePanelPush.AppendText(Environment.NewLine);
                WindowInteraction.FocusWindow(thisProc);
            }
            catch (Exception ex)
            {
                if (ex.HResult == excode.AUT_NOT_FOUND)
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "AUT NOT FOUND");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.CANNOT_FOCUS_ON_AUT)
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "CANNOT FOCUS ON AUT OR INPUT WAS NOT ENABLE");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.INVALID_SCRIPT)
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "CANNOT USE THE CURRENT SCRIPT ON THIS SCREEN");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.SCRIPT_ERROR)
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "SCRIPT FORMAT IS INVALID");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.AUT_QUIT_DURING_OP)
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "AUT QUIT DURING PLAYBACK OPERATION");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.INVALID_INDEX)
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "INPUT VALUE WAS OUT OF RANGE");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else if (ex.HResult == excode.OBJECT_NULL)
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + "CANNOT FOUND PLAYBACK SCRIPT");
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
                else
                {
                    ConsolePanelPush.AppendText(DateTime.Now + " - " + ex.HResult + " --- " + ex.Message);
                    ConsolePanelPush.AppendText(Environment.NewLine);
                }
            }
            WindowInteraction.FocusWindow(thisProc);
        }
        #endregion


        #region Code of Kiet
        /*private void btnAdd_Click(object sender, EventArgs e)
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

                // Add more actions here
                switch ((string)row.Cells[4].Value)
                {
                    case "Button":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsEnabled");
                        break;

                    case "RadioButton":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsChecked");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsUnChecked");
                        break;

                    case "TextBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsEmpty");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsEqual");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsReadOnly");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsEnabled");
                        break;

                    case "PasswordBox":
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

                    case "AutoCompleteCombobox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        break;

                    case "CheckBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Unselect");
                        break;

                    case "DataGrid":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Unselect");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("DoubleClick");
                        break;

                    case "TabItem":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsSelected");
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
        }*/
        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddTestSteps();
        }

        /// <summary>
        /// Add test steps to the Playback table
        /// </summary>
        private void AddTestSteps()
        {
            if (dataGridView1.SelectedCells.Count <= 0)
            {
                System.Windows.Forms.MessageBox.Show("There is no selected cell!", "Lack Of Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dataGridView2.AllowUserToAddRows = true;
            List<int> indexList = new List<int>();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                indexList.Add(Convert.ToInt32(dataGridView1.Rows[cell.RowIndex].Cells[1].Value));
            }

            var dict = new Dictionary<int, int>();
            foreach (var value in indexList)
            {
                if (dict.ContainsKey(value)) dict[value]++;
                else dict[value] = 1;
            }
            List<int> officialIndexList = new List<int>();
            foreach (var pair in dict)
            {
                officialIndexList.Add(pair.Key);
            }
            officialIndexList.Sort();

            int contIndex = dataGridView2.Rows.Count;
            for (int i = 0; i < officialIndexList.Count; i++)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[0].Clone();
                row.Cells[0].Value = contIndex;
                row.Cells[1].Value = SpyObjectList[officialIndexList[i]].index;
                row.Cells[2].Value = SpyObjectList[officialIndexList[i]].automationId;
                row.Cells[3].Value = SpyObjectList[officialIndexList[i]].name;
                row.Cells[4].Value = SpyObjectList[officialIndexList[i]].type;
                ((DataGridViewComboBoxCell)row.Cells[5]).Items.Clear();

                // Add more actions here
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
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsEmpty");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsEqual");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsReadOnly");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("IsEnabled");
                        break;

                    case "PasswordBox":
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

                    case "AutoCompleteCombobox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        break;

                    case "RichTextBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        break;

                    case "CheckBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Unselect");
                        break;

                    case "DataGrid":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Unselect");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("DoubleClick");
                        break;

                    case "TabItem":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        break;

                    default:
                        break;
                }
                dataGridView2.Rows.Add(row);
                contIndex++;
                row.Cells[0].ReadOnly = true;
                row.Cells[1].ReadOnly = true;
                row.Cells[2].ReadOnly = true;
                row.Cells[3].ReadOnly = true;
                row.Cells[4].ReadOnly = true;
            }
            dataGridView2.AllowUserToAddRows = false;
        }


        
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
                System.Windows.Forms.MessageBox.Show("There is no data!", "Lack Of Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you really want to remove this row?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    int i = 1;
                    foreach (DataGridViewRow row in dataGridView2.SelectedRows)
                        dataGridView2.Rows.Remove(row);
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        row.Cells[0].Value = i;
                        i++;
                    }
                }
                else if (dialogResult == DialogResult.No)
                    return;
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
                        if (SpyObjectList[i].type == "ComboBoxEdit" || SpyObjectList[i].type == "AutoCompleteCombobox")
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

                // Add more types here
                case "Interactive Controls":

                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if ((SpyObjectList[i].type == "ComboBox" || SpyObjectList[i].type == "ComboBoxEdit" ||
                            SpyObjectList[i].type == "DataGrid" || SpyObjectList[i].type == "TextBox" ||
                            SpyObjectList[i].type == "Button" || SpyObjectList[i].type == "RadioButton" ||
                            SpyObjectList[i].type == "AutoCompleteCombobox" || SpyObjectList[i].type == "TabItem" ||
                            SpyObjectList[i].type == "PasswordBox") && SpyObjectList[i].automationId.Contains("PART") != true)
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


        /// <summary>
        /// Open Inspector Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InspectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighlightForm HLightForm = new HighlightForm();
            HLightForm.ProcessName = ProcessForm.targetproc;
            HLightForm.Show();
        }

        private void btnCreateTestSteps_Click(object sender, EventArgs e)
        {
            if (ValidateJSON(rtxtScript.Text) == false)
            {
                System.Windows.Forms.MessageBox.Show("Test Script format is invalid Json!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CreateTestSteps();
        }


        /// <summary>
        /// Create a Playback table from JSON test script.
        /// </summary>
        private void CreateTestSteps()
        {
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
                ((DataGridViewComboBoxCell)row.Cells[5]).Items.Clear();
                switch (row.Cells[4].Value.ToString())
                {
                    case "TabItem":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        if (PlaybackObjectList[i].action == "Click")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                        break;

                    case "Button":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        if (PlaybackObjectList[i].action == "Click")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                        break;

                    case "RadioButton":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Click");
                        if (PlaybackObjectList[i].action == "Click")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                        break;

                    case "TextBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        if (PlaybackObjectList[i].action == "SetText")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
        
                        break;

                    case "RichTextBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        if (PlaybackObjectList[i].action == "SetText")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                        break;

                    case "PasswordBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SetText");
                        if (PlaybackObjectList[i].action == "SetText")
                            row.Cells[5].Value = PlaybackObjectList[i].action;
                          break;

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

                    case "CheckBox":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Unselect");
                        switch (PlaybackObjectList[i].action)
                        {
                            case "Select":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                                break;

                            case "Unselect":
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

                    case "AutoCompleteCombobox":
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

                    case "SendKeyorWaitEnable":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SendKey");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("WaitEnable");
                        switch (PlaybackObjectList[i].action)
                        {
                            case "SendKey":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                                break;

                            case "WaitEnable":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[1];
                                break;

                            default:
                                break;
                        }
                        break;

                    case "DataGrid":
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Select");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("Unselect");
                        ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("DoubleClick");
                        switch (PlaybackObjectList[i].action)
                        {
                            case "Select":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[0];
                                break;

                            case "Unselect":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[1];
                                break;

                            case "DoubleClick":
                                row.Cells[5].Value = ((DataGridViewComboBoxCell)row.Cells[5]).Items[2];
                                break;

                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }
                if (PlaybackObjectList[i].text != "" && PlaybackObjectList[i].itemIndex == -1)
                    row.Cells[6].Value = PlaybackObjectList[i].text.Trim();
                else if (PlaybackObjectList[i].text == "" && PlaybackObjectList[i].itemIndex >= 0)
                    row.Cells[6].Value = Convert.ToInt32(PlaybackObjectList[i].itemIndex);

                dataGridView2.Rows.Add(row);
                row.Cells[0].ReadOnly = true;
            }
            dataGridView2.AllowUserToAddRows = false;
        }

        //string JsonPath;
        private void btnCreateTestScript_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("There is no data!", "Lack Of Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

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
                if (PlaybackObjectList[pbindex].type.Trim() == "DataGrid"
                    || PlaybackObjectList[pbindex].type.Trim() == "ComboBox"
                    || PlaybackObjectList[pbindex].type.Trim() == "ComboBoxEdit"
                    || PlaybackObjectList[pbindex].type.Trim() == "AutoCompleteCombobox")
                {
                    if (isNumeric)
                    {
                        PlaybackObjectList[pbindex].text = "";
                        PlaybackObjectList[pbindex].itemIndex = Convert.ToInt32(row.Cells[6].Value);
                    }
                    else
                    {
                        PlaybackObjectList[pbindex].text = Convert.ToString(row.Cells[6].Value).Trim();
                        PlaybackObjectList[pbindex].itemIndex = -1;
                    }
                }
                else
                {
                    PlaybackObjectList[pbindex].text = Convert.ToString(row.Cells[6].Value).Trim();
                    PlaybackObjectList[pbindex].itemIndex = -1;
                }

                if (PlaybackObjectList[pbindex].type == "SendKeyorWaitEnable" && PlaybackObjectList[pbindex].action == "WaitEnable")
                {
                    if (string.IsNullOrEmpty(PlaybackObjectList[pbindex].text.Trim()))
                    {
                        System.Windows.Forms.MessageBox.Show("Dont Leave any blank");
                    }
                    else
                        PlaybackObjectList[pbindex].text = Convert.ToString(row.Cells[6].Value).Trim();
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
                    rtxtScript.Text += "},{\"Controller\":" + json + "}]";
                else
                    //System.IO.File.AppendAllText(JsonPath, "},{\"Controller\":" + json);
                    rtxtScript.Text += "},{\"Controller\":" + json;
                pbindex++;
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count <= 0)
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
            if (dataGridView2.Rows.Count <= 0)
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

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog ExportDialog = new System.Windows.Forms.SaveFileDialog();
            ExportDialog.Title = "Save Test Script File";
            ExportDialog.Filter = "JSON files (*.json)|*.json";
            ExportDialog.RestoreDirectory = true;

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
                    //clbTestScriptList.Items.Clear();
                    //lbxScriptList.SelectionMode = SelectionMode.MultiExtended;
                    //scriptFiles = new ScriptFile[ImportDialog.FileNames.Length];
                    int length = ImportDialog.FileNames.Length;

                    //System.Windows.Forms.MessageBox.Show(ImportDialog.SafeFileNames[0].ToString());
                    string content;
                    int j = 0;
                    int k = scriptFiles.Count;
                    //System.Windows.Forms.MessageBox.Show(length.ToString());
                    //System.Windows.Forms.MessageBox.Show(k.ToString());
                    while (j < length)
                    {
                        scriptFiles.Add(new ScriptFile() { Name = ImportDialog.SafeFileNames[j], Path = ImportDialog.FileNames[j] });
                        content = File.ReadAllText(scriptFiles[k].Path);
                        if (ValidateJSON(content) == false)
                        {
                            log.Error("SCRIPT FORMAT IS INVALID");
                            System.Windows.Forms.MessageBox.Show(scriptFiles[k].Name + " : Test Script's format is invalid Json", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            clbTestScriptList.Items.Add(scriptFiles[k].Name);
                        }
                        k++;
                        j++;
                    }
                }
                
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        /// <summary>
        /// The JSON test script validator
        /// </summary>
        public void readJson()
        {
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
                    //System.Windows.Forms.MessageBox.Show(PlaybackObjectList[pbindex].type.ToString());
                    pbindex++;
                }
            }
            catch (Exception ex)
            {
                ConsolePanelPush.AppendText("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
                ConsolePanelPush.AppendText(Environment.NewLine);
                WindowInteraction.FocusWindowNormal(thisProc);
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region HighLight Words
        /// <summary>
        /// Check a specific keyword and change its color.
        /// </summary>
        /// <param name="words">String</param>
        /// <param name="color"></param>
        /// <param name="StartIndex"></param>
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

            if (this.ConsolePanelPush.Text.Contains(word))
            {
                int index = -1;
                int selectStart = this.ConsolePanelPush.SelectionStart;
                while ((index = this.ConsolePanelPush.Text.IndexOf(word, (index + 1))) != -1)
                {
                    this.ConsolePanelPush.Select((index + StartIndex), word.Length);
                    this.ConsolePanelPush.SelectionColor = color;
                    this.ConsolePanelPush.Select(selectStart, 0);
                    this.ConsolePanelPush.SelectionColor = Color.Black;
                }
            }

            if (this.rtxtScript.Text.Contains(word))
            {
                int index = -1;
                int selectStart = this.rtxtScript.SelectionStart;
                while ((index = this.rtxtScript.Text.IndexOf(word, (index + 1))) != -1)
                {
                    this.rtxtScript.Select((index + StartIndex), word.Length);
                    this.rtxtScript.SelectionColor = color;
                    this.rtxtScript.Select(selectStart, 0);
                    this.rtxtScript.SelectionColor = Color.Black;
                }
            }
        }

        private void ResultPanelPush_TextChanged_1(object sender, EventArgs e)
        {
            this.CheckKeyWord("PASSED", Color.Green, 0);
            this.CheckKeyWord("FAILED", Color.Red, 0);
            this.CheckKeyWord("item id", Color.OrangeRed, 0);
            this.CheckKeyWord("ClassType", Color.Blue, 0);
            this.CheckKeyWord("PLAYBACK ON", Color.Purple, 0);
        }

        private void ConsolePanelPush_TextChanged(object sender, EventArgs e)
        {
            this.CheckKeyWord("SCENARIO", Color.Green, 0);
            this.CheckKeyWord("DONE PLAYBACK", Color.Green, 0);

            this.CheckKeyWord("AUT NOT FOUND", Color.Red, 0);
            this.CheckKeyWord("INPUT VALUE WAS OUT OF RANGE", Color.Red, 0);
            this.CheckKeyWord("AUT QUIT DURING PLAYBACK OPERATION", Color.Red, 0);
            this.CheckKeyWord("CANNOT SPY THIS PROGRAM", Color.Red, 0);
        }
        #endregion

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

        // Button Save jsonscript
        private void btnQuickSave_Click(object sender, EventArgs e)
        {
            if (clbTestScriptList.Items.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("There is no Test Script file!", "Lack Of Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (clbTestScriptList.SelectedItems.Count <= 0)
            {
                System.Windows.Forms.MessageBox.Show("There is no selected Test Script file!", "Lack Of Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (tctrlPlayback.SelectedTab == tpgPlaybackTable)
            {
                if (dataGridView2.Rows.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("There is no data!", "Lack Of Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int i = 0;
                foreach (var scriptFile in scriptFiles)
                {
                    if (scriptFile.Name == clbTestScriptList.SelectedItem.ToString())
                    {
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
                            if (PlaybackObjectList[pbindex].type.Trim() == "DataGrid"
                                || PlaybackObjectList[pbindex].type.Trim() == "ComboBox"
                                || PlaybackObjectList[pbindex].type.Trim() == "ComboBoxEdit"
                                || PlaybackObjectList[pbindex].type.Trim() == "AutoCompleteCombobox")
                            {
                                if (isNumeric)
                                {
                                    PlaybackObjectList[pbindex].itemIndex = Convert.ToInt32(row.Cells[6].Value);
                                    PlaybackObjectList[pbindex].text = "";
                                }
                                else
                                {
                                    PlaybackObjectList[pbindex].itemIndex = -1;
                                    PlaybackObjectList[pbindex].text = Convert.ToString(row.Cells[6].Value);
                                }
                            }
                            else
                            {
                                PlaybackObjectList[pbindex].text = Convert.ToString(row.Cells[6].Value);
                                PlaybackObjectList[pbindex].itemIndex = -1;
                            }

                            if (PlaybackObjectList[pbindex].type == "SendKeyorWaitEnable" && PlaybackObjectList[pbindex].action == "WaitEnable")
                            {
                                if (string.IsNullOrEmpty(PlaybackObjectList[pbindex].text))
                                {
                                    System.Windows.Forms.MessageBox.Show("Dont Leave any blank");
                                }
                                else
                                    PlaybackObjectList[pbindex].text = Convert.ToString(row.Cells[6].Value);
                                PlaybackObjectList[pbindex].itemIndex = -1;
                            }

                            string json = JsonConvert.SerializeObject(PlaybackObjectList[pbindex], Formatting.Indented);
                            if (dataGridView2.Rows.Count == 1)
                            {
                                System.IO.File.WriteAllText(scriptFiles[i].Path, "[{\"Controller\":" + json + "}]");
                                break;
                            }
                            if (pbindex == 0)
                                System.IO.File.WriteAllText(scriptFiles[i].Path, "[{\"Controller\":" + json);
                            else if (pbindex == dataGridView2.Rows.Count - 1)
                                System.IO.File.AppendAllText(scriptFiles[i].Path, "},{\"Controller\":" + json + "}]");
                            else
                                System.IO.File.AppendAllText(scriptFiles[i].Path, "},{\"Controller\":" + json);
                            pbindex++;
                        }
                        System.Windows.Forms.MessageBox.Show("Saved!", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    i++;
                }
            }
            else if (tctrlPlayback.SelectedTab == tpgPlaybackScript)
            {
                int i = 0;
                foreach (var scriptFile in scriptFiles)
                {
                    if (scriptFile.Name == clbTestScriptList.SelectedItem.ToString())
                    {
                        File.WriteAllText(scriptFiles[i].Path, rtxtScript.Text);
                        System.Windows.Forms.MessageBox.Show("Saved!", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    i++;
                }
            }
        }

        private void btnPlaybackScenario_Click(object sender, EventArgs e)
        {
            try
            {
                if (ProcessForm.isAttached == false)
                {
                    System.Windows.Forms.MessageBox.Show("Please attach AUT process to execute Scenario Playback function!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (clbTestScriptList.CheckedItems.Count <= 0)
                {
                    System.Windows.Forms.MessageBox.Show("There is no checked Test Script file!", "Lack Of Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<int> indexList = new List<int>();
                foreach (var item in clbTestScriptList.CheckedItems)
                {
                    indexList.Add(clbTestScriptList.Items.IndexOf(item));
                }

                ResultPanelPush.Clear();
                ConsolePanelPush.AppendText(DateTime.Now + " - " + "BEGIN SCENARIO PLAYBACK" + Environment.NewLine);
                log.Info("BEGIN SCENARIO PLAYBACK");

                ScenarioStatus = true;

                for (int i = 0; i < indexList.Count; i++)
                {
                    
                    clbTestScriptList.SelectedIndex = indexList[i];
                    //System.Windows.Forms.MessageBox.Show(rtxtScript.Text.ToString());
                    readJson();

                    ElementList = DoSpy.SearchbyFramework("WPF");
                    
                    Thread t1 = new Thread(() => ClearTextBox.ClearValue(ProcessForm.targetproc));
                    t1.Start();
                    t1.Join();
                    
                    ResultPanelPush.AppendText(DateTime.Now + " - " + "PLAYBACK ON: " + clbTestScriptList.SelectedItem + Environment.NewLine);
                    PlaybackTestScript();

                    ResultPanelPush.AppendText(Environment.NewLine);
                }
                ConsolePanelPush.AppendText(DateTime.Now + " - " + "DONE SCENARIO PLAYBACK" + Environment.NewLine);
                log.Info("DONE SCENARIO PLAYBACK");
                ScenarioStatus = false;
                WindowInteraction.FocusWindow(thisProc);
            }
            catch
            {
                var a = new Exception("NO AUT TO PLAYBACK");
                ConsolePanelPush.AppendText(DateTime.Now + " - " + a + Environment.NewLine);
            }
        }

        private void clbTestScriptList_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            clbTestScriptList.CheckOnClick = true;
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
                    CreateTestSteps();
                    //readJson();
                    break;
                }
                i++;
            }
        }


        private void btnSendKeyorWaitEnable_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count <= 0)
            {
                return;
            }

            DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[0].Clone();

            row.Cells[0].Value = dataGridView2.Rows.Count + 1;
            row.Cells[1].Value = -1;
            row.Cells[2].Value = "";
            row.Cells[3].Value = "";
            row.Cells[4].Value = "SendKeyorWaitEnable";
            ((DataGridViewComboBoxCell)row.Cells[5]).Items.Clear();
            ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("SendKey");
            ((DataGridViewComboBoxCell)row.Cells[5]).Items.Add("WaitEnable");
            row.Cells[6].Value = "";

            row.Cells[0].ReadOnly = true;
            row.Cells[1].ReadOnly = true;
            row.Cells[2].ReadOnly = true;
            row.Cells[3].ReadOnly = true;
            row.Cells[4].ReadOnly = true;

            dataGridView2.Rows.Add(row);
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow AboutW = new AboutWindow();
            AboutW.Show();
        }

        private void viewLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
    
        }


        /// <summary>
        /// Enable edited mode for selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView2.SelectedCells.Count > 0)
                {
                    int selectedrowindex = dataGridView2.SelectedCells[0].RowIndex;

                    DataGridViewRow selectedRow = dataGridView2.Rows[selectedrowindex];

                    dataGridView2.BeginEdit(true);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// PlaybackLogger
        /// </summary>
        /// <param name="index">Index of the UiElement</param>
        /// <param name="classname">Class of the UiElement</param>
        /// <param name="result">Pass of Failed</param>
        private void PlaybackLogger(int index, string classname, bool result)
        {
            if (result == true)
            {
                log.Info("PASSED at item id: " + index + " --- " + "ClassType: " + classname);
                ResultPanelPush.AppendText(DateTime.Now + " - " + "PASSED at item id: " + index + " --- " + "ClassType: " + classname + Environment.NewLine);
            }
            else
            {
                log.Info("FAILED at item id: " + index + " --- " + "ClassType: " + classname);
                ResultPanelPush.AppendText(DateTime.Now + " - " + "FAILED at item id: " + index + " --- " + "ClassType: " + classname + Environment.NewLine);
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddTestSteps();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F1:
                    button1.PerformClick();
                    break;
                case Keys.F2:
                    btnPlaybackTestSteps.PerformClick();
                    break;
                case Keys.F3:
                    btnPlaybackTestScript.PerformClick();
                    break;
                case Keys.F4:
                    btnPlaybackScenario.PerformClick();
                    break;
                case Keys.Escape:
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you really want to exit this program?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                    if (dialogResult == DialogResult.Yes)
                    {
                        // The user wants to exit the application. Close everything down.
                        System.Windows.Forms.Application.Exit();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                    break;
                case Keys.Control | Keys.Shift | Keys.S:
                    btnQuickSave.PerformClick();
                    break;
                case Keys.Control | Keys.S:
                    exportToolStripMenuItem.PerformClick();
                    break;
                case Keys.Alt | Keys.F4:
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Control | Keys.O:
                    importToolStripMenuItem.PerformClick();
                    break;
                default:
                    break;

            }
        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dataGridView2.Rows.Count <= 0)
                {
                    return;
                }
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you really want to delete this row?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    e.SuppressKeyPress = false;
                }
                else if (dialogResult == DialogResult.No)
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void clbTestScriptList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (clbTestScriptList.SelectedItems.Count <= 0)
                {
                    return;
                }
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you really want to delete this row?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    //foreach (var item in clbTestScriptList.SelectedItems)
                    //    clbTestScriptList.Items.Remove(item);
                    //scriptFiles.Clear();
                }
            }
        }


        /// <summary>
        /// View logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewLogsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var log = path + @"\Botsina\Logs\Botsina.log";

            Process.Start(log);
        }


        /// <summary>
        /// Tooltip for controls and buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region tooltip
        private void btnPlaybackTestSteps_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnPlaybackTestSteps, "Playback Test Steps");
        }
        private void btnCreateTestScript_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnCreateTestScript, "Create Test Script");
        }
        private void btnSendKeyorWaitEnable_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnSendKeyorWaitEnable, "Sendkey & WaitEnable");
        }
        private void btnRemove_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnRemove, "Remove Test Step");
        }
        private void btnMoveUp_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnMoveUp, "Move Up Test Step");
        }
        private void btnMoveDown_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnMoveDown, "Move Down Test Step");
        }

        private void btnAttach_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnAttach, "Attach an AUT");
        }
        private void button1_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(button1, "Spy");
        }
        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(btnAdd, "Add UI objects to Playback");
        }

        #endregion

        private void cbxProgressBar_CheckedChanged(object sender, EventArgs e)
        {
            if(cbxProgressBar.Checked.Equals(true))
            {
                Settings.ShowProgressBar = true;

            }
            else if(cbxProgressBar.Checked.Equals(false))
            {
                Settings.ShowProgressBar = false;
            }
        }
    }
}