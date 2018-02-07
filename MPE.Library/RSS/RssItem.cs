using System;

namespace MPE.Library.RSS
{
    public class RssItem
    {
        public string RelativeUrl { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Published { get; set; }
        public string Content { get; set; }
    }
}