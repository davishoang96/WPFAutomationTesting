using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using SpyandPlaybackTestTool.Ultils;
using static SpyandPlaybackTestTool.Ultils.Message;

namespace SpyandPlaybackTestTool
{
    public partial class ProcessForm : Form
    {
        public string ProcessName { get; set; }
        public static int processId = -1;
        int index = -1;
        public event EventHandler OnDataAvailable;
  
        public ProcessForm()
        {
         
            Process[] ListProcess = Process.GetProcesses();
            InitializeComponent();
            label2.Hide();
            listBox1.SelectionMode = SelectionMode.One;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            for (int i = 0; i < ListProcess.Count(); i++)
            {
                listBox1.Items.Add(ListProcess[i].ProcessName);
            }
        }
       
        public string getProcess()
        {
            Process[] ListProcess = Process.GetProcesses();
            index = listBox1.SelectedIndex;
            ProcessName = ListProcess[index].ProcessName;
            processId = ListProcess[index].Id;
            label2.Text = ListProcess[index].ProcessName + " is attached successfully!";
            label2.Show();
            if (OnDataAvailable != null)
            {
                OnDataAvailable(this, EventArgs.Empty);
            }
                
            return ListProcess[index].ProcessName;

        }
            
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int index = listBox1.FindString(this.textBox1.Text);
            if(0<=index)
            {
                listBox1.SelectedIndex = index;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
            if(e.KeyCode == Keys.Enter)
            {
                getProcess();
                this.Close();
                PublicMembers.theMessage = getProcess();
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem;

            PublicMembers.theMessage = getProcess();
        }
    }
}
