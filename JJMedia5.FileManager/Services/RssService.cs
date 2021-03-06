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

        public async Task PollFeeds() {
            var feeds = await _repo.WhereLessThanAsync(r => r.StartDate, DateTimeOffset.UtcNow, 1000);

            foreach (var feed in feeds)
                await ProcessFeed(feed); // yes wait on each one, we don't want to ddos the endpoints.
        }

        public async Task ProcessFeed(RssFeed feed) {
            // Find what we should download.
            var xml = await GetXmlFromFeed(feed);
            var hashes = GetHashesFromXML(xml, feed).ToArray();

            // magnet / hashes - send to torrent client.
        }

        private async Task<string> GetXmlFromFeed(RssFeed feed) {
            var res = await _client.GetAsync(feed.Url);
            if (!res.IsSuccessStatusCode) throw new HttpRequestException("Failed to get return XML from feed."); // what do we do here..?

            return await res.Content.ReadAsStringAsync();
        }

        private IEnumerable<string> GetHashesFromXML(string xml, RssFeed feed) {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            string xPathString = feed.XmlXPathLink;
            var nodes = doc.DocumentElement.SelectNodes(xPathString, nsmgr);

            foreach (XmlNode node in nodes)
                yield return node.InnerText;
        }
    }
}