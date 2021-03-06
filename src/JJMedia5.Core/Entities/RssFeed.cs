using JJMedia5.Core.Attributes;
using System;

namespace JJMedia5.Core.Entities {

    [Repository(TableName = "RssFeeds")]
    public class RssFeed : Entity {
        public string Url { get; set; }

        public string Info { get; set; }

        public bool IsSubscribed { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset CreatedDate { get; } = DateTime.UtcNow;

        public string XmlXPathLink { get; set; } = string.Empty;

        public string RegexMatch { get; set; } = string.Empty;

        public override object GetPropertyModel()
            => new {
                Url,
                Info,
                IsSubscribed,
                StartDate,
                CreatedDate,
                XmlXPathLink,
                RegexMatch
            };
    }
}