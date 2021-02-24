using System;

namespace JJMedia5.Core.Entities {

    public class RssFeed : Entity {
        public string Url { get; set; }

        public string Info { get; set; }

        public bool IsSubscribed { get; set; }

        public DateTimeOffset CreatedOn { get; } = DateTime.UtcNow;
    }
}