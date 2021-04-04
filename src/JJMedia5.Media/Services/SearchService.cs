using JJMedia5.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJMedia5.Media.Services {

    public abstract class SearchService<TEntity> where TEntity : Entity {

        public async Task<TEntity> FindAsync(string searchValue) {
            // We create a sub-list of our search value, pulled apart to find 'smart' potential values that
            // it could be.
            foreach (string value in CreatePrioritySearches(searchValue)) {
                TEntity result = await FindInDatabaseAsync(searchValue);

                if (result != null)
                    return result;

                result = await FindInAPIAsync(searchValue);

                if (result != null) {
                    result.Id = await AddSearchResultToDatabaseAsync(result);
                    return result;
                }
            }

            return null;
        }

        protected abstract IEnumerable<string> CreatePrioritySearches(string searchValue);

        protected abstract Task<int> AddSearchResultToDatabaseAsync(TEntity result);

        protected abstract Task<TEntity> FindInAPIAsync(string searchValue);

        protected abstract Task<TEntity> FindInDatabaseAsync(string searchValue);
    }
}