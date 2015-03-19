using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowsRSSFeeder
{
    public class TvTorrentsEpisodeInfo
    {
        public string title;
        public string category;
        public string pubdate;
        public string link;
        public string description;

        public TvTorrentsEpisodeInfo(string _title, string _category, string _pubdate, string _link, string _description)
        {
            title       = _title;
            category    = _category;
            pubdate     = _pubdate;
            link        = _link;
            description = _description;
        }
    }
}
