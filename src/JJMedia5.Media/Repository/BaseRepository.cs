using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace JJMedia5.Media.Repository {

    public class BaseRepository {
        private readonly string _sqlConn;
        protected readonly string _tableName;

        public BaseRepository(string sqlConn, string tableName) {
            _sqlConn = sqlConn;
            _tableName = tableName;
        }

        protected async Task<T> ExecAsync<T>(Func<QueryFactory, Task<T>> dbAction) {
            using (var connection = new SqlConnection(_sqlConn)) {
                var compiler = new SqlServerCompiler();

                using (var db = new QueryFactory(connection, compiler))
                    return await dbAction(db);
            }
        }

        public virtual async Task<T> GetAsync<T>(int id) {
            return await ExecAsync(async db => await db.Query(_tableName)
                                                        .Where("id", id)
                                                        .FirstAsync());
        }
    }
}