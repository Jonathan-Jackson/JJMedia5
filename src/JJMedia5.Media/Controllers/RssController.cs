using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RssController : ControllerBase {

        // GET: api/<RssController>
        [HttpGet]
        public IEnumerable<object> Get() {
            return new[] { new { id = 0, title = "placeholder" } };
        }

        // GET api/<RssController>/5
        [HttpGet("{id}")]
        public object Get(int id) {
            return new { id, title = "placeholder" };
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