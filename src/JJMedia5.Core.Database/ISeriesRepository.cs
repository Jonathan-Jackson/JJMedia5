using JJMedia5.Core.Entities;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {
    public interface ISeriesRepository : IRepository<Series> {
        Task<Series> FindBySourceAsync(int id);
        Task<int> FindIdByTitleNameAsync(string titleName);
    }
}