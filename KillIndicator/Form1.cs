using System;
using System.IO;
using System.Windows.Forms;

namespace KillIndicator
{
    public partial class Form1 : Form
    {
        public string pathToHighlights = Properties.Settings.Default.pathToHighlights;
        public string pathToSound = Properties.Settings.Default.pathToSound;
        public bool running = false;

        public Form1()
        {
            InitializeComponent();
            textBoxHighlightsPath.Text = pathToHighlights;
            textBoxSoundPath.Text = pathToSound;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBoxHighlightsPath.Text = fbd.SelectedPath.ToString();
                    pathToHighlights = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.pathToHighlights = fbd.SelectedPath.ToString();
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void btnBrowseSound_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    textBoxSoundPath.Text = ofd.FileName;
                    pathToSound = ofd.FileName;
                    Properties.Settings.Default.pathToSound = ofd.FileName;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            ToggleRun();
        }

        private void ToggleRun()
        {
            if (running)
            {
                running = false;
                buttonRun.Text = "RUN";
                buttonRun.ForeColor = System.Drawing.Color.Black;
                btnBrowseSound.Enabled = true;
                button1.Enabled = true;
                textBoxHighlightsPath.Enabled = true;
                textBoxSoundPath.Enabled = true;
                pictureBoxCheeki.Visible = false;
                notifyIcon1.Text = "Tarkov Kill Indicator - STOPPED";
                if(backgroundWorker1.IsBusy)
                backgroundWorker1.Dispose();
            }
            else
            {
                running = true;
                buttonRun.Text = "STOP";
                buttonRun.ForeColor = System.Drawing.Color.Red;
                backgroundWorker1.RunWorkerAsync();
                button1.Enabled = false;
                btnBrowseSound.Enabled = false;
                textBoxHighlightsPath.Enabled = false;
                textBoxSoundPath.Enabled = false;
                pictureBoxCheeki.Visible = true;
                notifyIcon1.Text = "Tarkov Kill Indicator - RUNNING";
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (Directory.Exists(pathToHighlights))
            {
                DetectKills();
            }
            else
            {
                MessageBox.Show("Please select a valid path to Nvidia Highlights temp directory.\n\n");
            }
        }

        private void DetectKills()
        {
            int newCount = 0;
            int oldCount = 0;
            while (running)
            {
                try
                {
                    newCount = Directory.GetFiles(pathToHighlights).Length;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Please select a valid path to Nvidia Highlights temp directory.\n\n" + e.Message);
                    break;
                }
                if (oldCount < newCount)
                {
                    oldCount = newCount;
                    PlaySound();
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        private void PlaySound()
        {
            try
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(pathToSound);
                player.Play();
            }
            catch(Exception e)
            {
                MessageBox.Show("Could not play selected sound file.\n\n" + e.Message);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
    }
}
