using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool.Views
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd,
        uint Msg,
        uint wParam,
        uint lParam);

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        
            progressBar1.Value = Form1.playbackprogress;

            if(Form1.playbackstatus.Equals(false) && Form1.playbacksuccess.Equals(true))
            {
                progressBar1.Value = 100;
                this.TopMost = false;
            }

            #region Draw % text
            int percent = (int)(((double)(progressBar1.Value - progressBar1.Minimum) /
            (double)(progressBar1.Maximum - progressBar1.Minimum)) * 100);
            using (Graphics gr = progressBar1.CreateGraphics())
            {
                gr.DrawString(percent.ToString() + "%",
                    SystemFonts.DefaultFont,
                    Brushes.Black,
                    new PointF(progressBar1.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Width / 2.0F),
                    progressBar1.Height / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Height / 2.0F)));
            }
            #endregion
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            //timer1.Start();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(workingArea.Right - Size.Width,
                          workingArea.Bottom - Size.Height);
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //timer1.Stop();
            progressBar1.Value = 0;
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.stop_playback = true;
        }

        private void ProgressForm_Shown(object sender, EventArgs e)
        {
        }

        private void ProgressForm_Activated(object sender, EventArgs e)
        {
            //timer1.Start();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(workingArea.Right - Size.Width,
                          workingArea.Bottom - Size.Height);

        }
    }
}
