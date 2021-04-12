using JJMedia5.Core;
using JJMedia5.Core.Models;
using JJMedia5.FileManager.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace JJMedia5.FileManager.Services {

    public class FileService {
        private readonly HttpClient _client;
        private readonly IReadOnlyCollection<string> _downloadPaths;
        private readonly ILogger<FileService> _logger;
        private readonly IReadOnlyCollection<string> _mediaPaths;
        private readonly IReadOnlyCollection<string> _processingPaths;
        private readonly string _mediaApiAddress;

        private IReadOnlyCollection<string> ValidMediaFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            ".mp4",
            ".webm",
            ".mkv"
        };

        public FileService(StorageOptions options, HttpClient client, ILogger<FileService> logger) {
            if (options.ProcessingPaths == null || options.ProcessingPaths.Length < 1 || options.ProcessingPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"ProcessingPaths is empty - this is required to find were to push files to.");
            if (options.MediaPaths == null || options.MediaPaths.Length < 1 || options.MediaPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"MediaPaths is empty - this is required to find were to push files to.");
            if (options.DownloadPaths == null || options.DownloadPaths.Length < 1 || options.DownloadPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"DownloadPaths is empty - this is required to find what files need moving.");

            _processingPaths = options.ProcessingPaths;
            _downloadPaths = options.DownloadPaths;
            _mediaPaths = options.MediaPaths;
            _mediaApiAddress = options.MediaApiAddress;
            _client = client;
            _logger = logger;
        }

        internal async Task ProcessMedia(IEnumerable<string> ignoredPaths) {
            var files = _downloadPaths.SelectMany(Directory.GetFiles);
            var filtered = files.Except(ignoredPaths, StringComparer.OrdinalIgnoreCase);

            // Move to staged area
            foreach (var source in filtered) {
                _logger?.LogDebug($"Moving File To Processing: {Path.GetFileName(source)}");
                var dest = Path.Join(GetFreeProcessingPath(), Path.GetFileName(source));
                if (await FileHelper.RetryMoveAsync(source, dest)) {
                    _logger?.LogInformation($"Moved File To Processing: {Path.GetFileName(dest)}");
                }
                else {
                    _logger?.LogError($"Failed to Move File To Processing: {source}");
                }
            }

            var toProcess = _processingPaths.SelectMany(Directory.GetFiles)
                                        .Where(IsValidMediaFile);
            if (toProcess.Any()) {
                // Add a static wait after moving files to ensure windows
                // has cleaned up file handles
                await FileHandleDelay();
                await TryMoveToFinalStorageAsync(toProcess);
            }
        }

        private Task FileHandleDelay()
            => Task.Delay(5_000);

        private async Task<EpisodeSeriesInfo> GetEpisodeSeriesInfoAsync(string fileName) {
            var result = await _client.GetAsync($"{_mediaApiAddress}search/seriesepisode?filename={HttpUtility.UrlEncode(fileName)}");

            if (result.IsSuccessStatusCode) {
                string body = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EpisodeSeriesInfo>(body, new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                });
            }
            else {
                throw new HttpRequestException($"Failed to get episode info. Reason: {result.ReasonPhrase}");
            }
        }

        /// <summary>
        /// TODO!
        /// </summary>
        private string GetFirstFreePath(IEnumerable<string> paths)
            => paths.First();

        private string GetFreeMediaPath()
            => GetFirstFreePath(_mediaPaths);

        private string GetFreeProcessingPath()
            => GetFirstFreePath(_processingPaths);

        private bool IsValidMediaFile(string path)
            => File.Exists(path)
                && ValidMediaFileExtensions.Contains(Path.GetExtension(path))
                && new FileInfo(path).Length > 100;

        private async Task TryMoveToFinalStorageAsync(IEnumerable<string> paths) {
            foreach (var path in paths) {
                await TryMoveToFinalStorageAsync(path);
            }
        }

        private string RemoveInvalidPathChars(string filename)
            => string.Concat(filename.Split(Path.GetInvalidFileNameChars()));

        private async Task TryMoveToFinalStorageAsync(string path) {
            try {
                // get episode info from api.
                var episodeInfo = await GetEpisodeSeriesInfoAsync(Path.GetFileName(path));

                // create parent folders
                var directory = Path.Join(GetFreeMediaPath(), RemoveInvalidPathChars(episodeInfo.SeriesTitle), $"Season {episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}");
                Directory.CreateDirectory(directory);

                // create new name.
                string newName = $"{episodeInfo.SeriesTitle} - S{episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}E{episodeInfo.EpisodeNumber.ToString().PadLeft(2, '0')} ({episodeInfo.EpisodeTitle}){Path.GetExtension(path)}";
                if (newName.Length > MaxFileNameLength) {
                    newName = $"{episodeInfo.SeriesTitle} - S{episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}E{episodeInfo.EpisodeNumber.ToString().PadLeft(2, '0')}{Path.GetExtension(path)}";
                }

                var output = Path.Join(GetFreeMediaPath(), RemoveInvalidPathChars(episodeInfo.SeriesTitle), $"Season {episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}", RemoveInvalidPathChars(newName));
                await FileHelper.RetryMoveAsync(path, output);

                _logger.LogInformation($"New File Added to Media Store: {output}");
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Unable to process path: {path}");
            }
        }

        private const int MaxFileNameLength = 200;
    }
}