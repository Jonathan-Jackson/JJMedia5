using JJMedia5.Core.Database.DbLite;
using JJMedia5.Core.Entities;
using LiteDB;
using SqlKata.Execution;
using System.Linq;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {

    public class DbLiteSeriesRepository : DbLiteRepository<Series>, ISeriesRepository {
        private DbLiteRepository<SeriesTitle> _titlesRepository;

        public DbLiteSeriesRepository(ILiteCollection<Series> entityDb, DbLiteRepository<SeriesTitle> titlesDb)
            : base(entityDb) {
            _titlesRepository = titlesDb;
        }

        public async Task<Series> FindBySourceAsync(int id) {
            // this needs to be better optimized
            // for now lets just be laaazy.
            var series = await base.FindAsync(id);

            if (series != null) {
                series.Titles = (await _titlesRepository.WhereAsync(t => t.SeriesId == id)).ToArray();
            }

            return series;
        }

        public override async Task<Series> FindAsync(int id) {
            // this needs to be better optimized
            // for now lets just be laaazy.
            var series = await base.FindAsync(id);

            if (series != null) {
                series.Titles = (await _titlesRepository.WhereAsync(t => t.SeriesId == id)).ToArray();
            }

            return series;
        }


        public async Task<int> FindIdByTitleNameAsync(string titleName) {
            // We should use a series title repository...
            var titles = await _titlesRepository.WhereAsync(t => t.Title == titleName, 1);
            return titles.FirstOrDefault()?.Id ?? 0;
        }

        public override async Task<int> AddAsync(Series item) {
            // this needs to be better optimized
            // for now lets just be laaazy.
            // Add the series.
            int seriesId = await base.AddAsync(item);

            foreach (var title in item.Titles) {
                await _titlesRepository.AddAsync(title);
            }

            return seriesId;
        }
    }
}