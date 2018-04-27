using Gu.Wpf.UiAutomation;
using log4net;
using SpyandPlaybackTestTool.SpyPlaybackObjects;
using SpyandPlaybackTestTool.Ultils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool
{
    public partial class HighlightForm : Form
    {
        private static HighlightForm _instance;

        public string ProcessName { get; set; }//nhớ sửa lại bỏ vào AUT path
        private Gu.Wpf.UiAutomation.Application App;
        private IReadOnlyList<UiElement> ElementList;
        private Gu.Wpf.UiAutomation.Window MainWindow;

        private SpyObject[] SpyObjectList;

        private Users theUser = new Users();
        private Process thisProc = Process.GetCurrentProcess();

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Process AUTPROC;
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

        public HighlightForm()
        {
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            
        }

        public static HighlightForm GetInstance()
        {
            if (_instance == null || (_instance.IsDisposed))
            {
                _instance = new HighlightForm();
            }
            return _instance;
        }

        public void spy()
        {
            var curtime = DateTime.Now;

            try
            {
                log.Info("BEGIN SPY FROM HIGHLIGHT FORM");
                Process targetProcess = WindowInteraction.GetProcess(ProcessName);
                //WindowInteraction.FocusWindow(targetProcess);
                App = Gu.Wpf.UiAutomation.Application.Attach(targetProcess.Id);
                MainWindow = App.MainWindow;
                ElementList = MainWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.FrameworkIdProperty, "WPF"));
                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = true;

                SpyObjectList = new SpyObject[ElementList.Count];
                int SpyObjectIndex = 0;

                //comboBox1.Enabled = false;

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
                    row.Cells[0].Value = SpyObjectList[SpyObjectIndex].index;
                    row.Cells[1].Value = SpyObjectList[SpyObjectIndex].automationId;
                    row.Cells[2].Value = SpyObjectList[SpyObjectIndex].name;
                    row.Cells[3].Value = SpyObjectList[SpyObjectIndex].type;
                    dataGridView1.Rows.Add(row);
                    SpyObjectIndex++;
                }
                log.Info("SPY DONE OF HIGHLIGHT");

                dataGridView1.AllowUserToAddRows = false;

                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                log.Error("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
            }
        }

        public void highlight(int objectID)
        {
            var curtime = DateTime.Now;

            try
            {
                log.Info("BEGIN HIGHLIGHT");
                Process targetProcess = WindowInteraction.GetProcess(ProcessName);
                GrabAUT.GetMainWindow();
                ElementList = GrabAUT.SearchbyFramework("WPF");

                SpyObjectList = new SpyObject[ElementList.Count];
                int SpyObjectIndex = 0;
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

                    if (SpyObjectList[SpyObjectIndex].index == objectID)
                    {
                        for (int a = 0; a < 3; a++)
                        {
                            ElementList[SpyObjectList[SpyObjectIndex].index].DrawHighlight(true, Color.Red, TimeSpan.FromSeconds(1));
                        }
                    }

                    SpyObjectIndex++;
                }
                log.Info("DONE HIGHLIGHT");
            }
            catch (Exception ex)
            {
                log.Error("ERROR CODE: " + ex.HResult + "  -----  " + "detail: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedCells.Count > 0)
                {
                    int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                    var objectID = Convert.ToInt32(selectedRow.Cells["Index"].Value);

                    Thread t1 = new Thread(delegate ()
                    {
                        this.Hide();
                        highlight(objectID);
                        this.Show();
                    });
                    t1.Start();
                    //captureIt();
                    t1.Join();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void captureIt()
        {
            var curtime = DateTime.Now.Second;

            this.Hide();

            System.Threading.Thread.Sleep(2000);

            SendKeys.Send("{PRTSC}");

            this.Show();

            Image img = Clipboard.GetImage();

            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var imagePath = path + @"\Botsina\";

            img.Save(imagePath + curtime + ".png");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //textBox1.Text = "";
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
                            row.Cells[0].Value = SpyObjectList[i].index;
                            row.Cells[1].Value = SpyObjectList[i].automationId;
                            row.Cells[2].Value = SpyObjectList[i].name;
                            row.Cells[3].Value = SpyObjectList[i].type;
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
                            row.Cells[0].Value = SpyObjectList[i].index;
                            row.Cells[1].Value = SpyObjectList[i].automationId;
                            row.Cells[2].Value = SpyObjectList[i].name;
                            row.Cells[3].Value = SpyObjectList[i].type;
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
                            row.Cells[0].Value = SpyObjectList[i].index;
                            row.Cells[1].Value = SpyObjectList[i].automationId;
                            row.Cells[2].Value = SpyObjectList[i].name;
                            row.Cells[3].Value = SpyObjectList[i].type;
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
                            row.Cells[0].Value = SpyObjectList[i].index;
                            row.Cells[1].Value = SpyObjectList[i].automationId;
                            row.Cells[2].Value = SpyObjectList[i].name;
                            row.Cells[3].Value = SpyObjectList[i].type;
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
                            row.Cells[0].Value = SpyObjectList[i].index;
                            row.Cells[1].Value = SpyObjectList[i].automationId;
                            row.Cells[2].Value = SpyObjectList[i].name;
                            row.Cells[3].Value = SpyObjectList[i].type;
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
                            row.Cells[0].Value = SpyObjectList[i].index;
                            row.Cells[1].Value = SpyObjectList[i].automationId;
                            row.Cells[2].Value = SpyObjectList[i].name;
                            row.Cells[3].Value = SpyObjectList[i].type;
                        }
                    }
                    break;

                case "Interactive Controls":

                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        if ((SpyObjectList[i].type == "ComboBox" || SpyObjectList[i].type == "ComboBoxEdit" || SpyObjectList[i].type == "DataGrid" || SpyObjectList[i].type == "TextBox" || SpyObjectList[i].type == "Button" || SpyObjectList[i].type == "RadioButton" || SpyObjectList[i].type == "AutoCompleteCombobox") && SpyObjectList[i].automationId.Contains("PART") != true)
                        {
                            int rowId = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowId];
                            row.Cells[0].Value = SpyObjectList[i].index;
                            row.Cells[1].Value = SpyObjectList[i].automationId;
                            row.Cells[2].Value = SpyObjectList[i].name;
                            row.Cells[3].Value = SpyObjectList[i].type;
                        }
                    }
                    break;

                case "All":
                    for (int i = 0; i < SpyObjectList.Count(); i++)
                    {
                        int rowId = dataGridView1.Rows.Add();
                        DataGridViewRow row = dataGridView1.Rows[rowId];
                        row.Cells[0].Value = SpyObjectList[i].index;
                        row.Cells[1].Value = SpyObjectList[i].automationId;
                        row.Cells[2].Value = SpyObjectList[i].name;
                        row.Cells[3].Value = SpyObjectList[i].type;
                    }
                    break;

                default:
                    break;
            }
            dataGridView1.AllowUserToAddRows = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            CaptureToImage.DoCapture(ProcessName);
            this.Show();
        }

        private void HighlightForm_Load(object sender, EventArgs e)
        {
            spy();
        }

        private void HighlightForm_Activated(object sender, EventArgs e)
        {
            if (ProcessForm.isAttached.Equals(false))
            {
                ProcessForm.targetproc = null;
                comboBox1.Enabled = false;
                dataGridView1.Visible = false;
                AUTPROC = null;
                this.Close();
                WindowInteraction.FocusWindowNormal(thisProc);
            }
            else if (ProcessForm.isAttached.Equals(true))
            {
                AUTPROC = WindowInteraction.GetProcess(ProcessForm.targetproc);
                comboBox1.Enabled = true;
                dataGridView1.Visible = true;
                
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (AUTPROC.HasExited)
            {
                timer1.Stop();
                comboBox1.Enabled = false;
                dataGridView1.Visible = false;
                ProcessForm.targetproc = null;
                ProcessForm.isAttached = false;
                AUTPROC = null;
            }

        }
    }
}