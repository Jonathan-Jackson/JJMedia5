using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.Core.Enums;
using JJMedia5.Media.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace JJMedia5.Media.Services {

    public class SeriesSearchService {
        private readonly TMDbClient _apiClient;
        private readonly ISeriesRepository _repo;

        public SeriesSearchService(TMDbClient tvDbClient, ISeriesRepository repo) {
            _apiClient = tvDbClient;
            _repo = repo;
        }

        public async Task<Series> FindAsync(string fileName) {
            foreach (var value in GetPossibleOrderedSeriesSearchValues(fileName)) {
                var result = await FindByValueAsync(value);
                if (result != null) {
                    return result;
                }
            }

            // not found.
            return null;
        }

        public async Task<Series> FindByValueAsync(string value) {
            Series result = await FindInDatabaseAsync(value);

            // A little nuance, but if this aired a while ago - check @ the API
            // to see if a new show has been added (since we last looked).
            if (result != null && result.AddedOn < DateTime.UtcNow.AddMonths(-2) && result.AirDate > DateTime.UtcNow.AddYears(-2))
                return result;

            result = await FindInAPIAsync(value);

            if (result != null) {
                result.Id = await AddSearchResultToDatabaseAsync(result);
                return result;
            }
            return null;
        }

        public IEnumerable<string> GetPossibleOrderedSeriesSearchValues(string fileName) {
            string noExtension = Path.GetFileNameWithoutExtension(fileName).Trim();
            string noMetadata = MediaNameHelper.RemoveMetadata(noExtension).Trim();

            string noEpisode = MediaNameHelper.RemoveEpisode(noMetadata).Trim();
            yield return noEpisode;

            string noSeason = MediaNameHelper.RemoveSeasonNotation(noEpisode).Trim();
            yield return noSeason;

            // remove the last word ~ this is odd, but it's a last ditch,
            // and often we get shows with silly names at the end.
            if (noSeason.Contains(' ') && noSeason.Length > 8) {
                yield return noSeason.Substring(0, noSeason.LastIndexOf(' ')).Trim();
            }

            // return the full one as the lowest priority (without an extension).
            yield return noMetadata;
            yield return noExtension;
        }

        protected Task<int> AddSearchResultToDatabaseAsync(Series result)
            => _repo.AddAsync(result);

        protected async Task<Series> FindInAPIAsync(string searchValue) {
            var result = await _apiClient.SearchTvShowAsync(searchValue);
            if (!result.Results.Any())
                return null;

            // update to find the most fitting result.
            var foundSeries = result.Results.OrderByDescending(y => y.VoteCount).FirstOrDefault();
            if (foundSeries == null)
                return null;

            var titles = new List<SeriesTitle>() {
                new SeriesTitle(foundSeries.Name, isPrimary: true)
            };

            // Add the other tittles (original & the search value)
            if (!foundSeries.OriginalName.Equals(foundSeries.Name, StringComparison.OrdinalIgnoreCase)) {
                titles.Add(new SeriesTitle(foundSeries.OriginalName, isPrimary: false));
            }
            if (!searchValue.Equals(foundSeries.Name, StringComparison.OrdinalIgnoreCase)
                    && !searchValue.Equals(foundSeries.OriginalName, StringComparison.OrdinalIgnoreCase)) {
                titles.Add(new SeriesTitle(searchValue, isPrimary: false));
            }

            return new Series {
                Description = foundSeries.Overview,
                AirDate = foundSeries.FirstAirDate,
                Titles = titles.Where(s => !string.IsNullOrWhiteSpace(s.Title)).ToArray(),
                SourceApi = eSeriesApi.TheMovieDb,
                SourceId = foundSeries.Id.ToString()
            };
        }

        protected async Task<Series> FindInDatabaseAsync(string searchValue) {
            int showId = await _repo.FindIdByTitleNameAsync(searchValue);

            return showId > 0
                ? await _repo.FindAsync(showId)
                : null;
        }
    }
}