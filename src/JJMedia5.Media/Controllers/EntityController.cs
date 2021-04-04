using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using JJMedia5.Media;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JJMedia5.MediaSubscription.Controllers {

    [ApiController]
    public class EntityController<TEntity> : Controller where TEntity : Entity {
        protected readonly IRepository<TEntity> _repo;

        public EntityController(IRepository<TEntity> repo) {
            _repo = repo;
        }

        [HttpGet]
        public Task<ICollection<TEntity>> Get()
            => _repo.GetAsync();

        [HttpGet("{id}")]
        public Task<TEntity> Get(int id)
            => _repo.FindAsync(id);

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody] TEntity value) {
            return Ok(new {
                id = await _repo.AddAsync(value)
            });
        }

        [HttpPut]
        [ValidateModel]
        public async Task<IActionResult> Put([FromBody] TEntity value) {
            await _repo.UpdateAsync(value);
            return Ok();
        }

        [HttpDelete("{id}")]
        public Task Delete(int id) {
            return _repo.DeleteAsync(id);
        }
    }
}