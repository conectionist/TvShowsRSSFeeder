using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace TvShowsRSSFeeder
{
    public partial class DownloadWindow : Form
    {
        List<TvTorrentsEpisodeInfo> episodeList;
        public DownloadWindow(List<TvTorrentsEpisodeInfo> _episodeList)
        {
            episodeList = _episodeList;

            Point beginningPoint = new Point(20, 20);
            int i = 0;
            foreach (TvTorrentsEpisodeInfo ep in episodeList)
            {
                Label l = new Label();
                l.Size = new Size(400, l.Size.Height);
                l.Location = new Point(beginningPoint.X, beginningPoint.Y + 35 * i);
                l.Text = ep.title;

                MyButton b = new MyButton();
                b.Location = new Point(beginningPoint.X + l.Size.Width + 10, beginningPoint.Y + 35 * i - 5);
                b.Text = "Download";
                b.episodeTitle = ep.title;
                b.episodeDownloadLink = ep.link;

                b.Click += b_Click;

                Controls.Add(l);
                Controls.Add(b);

                i++;
            }
            InitializeComponent();
        }

        void b_Click(object sender, EventArgs e)
        {
            MyButton b = sender as MyButton;
            
            string fileName = b.episodeTitle + ".torrent";

            DownloadFile(b.episodeDownloadLink, fileName);
            AddToUtorrent(fileName);

            //System.Diagnostics.Process.Start(b.episodeDownloadLink);
            
        }

        private void DownloadWindow_Load(object sender, EventArgs e)
        {
            this.Size = new Size(this.Size.Width, this.Size.Height + episodeList.Count * 35);
            this.CenterToScreen();
        }

        private void DownloadFile(string url, string file)
        {
            byte[] result;
            byte[] buffer = new byte[4096];

            WebRequest wr = WebRequest.Create(url);
            wr.ContentType = "application/x-bittorrent";
            using (WebResponse response = wr.GetResponse())
            {
                bool gzip = response.Headers["Content-Encoding"] == "gzip";
                var responseStream = gzip
                                        ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress)
                                        : response.GetResponseStream();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, count);
                    } while (count != 0);

                    result = memoryStream.ToArray();

                    using (BinaryWriter writer = new BinaryWriter(new FileStream(file, FileMode.Create)))
                    {
                        writer.Write(result);
                    }
                }
            }
        }
        
        private void AddToUtorrent(string fileName)
        {
            System.Diagnostics.Process.Start(fileName);
        }
    }
}
