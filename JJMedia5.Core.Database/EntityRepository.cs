using JJMedia5.Core.Attributes;
using JJMedia5.Core.Entities;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {

    public class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : Entity {
        protected readonly string _tableName;
        private readonly string _sqlConn;

        public EntityRepository(JJMediaDbManager connectionInfo) {
            _sqlConn = connectionInfo.ConnString;
            // is there a cleaner way to attach a datatable name to an entity?
            _tableName = ((RepositoryAttribute)typeof(TEntity).GetCustomAttributes(typeof(RepositoryAttribute), false).First()).TableName;
        }

        public Task<int> AddAsync(TEntity entity) {
            return ExecAsync(db => db.Query(_tableName)
                .InsertGetIdAsync<int>(entity.GetPropertyModel()));
        }

        public virtual Task<int> DeleteAsync(int id) {
            return ExecAsync(db => db.Query(_tableName)
                .Where("id", id)
                .DeleteAsync());
        }

        public Task<TEntity> FindAsync(int id) {
            return ExecAsync(async db => (await db.Query(_tableName)
                                                        .Where("Id", id)
                                                        .FirstOrDefaultAsync<TEntity>()));
        }

        public virtual Task<ICollection<TEntity>> GetAsync(int limit = 10) {
            return ExecAsync<ICollection<TEntity>>(async db => (await db.Query(_tableName)
                                                          .Limit(limit)
                                                          .GetAsync<TEntity>())
                                                          .ToArray());
        }

        public Task<int> UpdateAsync(TEntity entity) {
            return ExecAsync(db => db.Query(_tableName)
                .Where("id", entity.Id)
                .UpdateAsync(entity.GetPropertyModel()));
        }

        public Task<ICollection<TEntity>> WhereNotAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10)
            => WhereAsync(expression, value, "!=", limit);

        public Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10)
            => WhereAsync(expression, value, "=", limit);

        public Task<ICollection<TEntity>> WhereGreaterThanAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10)
            => WhereAsync(expression, value, ">", limit);

        public Task<ICollection<TEntity>> WhereLessThanAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10)
            => WhereAsync(expression, value, "<", limit);

        protected async Task<T> ExecAsync<T>(Func<QueryFactory, Task<T>> dbAction) {
            using (var connection = new SqlConnection(_sqlConn)) {
                var compiler = new SqlServerCompiler();

                using (var db = new QueryFactory(connection, compiler))
                    return await dbAction(db);
            }
        }

        public Task<ICollection<TEntity>> WhereInAsync(Expression<Func<TEntity, object>> expression, IEnumerable<object> values, int limit = 100) {
            var name = GetMemberName(expression);

            return ExecAsync<ICollection<TEntity>>(async db => (await db.Query(_tableName)
                                                          .WhereIn(name, values)
                                                          .Limit(limit)
                                                          .GetAsync<TEntity>())
                                                          .ToArray());
        }

        private Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, object>> expression, object value, string @operator, int limit = 10) {
            var name = GetMemberName(expression);

            return ExecAsync<ICollection<TEntity>>(async db => (await db.Query(_tableName)
                                                          .Where(name, @operator, value)
                                                          .Limit(limit)
                                                          .GetAsync<TEntity>())
                                                          .ToArray());
        }

        private string GetMemberName(Expression<Func<TEntity, object>> expression) {
            if (expression.Body is MemberExpression body) {
                return body.Member.Name;
            }
            else {
                var op = ((UnaryExpression)expression.Body).Operand;
                return ((MemberExpression)op).Member.Name;
            }
        }
    }
}