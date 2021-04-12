using JJMedia5.Core.Attributes;
using JJMedia5.Core.Enums;
using System;

namespace JJMedia5.Core.Entities {

    [Repository(TableName = "Series")]
    public class Series : Entity {
        public SeriesTitle[] Titles { get; set; } = Array.Empty<SeriesTitle>();

        public string Description { get; set; }

        public DateTimeOffset? AirDate { get; set; }

        public DateTimeOffset AddedOn { get; set; } = DateTimeOffset.UtcNow;

        public string SourceId { get; set; }

        public eSeriesApi SourceApi { get; set; }

        public override object GetPropertyModel()
            => new {
                Description,
                AirDate,
                AddedOn,
                SourceId,
                SourceApi
            };
    }
}