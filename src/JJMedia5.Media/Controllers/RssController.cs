using JJMedia5.Core.Entities;
using JJMedia5.Media.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RssController : ControllerBase {
        private readonly RssRepository _repo;

        // GET: api/<RssController>
        [HttpGet]
        public IEnumerable<RssFeed> Get() {
            return new[] { new RssFeed { Id = 0, Info = "PLACEHOLDER" } };
        }

        // GET api/<RssController>/5
        [HttpGet("{id}")]
        public async Task<RssFeed> Get(int id)
            => await _repo.GetAsync<RssFeed>(id);

        // POST api/<RssController>
        [HttpPost]
        public async Task<int> Post([FromBody] RssFeed value) {
            // validate
            return await _repo.AddAsync(value);
        }

        // PUT api/<RssController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] object value) {
        }

        // DELETE api/<RssController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}