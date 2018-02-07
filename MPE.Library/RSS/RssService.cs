using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace MPE.Library.RSS
{
    public class RssService
    {
        private const string FeedGeneratorName = "MPE RSS Feed Generator v1.0";
        private const string FeedCopyrightText = "MPE v1.0 Copyright";

        private const string TextHtmlMediaType = "text/html";
        private const string SelfRelationshipType = "self";
        private const string AlternateRelationshipType = "alternate";

        public string GenerateXml(RssConfiguration configuration)
        {
            var feed = new SyndicationFeed();
            feed.Generator = FeedGeneratorName;
            feed.Copyright = new TextSyndicationContent(FeedCopyrightText);

            feed.Id = configuration.BaseUrl;
            feed.Title = new TextSyndicationContent(configuration.Title);
            feed.Description = new TextSyndicationContent(configuration.Description);
            feed.LastUpdatedTime = new DateTimeOffset(DateTime.Now);

            if (!string.IsNullOrEmpty(configuration.LogoRelativeUrl))
            {
                feed.ImageUrl = new Uri(Path.Combine(configuration.BaseUrl, configuration.LogoRelativeUrl));
            }

            feed.Links.Add(GenerateLink(configuration.Title, Path.Combine(configuration.BaseUrl, configuration.FeedRelativeUrl), TextHtmlMediaType, SelfRelationshipType));
            feed.Links.Add(GenerateLink(configuration.BaseName, configuration.BaseUrl, TextHtmlMediaType, AlternateRelationshipType));

            var items = new List<SyndicationItem>();
            configuration.Items.ForEach(x => items.Add(GenerateItem(configuration.BaseUrl, x)));
            feed.Items = items;

            return WriteToString(feed);
        }

        private SyndicationLink GenerateLink(
            string title, 
            string url, 
            string mediaType, 
            string relationshipType = null)
        {
            var link = new SyndicationLink
            {
                Title = title,
                Uri = new Uri(url),
                MediaType = mediaType,
                Length = 1000
            };

            if (!string.IsNullOrEmpty(relationshipType))
            {
                link.RelationshipType = relationshipType;
            }

            return link;
        }

        private SyndicationItem GenerateItem(
            string baseUrl,
            RssItem rssItem)
        {
            var item = new SyndicationItem();
            var url = Path.Combine(baseUrl, rssItem.RelativeUrl);
            item.Id = url;
            item.Links.Add(new SyndicationLink(new Uri(url)));
            item.Title = new TextSyndicationContent(rssItem.Title);
            item.LastUpdatedTime = rssItem.LastUpdated;
            item.PublishDate = rssItem.Published;
            item.Content = new TextSyndicationContent(rssItem.Content, TextSyndicationContentKind.Html);

            return item;
        }

        private string WriteToString(SyndicationFeed feed)
        {
            var rssFormatter = new Rss20FeedFormatter(feed);
            var output = new StringBuilder();
            using (var writer = XmlWriter.Create(output, new XmlWriterSettings { Indent = true }))
            {
                rssFormatter.WriteTo(writer);
                writer.Flush();
                return output.ToString();
            }
        }
    }
}
