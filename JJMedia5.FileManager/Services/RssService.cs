using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace JJMedia5.FileManager.Services {

    public class RssService {
        private readonly IRepository<RssFeed> _rssFeedRepo;
        private readonly IRepository<RssDownload> _rssDownloadRepo;
        private readonly HttpClient _client;
        private readonly ILogger<RssService> _logger;

        public RssService(IRepository<RssFeed> rssFeedRepo, IRepository<RssDownload> rssDownloadRepo, HttpClient client, ILogger<RssService> logger) {
            _rssFeedRepo = rssFeedRepo;
            _rssDownloadRepo = rssDownloadRepo;
            _client = client;
            _logger = logger;
        }

        public async Task AddHashDownloads(IEnumerable<RssDownload> downloads) {
            // add bulk support for this to improve optimization.
            foreach (var download in downloads)
                download.Id = await _rssDownloadRepo.AddAsync(download);
        }

        public async Task<IReadOnlyCollection<RssDownload>> GetNewHashesFromFeeds() {
            var feeds = await _rssFeedRepo.WhereLessThanAsync(r => r.StartDate, DateTimeOffset.UtcNow, 1000);

            // yes wait on each one, we don't want to ddos the endpoints.
            var downloads = new List<RssDownload>();
            foreach (var feed in feeds) {
                var hashes = await ProcessFeed(feed);
                downloads.AddRange(hashes
                    .Where(h => !string.IsNullOrWhiteSpace(h))
                    .Select(h => new RssDownload {
                        Hash = h,
                        RssFeedId = feed.Id,
                    }));
            }

            // remove any already downloaded.
            var dups = await _rssDownloadRepo.WhereInAsync(r => r.Hash, downloads.Select(d => d.Hash), limit: downloads.Count);
            var dupHashes = dups.Select(d => d.Hash).ToHashSet();
            return downloads.Where(d => !dupHashes.Contains(d.Hash)).ToArray();
        }

        public async Task<IReadOnlyCollection<string>> ProcessFeed(RssFeed feed) {
            // Find what we should download.
            var xml = await GetXmlFromFeed(feed);

            // TODO: Add logic to handle the XML based on it's origin.
            // for now, we just assume it's nyaa
            if (feed.Url.Contains("subsplease.org", StringComparison.OrdinalIgnoreCase)) {
                return GetHashesFromSubsPleaseXML(xml, feed).ToArray();
            }
            else return GetHashesFromNyaaXML(xml, feed).ToArray();
        }

        private async Task<string> GetXmlFromFeed(RssFeed feed) {
            var res = await _client.GetAsync(feed.Url);
            if (!res.IsSuccessStatusCode) throw new HttpRequestException("Failed to get return XML from feed."); // what do we do here..?

            return await res.Content.ReadAsStringAsync();
        }

        private IEnumerable<string> GetHashesFromSubsPleaseXML(string xml, RssFeed feed) {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("//item", nsmgr)) {
                var pubDate = node["pubDate"];

                // If the date is valid AND is past the pub date, lets go.
                if (DateTimeOffset.TryParse(pubDate.InnerText, out DateTimeOffset date)
                    && date > feed.StartDate) {
                    var hashNode = node["link"];
                    yield return hashNode.InnerText;
                }
            }
        }

        private IEnumerable<string> GetHashesFromNyaaXML(string xml, RssFeed feed) {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            foreach (XmlNode node in doc.DocumentElement.SelectNodes("//item", nsmgr)) {
                var pubDate = node["pubDate"];

                // If the date is valid AND is past the pub date, lets go.
                if (DateTimeOffset.TryParse(pubDate.InnerText, out DateTimeOffset date)
                    && date > feed.StartDate) {
                    var hashNode = node["nyaa:infoHash"];
                    yield return hashNode?.InnerText ?? string.Empty;
                }
            }
        }
    }
}