using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.Core.Enums;
using JJMedia5.Media.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;

namespace JJMedia5.Media.Services
{

    public class EpisodeSearchService {
        private readonly IEpisodeRepository _repo;
        private readonly TMDbClient _apiClient;

        public EpisodeSearchService(TMDbClient tvDbClient, IEpisodeRepository repo) {
            _apiClient = tvDbClient;
            _repo = repo;
        }

        public async Task<Episode> FindAsync(Series series, string fileName) {
            var noExtension = Path.GetFileNameWithoutExtension(fileName);

            int episodeNumber = FindEpisodeNumber(noExtension);
            int seasonNumber = await FindSeasonNumberAsync(series, noExtension);

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

            int sourceId = int.Parse(series.SourceId);
            var result = await _apiClient.GetTvEpisodeAsync(sourceId, seasonNumber, episodeNumber);
            if (result == null && episodeNumber > 10) {
                // Do some dumb checks here.
                // Because the episode couldn't be found,
                // we're going to try searching through seasons
                // and seeing if we can calculate were this lands.
                // Later on we can improve accuracy by adding hints to requests.
                var apiSeires = await _apiClient.GetTvShowAsync(sourceId);
                var seasons = apiSeires.Seasons.Where(s => s.SeasonNumber > 0);

                // we don't modify the original season as that appears correct
                // from within the api, however some APIs use the absolute number
                // to represent the episode number which is a bit weird.
                int newSeason = 0;
                foreach (var season in seasons) {
                    if (episodeNumber - season.EpisodeCount > 0) {
                        episodeNumber -= season.EpisodeCount;
                        newSeason = season.SeasonNumber;
                    }
                    else {
                        break;
                    }
                }

                if (newSeason > 0) {
                    result = await _apiClient.GetTvEpisodeAsync(sourceId, newSeason, episodeNumber);
                }

                // Check if the last episode of the latest season is 6 montths after it aired,
                // this is getting into dangerous best guess territory but unsure alternatives?
                // Sometimes we get 'Season 2 Episode 1' which is actually episode '13' of Season 1, it gets weird.
                var latestSeason = seasons.Where(s => s.AirDate < DateTime.UtcNow)
                                            .OrderByDescending(s => s.SeasonNumber)
                                            .FirstOrDefault();

                if (latestSeason != null) {
                    var lastEp = await _apiClient.GetTvEpisodeAsync(sourceId, latestSeason.SeasonNumber, latestSeason.EpisodeCount);
                    
                    while (lastEp != null && (lastEp.AirDate == null || lastEp.AirDate > DateTime.UtcNow)) {
                        lastEp = await _apiClient.GetTvEpisodeAsync(sourceId, newSeason, lastEp.EpisodeNumber);
                    }

                    if (lastEp != null && lastEp.AirDate > latestSeason.AirDate.Value.AddMonths(6)) {
                        result = lastEp;
                    }
                }

            }

            if (result == null) {
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

        private async Task<int> FindSeasonNumberAsync(Series series, string fileName) {
            var removedMetadata = MediaNameHelper.RemoveMetadata(fileName);
            // TODO: add regex to pattern match on S{digit}E{digit}.

            if (fileName.Contains("Season", StringComparison.OrdinalIgnoreCase)) {
                int i = fileName.IndexOf("Season", StringComparison.OrdinalIgnoreCase);
                string sub = fileName.Substring(i + 6).Trim();

                if (sub.Any() && char.IsDigit(sub.First())) {
                    return int.Parse(string.Concat(sub.TakeWhile(char.IsDigit)));
                }
            }

            var removedEpisode = MediaNameHelper.RemoveEpisode(fileName).Trim();

            // Do we have at the end, or the second to last sentance,
            var atEnd = removedEpisode.Split(' ', StringSplitOptions.RemoveEmptyEntries).TakeLast(2);

            foreach (string value in atEnd) {
                if (value.StartsWith("s", StringComparison.OrdinalIgnoreCase)
                    && value.Skip(1).All(char.IsDigit)
                    && int.TryParse(string.Concat(value.Skip(1)), out int season)) {
                    return season;
                }

                if (MediaNameHelper.SpecialSeasonNotations.Contains(value))
                    return 0;
            }

            // What's the latest season on the API?
            var apiResult = await _apiClient.GetTvShowAsync(int.Parse(series.SourceId));
            var validSeasons = apiResult?.Seasons.Where(s => s.AirDate != null && s.AirDate < DateTime.UtcNow);
            if (validSeasons.Any() == true) {
                return validSeasons.Max(s => s.SeasonNumber);
            }

            // Check folders..!

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