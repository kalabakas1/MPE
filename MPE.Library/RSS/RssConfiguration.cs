using System;
using System.Collections.Generic;

namespace MPE.Library.RSS
{
    public class RssConfiguration
    {
        public string BaseName { get; set; }
        public string BaseUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LogoRelativeUrl { get; set; }
        public string FeedRelativeUrl { get; set; }
        public List<RssItem> Items { get; set; }

    }
}
