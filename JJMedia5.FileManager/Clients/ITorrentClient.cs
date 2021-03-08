using System.Threading.Tasks;

namespace JJMedia5.FileManager.Clients {

    public interface ITorrentClient {

        Task AddHash(string hash);
    }
}