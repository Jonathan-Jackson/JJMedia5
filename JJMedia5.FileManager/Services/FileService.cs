using JJMedia5.Core;
using JJMedia5.FileManager.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Services {

    public class FileService {
        private readonly ILogger<FileService> _logger;
        private readonly IReadOnlyCollection<string> _processingPaths;
        private readonly IReadOnlyCollection<string> _downloadPaths;
        private readonly IReadOnlyCollection<string> _mediaPaths;

        public FileService(StorageOptions options, ILogger<FileService> logger)
            : this(options.ProcessingPaths, options.DownloadPaths, options.MediaPaths, logger) {
        }

        public FileService(IReadOnlyCollection<string> processingPaths, IReadOnlyCollection<string> downloadPaths, IReadOnlyCollection<string> mediaPaths, ILogger<FileService> logger) {
            if (processingPaths == null || processingPaths.Count < 1 || processingPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"ProcessingPaths is empty - this is required to find were to push files to.");
            if (mediaPaths == null || mediaPaths.Count < 1 || mediaPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"MediaPaths is empty - this is required to find were to push files to.");
            if (downloadPaths == null || downloadPaths.Count < 1 || downloadPaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"DownloadPaths is empty - this is required to find what files need moving.");

            _processingPaths = processingPaths;
            _downloadPaths = downloadPaths;
            _mediaPaths = mediaPaths;
            _logger = logger;
        }

        internal async Task ProcessMedia(IEnumerable<string> ignoredPaths) {
            var files = _downloadPaths.SelectMany(Directory.GetFiles);
            var filtered = files.Except(ignoredPaths, StringComparer.OrdinalIgnoreCase);

            // Move to staged area
            foreach (var file in filtered) {
                _logger?.LogDebug($"Moving File To Processing: {Path.GetFileName(file)}");
                if (await FileHelper.RetryMoveAsync(file, Path.Join(GetFreeProcessingPath(), Path.GetFileName(file))))
                    _logger?.LogInformation($"Moved File To Processing: {Path.GetFileName(file)}");
                else
                    _logger?.LogError($"Failed to Move File To Processing: {Path.GetFileName(file)}");
            }

            // Add a static wait after moving files to ensure windows
            // has cleaned up file handles
            await Task.Delay(5_000);
        }

        private string GetFreeProcessingPath()
            => GetFirstFreePath(_processingPaths);

        /// <summary>
        /// TODO!
        /// </summary>
        private string GetFirstFreePath(IEnumerable<string> paths)
            => paths.First();
    }
}