using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Services {

    public class FileService {
        private readonly IReadOnlyCollection<string> _storePaths;
        private readonly IReadOnlyCollection<string> _sourcePaths;
        private readonly string _stagePath;

        public FileService(IReadOnlyCollection<string> storePaths, IReadOnlyCollection<string> sourcePaths, string stagePath) {
            if (storePaths == null || storePaths.Count < 1 || storePaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"StorePaths is empty - this is required to find were to push files to.");

            if (sourcePaths == null || sourcePaths.Count < 1 || sourcePaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"SourcePaths is empty - this is required to find what files need moving.");

            if (string.IsNullOrWhiteSpace(stagePath))
                throw new ArgumentException($"StagePath is empty - this is required to find what files need moving.");

            _storePaths = storePaths;
            _sourcePaths = sourcePaths;
            _stagePath = stagePath;
        }

        internal async Task ProcessMedia(IEnumerable<string> ignoredPaths) {
            var files = _sourcePaths.SelectMany(Directory.GetFiles);
            var filtered = files.Except(ignoredPaths, StringComparer.OrdinalIgnoreCase);

            // Move to staged area
            foreach (var file in filtered)
                File.Move(file, Path.Join(_stagePath, Path.GetFileName(file)));

            // Add a static wait after moving files to ensure windows
            // has cleaned up file handles
            await Task.Delay(5_000);
        }
    }
}