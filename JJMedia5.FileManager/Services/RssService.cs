using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace JJMedia5.FileManager.Services {

    public class RssService {
        private readonly IRepository<RssFeed> _repo;
        private readonly HttpClient _client;

        public RssService(IRepository<RssFeed> repo, HttpClient client) {
            _repo = repo;
            _client = client;
        }

        public async Task<IReadOnlyCollection<string>> GetHashesFromFeeds() {
            var feeds = await _repo.WhereLessThanAsync(r => r.StartDate, DateTimeOffset.UtcNow, 1000);

            // yes wait on each one, we don't want to ddos the endpoints.
            var hashes = new List<string>();
            foreach (var feed in feeds) {
                hashes.AddRange(await ProcessFeed(feed));
            }

            return hashes;
        }

        public async Task<IReadOnlyCollection<string>> ProcessFeed(RssFeed feed) {
            // Find what we should download.
            var xml = await GetXmlFromFeed(feed);

            // TODO: Add logic to handle the XML based on it's origin.
            // for now, we just assume it's nyaa
            return GetHashesFromNyaaXML(xml, feed).ToArray();
        }

        private async Task<string> GetXmlFromFeed(RssFeed feed) {
            var res = await _client.GetAsync(feed.Url);
            if (!res.IsSuccessStatusCode) throw new HttpRequestException("Failed to get return XML from feed."); // what do we do here..?

            return await res.Content.ReadAsStringAsync();
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
                    yield return hashNode.InnerText;
                }
            }
        }
    }
}