using log4net;
using SpyandPlaybackTestTool.Ultils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool
{
    public partial class ProcessForm : Form
    {

        private static ProcessForm _instance;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string targetproc { get; set; }

        public static Process thisProc = Process.GetCurrentProcess();

        private Thread T1;

        public static bool isAttached = false;

        public static int processId = -1;
        private int index;
        private int selected_item;
        private int i;
        private Process[] ListProcess = Process.GetProcesses();

        public ProcessForm()
        {
            InitializeComponent();
        }

        public static ProcessForm GetInstance()
        {
            if (_instance == null || (_instance.IsDisposed))
            { 
                _instance = new ProcessForm();
            }
            return _instance;
        }

        public string getProcess()
        {
            try
            {
                //ListProcess = Process.GetProcesses();

                index = selected_item;


                targetproc = ListProcess[index].ProcessName;
                processId = ListProcess[index].Id;

                GrabAUT.GetMainWindow();

                T1 = new Thread(() =>
                {
                    MessageBox.Show("AUT: " + targetproc + " ATTACHED SUCCESSFUL", "NOTICED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    WindowInteraction.FocusWindowNormal(thisProc);
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
                foreach (ListViewItem item1 in listView1.Items)
                {
                    item1.BackColor = SystemColors.Window;
                    item1.ForeColor = SystemColors.WindowText;
                }

                var item = listView1.FindItemWithText(this.textBox1.Text);
                if (!item.Equals(null))
                {
                    int a = listView1.Items.IndexOf(item);


                    listView1.Items[a].Selected = true;
                    listView1.EnsureVisible(a);
                    listView1.Items[a].BackColor = SystemColors.Highlight;
                    listView1.Items[a].ForeColor = SystemColors.HighlightText;
                }
            }
            catch
            {

            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.BackColor = SystemColors.Window;
                    item.ForeColor = SystemColors.WindowText;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(this.textBox1.Text))
                    {
                        MessageBox.Show("Please Enter a process's name!!!");
                    }
                    else
                    {
                        var item = listView1.FindItemWithText(this.textBox1.Text);

                        selected_item = (int)item.Tag;

                        int a = listView1.Items.IndexOf(item);

                        listView1.Items[a].Selected = true;
                        listView1.Items[a].BackColor = SystemColors.Highlight;
                        listView1.Items[a].ForeColor = SystemColors.HighlightText;

                        index = selected_item;

                        DialogResult dr = MessageBox.Show("Do you want to attach: " + ListProcess[index].ProcessName, " WARNING!!!",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Information);

                        if (dr == DialogResult.OK)
                        {
                            targetproc = getProcess();
                            this.Close();
                        }
                        else
                        {

                        }


                        

                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProcessForm_Load(object sender, EventArgs e)
        {
            try
            {
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                ListProcess = Process.GetProcesses();

                for (i = 0; i < ListProcess.Count(); i++)
                {
      
                    if(!ListProcess[i].MainWindowTitle.Equals(""))
                    {
                        ListViewItem item = new ListViewItem(ListProcess[i].Id.ToString());
                        item.Tag = i;


                        item.SubItems.Add(ListProcess[i].ProcessName);
                        item.SubItems.Add(ListProcess[i].MainWindowTitle.ToString());
                        //item.SubItems.Add(item.Tag.ToString());
                        listView1.Items.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            try
            {
                listView1.Items.Clear();
                ListProcess = Process.GetProcesses();
               
                for (i = 0; i < ListProcess.Count(); i++)
                {
                    if (!ListProcess[i].MainWindowTitle.Equals(""))
                    {
                        ListViewItem item = new ListViewItem(ListProcess[i].Id.ToString());
                        item.Tag = i;
                        item.SubItems.Add(ListProcess[i].ProcessName);
                        item.SubItems.Add(ListProcess[i].MainWindowTitle.ToString());
                        listView1.Items.Add(item);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                selected_item = (int)listView1.SelectedItems[0].Tag;
                index = selected_item;

                DialogResult dr = MessageBox.Show("Do you want to attach: "+ ListProcess[index].ProcessName, " WARNING!!!",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Information);

                if (dr == DialogResult.OK)
                {
                    targetproc = getProcess();
                    this.Close();
                }
                else
                {

                }


            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        
        }

        private void listView1_Enter(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.BackColor = SystemColors.Window;
                item.ForeColor = SystemColors.WindowText;
            }
        }


        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void ProcessForm_Activated(object sender, EventArgs e)
        {
            try
            {
                listView1.Items.Clear();

                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                ListProcess = Process.GetProcesses();

                for (i = 0; i < ListProcess.Count(); i++)
                {

                    if (!ListProcess[i].MainWindowTitle.Equals(""))
                    {
                        ListViewItem item = new ListViewItem(ListProcess[i].Id.ToString());
                        item.Tag = i;


                        item.SubItems.Add(ListProcess[i].ProcessName);
                        item.SubItems.Add(ListProcess[i].MainWindowTitle.ToString());
                        //item.SubItems.Add(item.Tag.ToString());
                        listView1.Items.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}