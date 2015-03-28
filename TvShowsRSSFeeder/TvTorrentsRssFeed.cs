using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace TvShowsRSSFeeder
{
    class TvTorrentsRssFeed
    {
        private string rssFeedString;
        private Dictionary<string, TvTorrentsEpisodeInfo> rssFeedsList;
        public Dictionary<string, TvTorrentsEpisodeInfo> RssFeedsList
        {
            get
            {
                return rssFeedsList;
            }
        }
        public TvTorrentsEpisodeInfo this[string title]
        {
            get
            {
                return rssFeedsList[title];
            }
        }
        
        public TvTorrentsRssFeed(string _rssFeedString)
        {
            rssFeedsList = new Dictionary<string, TvTorrentsEpisodeInfo>();
            rssFeedString = _rssFeedString;
        }

        public void UpdateRssFeeds()
        {
            WebClient client = new WebClient();
            try
            {
                client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0");
                string downloadString = client.DownloadString(rssFeedString);

                XmlDocument doc = new XmlDocument();
                doc.Load(new StringReader(downloadString));

                XmlNodeList episodes = doc.SelectNodes("rss/channel/item");
                XmlNode episodeTitle;
                XmlNode episodeCategory;
                XmlNode episodePubdate;
                XmlNode episodeLink;
                XmlNode episodeDescription;

                rssFeedsList.Clear();

                for (int i = 0; i < episodes.Count; i++)
                {
                    episodeTitle = episodes.Item(i).SelectSingleNode("title");
                    episodeCategory = episodes.Item(i).SelectSingleNode("category");
                    episodePubdate = episodes.Item(i).SelectSingleNode("pubDate");
                    episodeLink = episodes.Item(i).SelectSingleNode("link");
                    episodeDescription = episodes.Item(i).SelectSingleNode("description");

                    rssFeedsList.Add(episodeTitle.InnerText, new TvTorrentsEpisodeInfo(episodeTitle.InnerText,
                                                                                       episodeCategory.InnerText,
                                                                                       episodePubdate.InnerText,
                                                                                       episodeLink.InnerText,
                                                                                       episodeDescription.InnerText));
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("There seems to be an issue with the following RSS feed:\n" + rssFeedString + "\n\nPlease change it!\n\nError: " + ex.Message);
            }
            
        }

        public List<TvTorrentsEpisodeInfo> GetLatestEpisodes(int noOlderThanNDays)
        {
            List<TvTorrentsEpisodeInfo> episodes = new List<TvTorrentsEpisodeInfo>();

            foreach(KeyValuePair<string, TvTorrentsEpisodeInfo> pair in rssFeedsList)
            {
                if (IsAtMostNDaysOld(DateTime.Parse(pair.Value.pubdate), noOlderThanNDays))
                {
                    episodes.Add(pair.Value);
                }
            }

            return episodes;
        }

        private bool IsAtMostNDaysOld(DateTime dateTime, int noOlderThanNDays)
        {
            TimeSpan ts = DateTime.Now - dateTime;

            return (ts.TotalDays <= noOlderThanNDays);
        }
    }
}
