using JJMedia5.FileManager.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JJMedia5.FileManager {
    public class FileManagerService {
        private static FileService _fileService;
        private static RssService _rssService;
        private static TorrentService _torrentService;
        private static ILogger<FileManagerService> _logger;


        // This semaphore exists to make sure that 
        // download torrents / poll / move files
        // do not overlap. We can run them on seperate timers.
        private static SemaphoreSlim _semaphore;

        public FileManagerService(FileService fileService, RssService rssService, TorrentService torrentService, ILogger<FileManagerService> logger) {
            _semaphore = new SemaphoreSlim(1, 1);

            _fileService = fileService;
            _rssService = rssService;
            _torrentService = torrentService;
            _logger = logger;
        }

        public async Task PollCompleteFiles() {
            for (; ; await Task.Delay(60_000)) {
                try {
                    await _semaphore.WaitAsync();
                    // Delay slightly for any processes to fully finish (file locks / torrent client).
                    await Task.Delay(5000);

                    // Clear out & Process
                    var removed = await _torrentService.RemoveCompleteTorrents();
                    await _fileService.ProcessMedia(removed);

                    _logger.LogInformation("Poll for completed files has finished.");
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error thrown on polling for complete files.");
                }
                finally {
                    _semaphore.Release();
                }
            }
        }

        public async Task PollPendingFiles() {
            for (; ; await Task.Delay(12_600_000)) {
                try {
                    await _semaphore.WaitAsync();
                    // Delay slightly for any processes to fully finish (file locks / torrent client).
                    await Task.Delay(5000);

                    // There may have been torrents that failed to move in the past,
                    // so we just re-process everything that isn't active.
                    IEnumerable<string> paths = await _torrentService.GetActiveTorrentPaths();
                    await _fileService.ProcessPendingMedia(ignoredPaths: paths);

                    _logger.LogInformation("Poll for pending files has finished.");
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error thrown on polling for pending files.");
                }
                finally {
                    _semaphore.Release();
                }
            }
        }

        public async Task PollFeeds() {
            for (; ; await Task.Delay(600_000)) {
                try {
                    await _semaphore.WaitAsync();

                    // We want to process these
                    // in sequence, to avoid disrupting a previous step.
                    var toDownload = (await _rssService.GetNewHashesFromFeeds());

                    if (toDownload.Any()) {
                        await _torrentService.DownloadHashes(toDownload.Select(d => d.Hash));
                        await _rssService.AddHashDownloads(toDownload);
                        foreach (var download in toDownload) {
                            _logger.LogInformation($"Downloaded New Hash: {download.Id}");
                        }
                    }

                    _logger.LogInformation("Poll for feeds has finished.");
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error thrown on polling feeds.");
                }
                finally {
                    _semaphore.Release();
                }
            }
        }


    }
}
