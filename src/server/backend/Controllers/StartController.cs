using JJMedia5.Server.backend.Context;
using JJMedia5.Server.backend.Context.Entity;
using JJMedia5.Server.backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JJMedia5.Server.backend.Controllers {

    [Route("api/v1/[controller]")]
    [ApiController]
    public class StartController : ControllerBase {
        private readonly JJMedia5Context _db;

        public StartController(JJMedia5Context db) {
            _db = db;
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status() {
            await _db.Database.EnsureCreatedAsync();

            // check if the db is setup.
            bool isSetup = await _db.Users.AnyAsync();

            return Ok(new { isSetup });
        }

        [HttpPost("setup")]
        public async Task<IActionResult> Setup([FromBody] SetupModel model) {
            await _db.AddAsync(new User(model.Username));
            await _db.AddAsync(model.TorrentClient);
            await _db.AddRangeAsync(model.Rss);
            await _db.AddRangeAsync(model.Stores);
            await _db.AddRangeAsync(model.Imports);

            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}