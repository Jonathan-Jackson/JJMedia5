using JJMedia5.Core.Models;
using System.Threading.Tasks;

namespace JJMedia5.Core.Interfaces {
    public interface IEpisodeLookupService {

        Task<EpisodeSeriesInfo> FindEpisodeSeriesInfoAsync(string fileName);

    }
}
