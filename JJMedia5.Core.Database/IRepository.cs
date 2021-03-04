using JJMedia5.Core.Entities;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {

    public interface IRepository<TEntity> where TEntity : Entity {

        Task<int> AddAsync(TEntity item);

        Task<int> DeleteAsync(int id);

        Task<TEntity> FindAsync(int id);

        Task<ICollection<TEntity>> GetAsync(int limit = 10);

        Task<int> UpdateAsync(TEntity item);
    }
}