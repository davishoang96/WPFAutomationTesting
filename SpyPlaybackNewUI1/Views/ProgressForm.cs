using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                while (Form1.playbackstatus.Equals(true))
                {
                    progressBar1.Value = 0;
                    break;
                }

                progressBar1.Value = Form1.playbackprogress;

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

                if (progressBar1.Value >= 100)
                {

                    timer1.Stop();

                    //progressBar1.Value = 100;

                    while (Form1.playbackprogress >= 1)
                    {
                        timer1.Start();
                        break;
                    }
                }
            } catch
            {
                //throw new Exception("Progress Bar's value is higher than 100 percent");
            }
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(workingArea.Right - Size.Width,
                          workingArea.Bottom - Size.Height);
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.stop_playback = true;
        }

        public void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
