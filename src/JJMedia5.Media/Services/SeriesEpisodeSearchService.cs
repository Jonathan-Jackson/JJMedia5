using JJMedia5.Core.Entities;
using JJMedia5.Core.Models;
using System.Threading.Tasks;

namespace JJMedia5.Media.Services {

    public class SeriesEpisodeSearchService {
        private readonly SeriesSearchService _seriesSearch;
        private readonly EpisodeSearchService _episodeSearch;

        public SeriesEpisodeSearchService(SeriesSearchService seriesSearch, EpisodeSearchService episodeSearch) {
            _seriesSearch = seriesSearch;
            _episodeSearch = episodeSearch;
        }

        public async Task<EpisodeSeriesInfo> FindAsync(string fileName) {
            // Find series.
            Series foundSeries = await _seriesSearch.FindAsync(fileName);

            // Find episode.
            Episode foundEpisode = foundSeries != null
                ? await _episodeSearch.FindAsync(foundSeries, fileName)
                : null;

            return foundSeries != null && foundEpisode != null
                    ? new EpisodeSeriesInfo(foundEpisode, foundSeries)
                    : null;
        }
    }
}