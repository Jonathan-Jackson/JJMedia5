using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJMedia5.FileManager.Clients {

    public interface ITorrentClient {

        Task AddHash(string hash);

        Task<IEnumerable<string>> GetActiveTorrentPaths();

        Task RemoveCompleteTorrents();
    }
}