using JJMedia5.Core.Entities;
using LiteDB;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database.DbLite {

    public class DbLiteEpisodeRepository : DbLiteRepository<Episode>, IEpisodeRepository
    {
        public DbLiteEpisodeRepository(ILiteCollection<Episode> entityDb)
            : base(entityDb)
        {
        }

        public Task<Episode> FindAsync(int episodeNumber, int seasonNumber, int seriesId) {
            var episode = _entityDb.FindOne(ep => ep.EpisodeNumber == episodeNumber 
                && ep.SeasonNumber == seasonNumber 
                && ep.SeriesId == seriesId);
            return Task.FromResult(episode);
        }

    }
}