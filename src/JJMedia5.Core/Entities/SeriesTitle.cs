using JJMedia5.Core.Attributes;

namespace JJMedia5.Core.Entities {

    [Repository(TableName = "SeriesTitles")]
    public class SeriesTitle : Entity {

        public SeriesTitle() {
        }

        public SeriesTitle(string title, bool isPrimary) {
            Title = title;
            IsPrimary = isPrimary;
        }

        public int SeriesId { get; set; }

        public string Title { get; set; }

        public bool IsPrimary { get; set; }

        public override object GetPropertyModel()
            => new {
                SeriesId,
                Title,
                IsPrimary
            };
    }
}