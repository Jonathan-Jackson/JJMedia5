using JJMedia5.Core.Entities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database.DbLite
{

    public class DbLiteRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly ILiteCollection<TEntity> _entityDb;

        public DbLiteRepository(ILiteCollection<TEntity> entityDb)
        {
            _entityDb = entityDb;
        }

        public virtual Task<int> AddAsync(TEntity entity)
        {
            int id = _entityDb.Insert(entity).AsInt32;
            return Task.FromResult(id);
        }

        public virtual Task<int> DeleteAsync(int id)
        {
            _entityDb.Delete(new BsonValue(id));
            return Task.FromResult(id);
        }

        public virtual Task<TEntity> FindAsync(int id)
        {
            return Task.FromResult(_entityDb.FindById(new BsonValue(id)));
        }

        public virtual Task<ICollection<TEntity>> GetAsync(int limit = 10)
        {
            ICollection<TEntity> results = _entityDb.Find(_ => true, 0, limit).ToList();
            return Task.FromResult(results);
        }

        public virtual Task<int> UpdateAsync(TEntity entity)
        {
            if (_entityDb.Update(entity))
            {
                return Task.FromResult(entity.Id);
            }
            else
            {
                throw new InvalidOperationException($"Entity with Id {entity.Id} does not exist.");
            }
        }

        public Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> expression, int limit = 10) {
            ICollection<TEntity> results = _entityDb.Find(expression, 0, limit).ToList();
            return Task.FromResult(results);
        }

        public Task<ICollection<TEntity>> WhereGreaterThanAsync(Expression<Func<TEntity, bool>> expression, int limit = 10) {
            ICollection<TEntity> results = _entityDb.Find(expression, 0, limit).ToList();
            return Task.FromResult(results);
        }

        public Task<ICollection<TEntity>> WhereInAsync(Expression<Func<TEntity, bool>> expression, int limit = 100) {
            ICollection<TEntity> results = _entityDb.Find(expression, 0, limit).ToList();
            return Task.FromResult(results);
        }

        public Task<ICollection<TEntity>> WhereLessThanAsync(Expression<Func<TEntity, bool>> expression, int limit = 10) {
            ICollection<TEntity> results = _entityDb.Find(expression, 0, limit).ToList();
            return Task.FromResult(results);
        }

        public Task<ICollection<TEntity>> WhereNotAsync(Expression<Func<TEntity, bool>> expression, int limit = 10) {
            ICollection<TEntity> results = _entityDb.Find(expression, 0, limit).ToList();
            return Task.FromResult(results);
        }
    }
}