using JJMedia5.Core.Interfaces;
using JJMedia5.Core.Models;
using System;
using System.Threading.Tasks;

namespace JJMedia5.Media.Services {
    public class EpisodeLookupService : IEpisodeLookupService {
        private readonly SeriesEpisodeSearchService _seriesEpisodeSearch;

        public EpisodeLookupService(SeriesEpisodeSearchService seriesEpisodeSearch) {
            _seriesEpisodeSearch = seriesEpisodeSearch ?? throw new ArgumentNullException(nameof(seriesEpisodeSearch));
        }

        public Task<EpisodeSeriesInfo> FindEpisodeSeriesInfoAsync(string fileName) {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            return _seriesEpisodeSearch.FindAsync(fileName);
        }
    }
}
