using JJMedia5.Core.Entities;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {
    public interface IEpisodeRepository : IRepository<Episode> {
        Task<Episode> FindAsync(int episodeNumber, int seasonNumber, int seriesId);
    }
}