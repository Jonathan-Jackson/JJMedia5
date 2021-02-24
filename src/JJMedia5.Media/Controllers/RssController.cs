using JJMedia5.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RssController : ControllerBase {

        // GET: api/<RssController>
        [HttpGet]
        public IEnumerable<RssFeed> Get() {
            return new[] { new RssFeed { Id = 0, Info = "PLACEHOLDER" } };
        }

        // GET api/<RssController>/5
        [HttpGet("{id}")]
        public RssFeed Get(int id) {
            return new RssFeed { Id = id, Info = "PLACEHOLDER" };
        }

        // POST api/<RssController>
        [HttpPost]
        public void Post([FromBody] object value) {
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