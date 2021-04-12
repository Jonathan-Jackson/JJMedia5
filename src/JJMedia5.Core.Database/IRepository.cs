using JJMedia5.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {

    public interface IRepository<TEntity> where TEntity : Entity {

        Task<int> AddAsync(TEntity item);

        Task<int> DeleteAsync(int id);

        Task<TEntity> FindAsync(int id);

        Task<ICollection<TEntity>> GetAsync(int limit = 10);

        Task<int> UpdateAsync(TEntity item);

        Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10);

        // Is there a better way to do this with expressions? Just take the lazy route for now since the outcome is the same,
        // and with generics we only have to write this once - more complex queries will need overrides anyway.
        Task<ICollection<TEntity>> WhereGreaterThanAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10);

        Task<ICollection<TEntity>> WhereInAsync(Expression<Func<TEntity, object>> expression, IEnumerable<object> values, int limit = 100);

        Task<ICollection<TEntity>> WhereLessThanAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10);

        Task<ICollection<TEntity>> WhereNotAsync(Expression<Func<TEntity, object>> expression, object value, int limit = 10);

        //--
    }
}