using JJMedia5.Server.backend.Context.Entity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace JJMedia5.Server.backend.Context {

    public class JJMedia5Context : DbContext {

        public JJMedia5Context([NotNull] DbContextOptions options) : base(options) {
        }

        public DbSet<RssFeed> RssFeeds { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TorrentClient> TorrentClients { get; set; }
    }
}