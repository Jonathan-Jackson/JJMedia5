using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace JJMedia5.Media.Services {

    public class SeriesSearchService : SearchService<Series> {
        private readonly SeriesRepository _repo;
        private readonly TMDbClient _apiClient;

        public SeriesSearchService(TMDbClient tvDbClient, SeriesRepository repo) {
            _apiClient = tvDbClient;
            _repo = repo;
        }

        protected override Task<int> AddSearchResultToDatabaseAsync(Series result)
            => _repo.AddAsync(result);

        protected override IEnumerable<string> CreatePrioritySearches(string searchValue) {
            yield return searchValue;

            // this needs a proper implementation
            // as it can be quite complex
            // for now i've done a 5 minute job
            // - remove episode ending
            // - just remove pretty obvious season notations
            // - funky characters (often sub names)
            // - cut off the last sentence if a certain length
            string modified = searchValue.Trim();

            if (modified.Contains('-') && modified.Length > 5) {
                modified = modified.Substring(0, modified.LastIndexOf('-')).Trim();
                yield return modified;
            }

            if (modified != Regex.Replace(modified, @"\[.*\]", "")) {
                modified = Regex.Replace(modified, @"\[.*\]", "").Trim();
                yield return modified;
            }

            if (modified != Regex.Replace(modified, @"\(.*\)", "")) {
                modified = Regex.Replace(modified, @"\(.*\)", "").Trim();
                yield return modified;
            }

            if (modified.Contains(' ') && modified.Length > 5) {
                modified = modified.Substring(0, modified.LastIndexOf(' ')).Trim();
                yield return modified;
            }
        }

        protected override async Task<Series> FindInAPIAsync(string searchValue) {
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
                AirDate = foundSeries?.FirstAirDate,
                Titles = titles.Where(s => !string.IsNullOrWhiteSpace(s.Title)).ToArray()
            };
        }

        protected override async Task<Series> FindInDatabaseAsync(string searchValue) {
            int showId = await _repo.FindIdByTitleNameAsync(searchValue);

            return showId > 0
                ? await _repo.FindAsync(showId)
                : null;
        }
    }
}