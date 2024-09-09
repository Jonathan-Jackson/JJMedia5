using JJMedia5.Media.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JJMedia5.Tests.Functional {
    [TestClass]
    public class SeriesEpisodeSearchServiceTests : FunctionalTestBase {

        [TestMethod]
        [DataRow("[Anime Time] Komi-san wa, Comyushou desu. (Season 2) - 12 [NF][1080p][HEVC 10bit x265][Eng Sub] (Komi Can't Communicate).mkv")]
        [DataRow("[SubsPlease] Shinigami Bocchan to Kuro Maid - 22 (480p) [17230B83].mkv")]
        [DataRow("[SubsPlease] Cue! - 24 (1080p) [FDD6DFBA].mkv")]
        public async Task EpisodeNamesTest(string fileName)
        {
            var service = _provider.GetRequiredService<SeriesEpisodeSearchService>();
            var result = await service.FindAsync(fileName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.EpisodeId > 0);
        }
    }
}