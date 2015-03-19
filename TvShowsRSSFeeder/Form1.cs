using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Xml;

namespace TvShowsRSSFeeder
{
    public partial class Form1 : Form
    {
        private bool itsQuittingTime;
        private bool rssFeedHandlerIsDone = false;
        private TvTorrentsRssFeed tvtRssFeed;
        private List<TvTorrentsEpisodeInfo> newEpisodeList;
        private Size originalWindowSize;

        private int episodeMaxAgeInDays;

        public Form1()
        {
            InitializeComponent();

            tvtRssFeed = new TvTorrentsRssFeed("http://freshon.tv/rss.php?feed=dl&c[]=545&c[]=460&c[]=589&c[]=774&c[]=463&c[]=82&passkey=f8553887017f51f0739291799a0973c7");
            
            itsQuittingTime = false;
            SetMaxAge();

            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Text = "No new episodes, yet!";

            backgroundRSSHandler.RunWorkerAsync();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left)
            //{

            //}
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            itsQuittingTime = true;
            backgroundCleanupWorker.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            originalWindowSize = this.Size;
            backgroundWindowAnimator.RunWorkerAsync();
        }

        private void backgroundRSSHandler_DoWork(object sender, DoWorkEventArgs e)
        {
            int secondsCounter = 0;

            while (!itsQuittingTime)
            {
                // get rss feed content every minute
                if (secondsCounter % 60 == 0)
                {
                    tvtRssFeed.UpdateRssFeeds();
                    newEpisodeList = tvtRssFeed.GetLatestEpisodes(episodeMaxAgeInDays);

                    if (ThereAreUnseenEpisodes(newEpisodeList) && newEpisodeList.Count != 0)
                    {
                        string newEpisodes = "";

                        foreach (TvTorrentsEpisodeInfo ep in newEpisodeList)
                        {
                            newEpisodes += ep.title + "\n";
                        }
                        notifyIcon1.ShowBalloonTip(5000, "New episode(s) available", newEpisodes, ToolTipIcon.Info);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }

                secondsCounter++;
            }

            rssFeedHandlerIsDone = true;
        }

        private void backgroundRSSHandler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void backgroundCleanupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!rssFeedHandlerIsDone)
                Thread.Sleep(100);
        }

        private void backgroundCleanupWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Exit();
        }

        private void backgroundWindowAnimator_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            for (int i = 0; i < 15; i++)
            {
                bw.ReportProgress(i);

                Thread.Sleep(20);
            }
        }

        private void backgroundWindowAnimator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Location = new Point(Location.X + 120, Location.Y + 65);
            this.Size = new Size(Size.Width - 10, Size.Height - 10);
        }

        private void backgroundWindowAnimator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Hide();
            this.Size = originalWindowSize;
            CenterToScreen();
        }

        private bool ThereAreUnseenEpisodes(List<TvTorrentsEpisodeInfo> episodeList)
        {
            return true;
        }

        private void SetMaxAge()
        {
            episodeMaxAgeInDays = 7;
            try
            {
                episodeMaxAgeInDays = int.Parse(textBoxMaxAge.Text);
            }
            catch (FormatException)
            {
                //MessageBox.Show("The max age is invalid. Please open the settings dialog and enter a valid age in days");
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            DownloadWindow dlWnd = new DownloadWindow(newEpisodeList);
            dlWnd.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetMaxAge();
            Close();
        }
    }
}
