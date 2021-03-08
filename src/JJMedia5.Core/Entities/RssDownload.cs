using JJMedia5.Core.Attributes;
using System;

namespace JJMedia5.Core.Entities {

    [Repository(TableName = "RssDownload")]
    public class RssDownload : Entity {
        public string Hash { get; set; }

        public int RssFeedId { get; set; }

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;

        public override object GetPropertyModel()
            => new {
                Hash,
                RssFeedId,
                CreatedOn
            };
    }
}