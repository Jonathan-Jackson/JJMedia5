using JJMedia5.Core;
using JJMedia5.Core.Interfaces;
using JJMedia5.Core.Models;
using JJMedia5.FileManager.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Services {

    public class FileService {
        private const int MaxFileNameLength = 200;
        private readonly IReadOnlyCollection<string> _downloadPaths;
        private readonly ILogger<FileService> _logger;
        private readonly IReadOnlyCollection<string> _mediaPaths;
        private readonly IReadOnlyCollection<string> _processingPaths;
        private readonly ISeriesEpisodeSearchService _showService;


        private IReadOnlyCollection<string> ValidMediaFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            ".mp4",
            ".webm",
            ".mkv"
        };

        public FileService(StorageOptions options, ISeriesEpisodeSearchService showService, ILogger<FileService> logger) {
            if (options.ProcessingPaths == null || options.ProcessingPaths.Length < 1 || options.ProcessingPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"ProcessingPaths is empty - this is required to find were to push files to.");
            if (options.MediaPaths == null || options.MediaPaths.Length < 1 || options.MediaPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"MediaPaths is empty - this is required to find were to push files to.");
            if (options.DownloadPaths == null || options.DownloadPaths.Length < 1 || options.DownloadPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"DownloadPaths is empty - this is required to find what files need moving.");

            _processingPaths = options.ProcessingPaths;
            _downloadPaths = options.DownloadPaths;
            _mediaPaths = options.MediaPaths;
            _showService = showService ?? throw new ArgumentNullException(nameof(showService));
            _logger = logger;
        }

        internal async Task ProcessMedia(IEnumerable<string> paths) {
            // Move to staged area
            var moved = new List<string>();

            foreach (var source in paths) {
                await TryMoveFileToProcessing(source, moved);
            }

            if (moved.Any()) {
                await FileHandleDelay();
                await TryMoveToFinalStorageAsync(moved);
            }
        }

        private async Task<bool> TryMoveFileToProcessing(string source, List<string> moved) {
            _logger?.LogDebug($"Moving File To Processing: {Path.GetFileName(source)}");

            var dest = Path.Join(GetFreeProcessingPath(), Path.GetFileName(source));
            if (string.Equals(source, dest, StringComparison.OrdinalIgnoreCase)) {
                // we're already were we need to be.
                moved.Add(dest);
                return true;
            }
            else if (await FileHelper.RetryMoveAsync(source, dest)) {
                _logger?.LogInformation($"Moved File To Processing: {Path.GetFileName(dest)}");
                moved.Add(dest);
                return true;
            }
            else {
                _logger?.LogError($"Failed to Move File To Processing: {source}");
                return false;
            }
        }

        internal Task ProcessPendingMedia(IEnumerable<string> ignoredPaths) {
            var downloads = _downloadPaths.SelectMany(Directory.GetFiles);
            var toProcess = _processingPaths.SelectMany(Directory.GetFiles)
                                        .Union(downloads)
                                        .Except(ignoredPaths, StringComparer.OrdinalIgnoreCase)
                                        .Where(IsValidMediaFile);

            return ProcessMedia(toProcess);
        }

        private Task FileHandleDelay()
            => Task.Delay(5_000);

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

        private string RemoveInvalidPathChars(string filename)
            => string.Concat(filename.Split(Path.GetInvalidFileNameChars()));

        private async Task TryMoveToFinalStorageAsync(IEnumerable<string> paths) {
            foreach (var path in paths) {
                await TryMoveToFinalStorageAsync(path);
            }
        }

        private async Task TryMoveToFinalStorageAsync(string path) {
            try {
                var fileInfo = new FileInfo(path);
                if (!fileInfo.Exists)
                    throw new IOException($"File not found: {path}");
                if (fileInfo.Length < 1000) {
                    throw new IOException($"File appears broken: {path}");
                }

                // get episode info from api.
                EpisodeSeriesInfo episodeInfo = episodeInfo = await _showService.FindAsync(Path.GetFileName(path));
                if (episodeInfo == null) {
                    _logger.LogWarning($"Could not find episode info for '{path}'");
                    // LOG FILE FAILURE TO LEARN TO SKIP AFTER X ATTEMPTS...
                    return;
                }

                // create parent folders
                var directory = Path.Join(GetFreeMediaPath(), RemoveInvalidPathChars(episodeInfo.SeriesTitle), $"Season {episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}");
                Directory.CreateDirectory(directory);
                string newName = CreateEpisodeName(path, episodeInfo);

                var output = Path.Join(GetFreeMediaPath(), RemoveInvalidPathChars(episodeInfo.SeriesTitle), $"Season {episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}", RemoveInvalidPathChars(newName));

                // delete any already existing..
                if (File.Exists(output))
                    await FileHelper.RetryDeleteAsync(output);

                await FileHelper.RetryMoveAsync(path, output, throwOnFinalException: true);

                _logger.LogInformation($"New File Added to Media Store: {output}");
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Unable to process path: {path}");
            }
        }

        private static string CreateEpisodeName(string path, EpisodeSeriesInfo episodeInfo) {
            // create new name.
            string newName = $"{episodeInfo.SeriesTitle} - S{episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}E{episodeInfo.EpisodeNumber.ToString().PadLeft(2, '0')} ({episodeInfo.EpisodeTitle}){Path.GetExtension(path)}";
            if (newName.Length > MaxFileNameLength) {
                newName = $"{episodeInfo.SeriesTitle} - S{episodeInfo.EpisodeSeason.ToString().PadLeft(2, '0')}E{episodeInfo.EpisodeNumber.ToString().PadLeft(2, '0')}{Path.GetExtension(path)}";
            }

            return newName;
        }
    }
}