using JJMedia5.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace JJMedia5.Tests.Functional {
    [TestClass]
    public class SeriesEpisodeSearchServiceTests : FunctionalTestBase {

        [TestMethod]
        [DataRow("[Anime Time] Komi-san wa, Comyushou desu. (Season 2) - 12 [NF][1080p][HEVC 10bit x265][Eng Sub] (Komi Can't Communicate).mkv")]
        [DataRow("[SubsPlease] Shinigami Bocchan to Kuro Maid - 22 (480p) [17230B83].mkv")]
        public async Task EpisodeNamesTest(string fileName)
        {
            var service = _provider.GetRequiredService<ISeriesEpisodeSearchService>();
            var result = await service.FindAsync(fileName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.EpisodeId > 0);
        }
    }
}