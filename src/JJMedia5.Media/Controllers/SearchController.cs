using JJMedia5.Media.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/v1/[controller]")]
    public class SearchController : Controller {
        private readonly SeriesSearchService _seriesSearch;
        private readonly SeriesEpisodeSearchService _seriesEpisodeSearch;

        public SearchController(SeriesSearchService seriesSearch, SeriesEpisodeSearchService seriesEpisodeSearch) {
            _seriesSearch = seriesSearch;
            _seriesEpisodeSearch = seriesEpisodeSearch;
        }

        [HttpGet("series")]
        public async Task<IActionResult> Series([FromQuery] string title) {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Search Title cannot be null or empty.");

            var result = await _seriesSearch.FindAsync(title);
            if (result == null) return NotFound(title);
            else return Ok(result);
        }

        [HttpGet("seriesepisode")]
        public async Task<IActionResult> SeriesEpisode([FromQuery] string filename) {
            if (string.IsNullOrWhiteSpace(filename))
                return BadRequest("File name cannot be null or empty.");

            var result = await _seriesEpisodeSearch.FindAsync(filename);
            if (result == null) return NotFound(filename);
            else return Ok(result);
        }
    }
}