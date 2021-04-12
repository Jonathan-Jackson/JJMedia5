using JJMedia5.Core.Entities;
using System.Linq;

namespace JJMedia5.Core.Models {

    public class EpisodeSeriesInfo {

        public EpisodeSeriesInfo() {
        }

        public EpisodeSeriesInfo(Episode episode, Series series) {
            EpisodeId = episode.Id;
            EpisodeNumber = episode.EpisodeNumber;
            EpisodeSeason = episode.SeasonNumber;
            EpisodeTitle = episode.Title;

            SeriesId = series.Id;
            SeriesTitle = series.Titles.First(c => c.IsPrimary).Title;
        }

        public int EpisodeId { get; set; }
        public int EpisodeNumber { get; set; }
        public int EpisodeSeason { get; set; }
        public string EpisodeTitle { get; set; }
        public int SeriesId { get; set; }
        public string SeriesTitle { get; set; }
    }
}