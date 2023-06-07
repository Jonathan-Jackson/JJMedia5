using System;

namespace JJMedia5.Server.backend.Context.Entity {

    public class RssFeed {
        public int Id { get; set; }

        public string Url { get; set; }

        public string RegexMatch { get; set; }

        public bool IsSubscribed { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}