using JJMedia5.Core.Models;
using System.Threading.Tasks;

namespace JJMedia5.Core.Interfaces {
    public interface ISeriesEpisodeSearchService {

        Task<EpisodeSeriesInfo> FindAsync(string fileName);

    }
}
