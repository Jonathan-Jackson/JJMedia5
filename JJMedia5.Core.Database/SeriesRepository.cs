using JJMedia5.Core.Entities;
using SqlKata.Execution;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {

    public class SeriesRepository : EntityRepository<Series> {
        private const string _titlesTableName = "SeriesTitles";

        public SeriesRepository(JJMediaDbManager connectionInfo)
            : base(connectionInfo) {
        }

        public Task<int> FindIdByTitleName(string titleName) {
            return ExecAsync(db => db
                .Query(_tableName)
                .Select(_tableName + ".Id")
                .Join(_titlesTableName, _tableName + ".Id", _titlesTableName + ".SeriesId")
                .Where(_titlesTableName + ".Title", titleName)
                .FirstOrDefaultAsync<Series>());
        }
    }
}