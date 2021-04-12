using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.Core.Enums;
using JJMedia5.Media.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace JJMedia5.Media.Services {

    public class EpisodeSearchService {
        private readonly EpisodeRepository _repo;
        private readonly TMDbClient _apiClient;

        public EpisodeSearchService(TMDbClient tvDbClient, EpisodeRepository repo) {
            _apiClient = tvDbClient;
            _repo = repo;
        }

        public async Task<Episode> FindAsync(Series series, string fileName) {
            var noExtension = Path.GetFileNameWithoutExtension(fileName);

            int episodeNumber = FindEpisodeNumber(noExtension);
            int seasonNumber = FindSeasonNumber(series, noExtension);

            // check the database!
            Episode result = await _repo.FindAsync(episodeNumber, seasonNumber, series.Id);

            if (result != null)
                return result;

            // check api
            result = await FindWithApi(series, seasonNumber, episodeNumber);
            if (result != null) {
                result.Id = await _repo.AddAsync(result);
                return result;
            }

            return null;
        }

        private async Task<Episode> FindWithApi(Series series, int seasonNumber, int episodeNumber) {
            if (string.IsNullOrWhiteSpace(series.SourceId) || series.SourceApi != eSeriesApi.TheMovieDb)
                return null;

            var result = await _apiClient.GetTvEpisodeAsync(int.Parse(series.SourceId), seasonNumber, episodeNumber);
            if (result == null) {
                // if it's a really high number, check our end point to see
                // if the episode number we have is an AbsoluteNumber rather
                // than EpisodeNumber

                // -- find how many episodes in the season so far

                // -- are we +1 more then that?? And a season 2 exists???

                // ok probs absolute.

                return null;
            }

            return new Episode {
                SourceApi = eSeriesApi.TheMovieDb,
                SourceId = result.Id?.ToString() ?? "0",
                AiredOn = result.AirDate,
                Description = result.Overview ?? string.Empty,
                Title = result.Name ?? string.Empty,
                EpisodeNumber = result.EpisodeNumber,
                SeasonNumber = result.SeasonNumber,
                SeriesId = series.Id
            };
        }

        private int FindSeasonNumber(Series series, string fileName) {
            var removedMetadata = MediaNameHelper.RemoveMetadata(fileName);

            // TODO: add regex to pattern match on S{digit}E{digit}.
            var fileParts = removedMetadata.Split('-');

            // for now, we just look for special notation
            // we can also look at our db to see latest season & if already exists?

            return 1;
        }

        private int FindEpisodeNumber(string fileName) {
            var removedMetadata = MediaNameHelper.RemoveMetadata(fileName);

            // TODO: add regex to pattern match on S{digit}E{digit}.
            var fileParts = removedMetadata.Split('-');

            // files sometimes contain versions
            // i.e. [jam] one piece - 12v3.mkv
            // so we wanna throw that version away.
            var noVersionNumbers = fileParts.Select(val => val.Split('v').First());

            var episodePart = noVersionNumbers.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(part => part.All(char.IsDigit));
            return string.IsNullOrWhiteSpace(episodePart)
                ? 1
                : int.Parse(episodePart);
        }
    }
}