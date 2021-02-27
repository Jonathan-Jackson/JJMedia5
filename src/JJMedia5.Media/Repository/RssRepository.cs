using JJMedia5.Core.Entities;
using SqlKata.Execution;
using System.Threading.Tasks;

namespace JJMedia5.Media.Repository {

    public class RssRepository : BaseRepository {

        public RssRepository(string sqlConn) : base(sqlConn, "RssFeeds") {
        }

        public Task<int> AddAsync(RssFeed rss) {
            return ExecAsync(db => db.Query(_tableName).InsertAsync(new {
                rss.Info,
                rss.IsSubscribed,
                rss.CreatedOn,
                rss.Url
            }));
        }
    }
}