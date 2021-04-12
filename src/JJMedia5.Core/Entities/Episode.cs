using JJMedia5.Core.Attributes;
using JJMedia5.Core.Enums;
using System;

namespace JJMedia5.Core.Entities {

    [Repository(TableName = "Episodes")]
    public class Episode : Entity {
        public DateTimeOffset AddedOn { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? AiredOn { get; set; }
        public string Description { get; set; }
        public int EpisodeNumber { get; set; }
        public int SeasonNumber { get; set; }
        public int SeriesId { get; set; }
        public eSeriesApi SourceApi { get; set; }
        public string SourceId { get; set; }
        public string Title { get; set; }

        public override object GetPropertyModel()
            => new {
                Title,
                SeasonNumber,
                EpisodeNumber,
                AiredOn,
                SeriesId,
                AddedOn,
                SourceId,
                SourceApi,
                Description
            };
    }
}