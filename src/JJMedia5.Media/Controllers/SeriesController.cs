using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/v1/[controller]")]
    public class SeriesController : EntityController<Series> {
        private readonly SeriesRepository _seriesRepo;

        public SeriesController(SeriesRepository repo)
            : base(repo) {
            _seriesRepo = repo;
        }

        public async Task<IActionResult> Search([FromBody] string title) {
            return Ok(await _seriesRepo.FindIdByTitleName(title));
        }
    }
}