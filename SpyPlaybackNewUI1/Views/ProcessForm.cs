using log4net;
using SpyandPlaybackTestTool.Ultils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool
{
    public partial class ProcessForm : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string targetproc { get; set; }

        public static Process thisProc = Process.GetCurrentProcess();

        public static bool isAttached;

        public static int processId = -1;
        private int index = -1;

        public ProcessForm()
        {
            InitializeComponent();
        }

        public string getProcess()
        {
            try
            {
                Process[] ListProcess = Process.GetProcesses();
                index = listBox1.SelectedIndex;
                targetproc = ListProcess[index].ProcessName;
                processId = ListProcess[index].Id;

                DoSpy.GetMainWindow();

                Thread T1 = new Thread(() =>
                {
                    MessageBox.Show("AUT: " + targetproc + " ATTACHED SUCCESSFUL", "NOTICED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    WindowInteraction.FocusWindow(thisProc);
                });

                T1.Start();
                log.Info("ATTACHED PROCESS: " + ListProcess[index].ProcessName);
                isAttached = true;
                return ListProcess[index].ProcessName;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int index = listBox1.FindString(this.textBox1.Text);
                if (0 <= index)
                {
                    listBox1.SelectedIndex = index;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(this.textBox1.Text))
                {
                    System.Windows.Forms.MessageBox.Show("Please Enter a process's name!!!");
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to attach this process?", "Warning!!!", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        targetproc = getProcess();

                        this.Close();
                    }
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to attach this process?", "Warning!!!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    targetproc = getProcess();

                    this.Close();
                }
            }
        }

        private void ProcessForm_Load(object sender, EventArgs e)
        {
            try
            {
                Process[] ListProcess = Process.GetProcesses();
                listBox1.SelectionMode = SelectionMode.One;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                for (int i = 0; i < ListProcess.Count(); i++)
                {
                    listBox1.Items.Add(ListProcess[i].ProcessName);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                Process[] ListProcess = Process.GetProcesses();
                listBox1.SelectionMode = SelectionMode.One;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                for (int i = 0; i < ListProcess.Count(); i++)
                {
                    listBox1.Items.Add(ListProcess[i].ProcessName);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to attach this process?", "Warning!!!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var item = listBox1.SelectedItem;

                targetproc = getProcess();

                this.Close();
            }
            else if (dialogResult == DialogResult.No)
            {
            }
        }
    }
}