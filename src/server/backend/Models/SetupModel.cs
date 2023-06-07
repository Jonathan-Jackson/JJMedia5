using JJMedia5.Server.backend.Context.Entity;
using System;

namespace JJMedia5.Server.backend.Models {

    public class SetupModel {
        public string Username { get; set; }

        public RssFeed[] Rss { get; set; } = Array.Empty<RssFeed>();

        public TorrentClient TorrentClient { get; set; }

        public Import[] Imports { get; set; } = Array.Empty<Import>();

        public Store[] Stores { get; set; } = Array.Empty<Store>();
    }
}