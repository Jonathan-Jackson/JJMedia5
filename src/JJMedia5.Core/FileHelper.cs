using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JJMedia5.Core {

    public static class FileHelper {

        public static bool RetryMove(string source, string dest, int attempts = 3, int exponential = 1000, bool throwOnFinalException = false) {
            for (int attempt = 1; attempt <= attempts; attempt++) {
                try {
                    File.Move(source, dest);
                    if (File.Exists(dest)) return true;
                }
                catch {
                    if (attempt == attempts && throwOnFinalException) throw;
                }
                Thread.Sleep(exponential * attempt);
            }

            return false;
        }

        public static async Task<bool> RetryDeleteAsync(string source, int attempts = 3, int exponential = 1000, bool throwOnFinalException = false) {
            for (int attempt = 1; attempt <= attempts; attempt++) {
                try {
                    File.Delete(source);
                    if (!File.Exists(source)) return true;
                }
                catch {
                    if (attempt == attempts && throwOnFinalException) throw;
                }
                await Task.Delay(exponential * attempt);
            }

            return false;
        }

        public static async Task<bool> RetryMoveAsync(string source, string dest, int attempts = 3, int exponential = 1000, bool throwOnFinalException = false) {
            for (int attempt = 1; attempt <= attempts; attempt++) {
                try {
                    File.Move(source, dest);
                    if (File.Exists(dest)) return true;
                }
                catch {
                    if (attempt == attempts && throwOnFinalException) throw;
                }
                await Task.Delay(exponential * attempt);
            }

            return false;
        }
    }
}