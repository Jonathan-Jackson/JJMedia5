using JJMedia5.Media.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/v1/[controller]")]
    public class SearchController : Controller {
        private readonly SeriesSearchService _seriesSearch;

        public SearchController(SeriesSearchService seriesSearch) {
            _seriesSearch = seriesSearch;
        }

        [HttpGet("series")]
        public async Task<IActionResult> Series([FromQuery] string title) {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Search Title cannot be null or empty.");

            var result = await _seriesSearch.FindAsync(title);
            if (result == null) return NotFound(title);
            else return Ok(result);
        }
    }
}