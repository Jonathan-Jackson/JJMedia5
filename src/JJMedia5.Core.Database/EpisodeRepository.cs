using JJMedia5.Core.Attributes;
using JJMedia5.Core.Entities;
using SqlKata.Execution;
using System.Linq;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {

    public class EpisodeRepository : EntityRepository<Episode> {

        public EpisodeRepository(JJMediaDbManager connectionInfo)
            : base(connectionInfo) {
        }

        public async Task<Episode> FindAsync(int episodeNumber, int seasonNumber, int seriesId) {
            return await ExecAsync(db => db
                .Query(_tableName)
                .Where("SeriesId", seriesId)
                .Where("EpisodeNumber", episodeNumber)
                .Where("SeasonNumber", seasonNumber)
                .FirstOrDefaultAsync<Episode>());
        }
    }
}