using JJMedia5.Core.Attributes;
using JJMedia5.Core.Entities;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJMedia5.Core.Database {

    public class SeriesRepository : EntityRepository<Series> {
        private static string _titlesTableName = ((RepositoryAttribute)typeof(SeriesTitle).GetCustomAttributes(typeof(RepositoryAttribute), false).First()).TableName;

        public SeriesRepository(JJMediaDbManager connectionInfo)
            : base(connectionInfo) {
        }

        public async Task<Series> FindBySourceAsync(int id) {
            // this needs to be better optimized
            // for now lets just be laaazy.
            var series = await base.FindAsync(id);

            if (series != null) {
                series.Titles = (await GetSeriesTitles(id)).ToArray();
            }

            return series;
        }

        public override async Task<Series> FindAsync(int id) {
            // this needs to be better optimized
            // for now lets just be laaazy.
            var series = await base.FindAsync(id);

            if (series != null) {
                series.Titles = (await GetSeriesTitles(id)).ToArray();
            }

            return series;
        }

        // move into own repo..
        private Task<IEnumerable<SeriesTitle>> GetSeriesTitles(int seriesId)
            => ExecAsync(db => db
                .Query(_titlesTableName)
                .Where("SeriesId", seriesId)
                .GetAsync<SeriesTitle>());

        public Task<int> FindIdByTitleNameAsync(string titleName) {
            // We should use a series title repository...
            return ExecAsync(db => db
                .Query(_titlesTableName)
                .Select("SeriesId")
                .Where("Title", titleName)
                .FirstOrDefaultAsync<int>());
        }

        public override async Task<int> AddAsync(Series item) {
            // this needs to be better optimized
            // for now lets just be laaazy.
            // Add the series.
            int seriesId = await base.AddAsync(item);

            // ok.. now add the titles.
            var cols = new[] { "IsPrimary", "SeriesId", "Title" };
            var items = item.Titles.Select(title => new object[] { title.IsPrimary, seriesId, title.Title });

            await ExecAsync(db => db.Query(_titlesTableName)
                .AsInsert(cols, items)
                .GetAsync());

            return seriesId;
        }
    }
}