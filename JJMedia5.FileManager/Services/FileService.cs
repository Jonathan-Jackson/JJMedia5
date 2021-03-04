using System;
using System.Collections.Generic;

namespace JJMedia5.FileManager.Services {

    public class FileService {
        private readonly IReadOnlyCollection<string> _storePaths;
        private readonly IReadOnlyCollection<string> _sourcePaths;

        public FileService(IReadOnlyCollection<string> storePaths, IReadOnlyCollection<string> sourcePaths) {
            if (storePaths == null || storePaths.Count > 0)
                throw new ArgumentException($"StorePaths is empty - this is required to find were to push files to.");

            if (sourcePaths == null || sourcePaths.Count > 0)
                throw new ArgumentException($"SourcePaths is empty - this is required to find what files need moving.");

            _storePaths = storePaths;
            _sourcePaths = sourcePaths;
        }
    }
}