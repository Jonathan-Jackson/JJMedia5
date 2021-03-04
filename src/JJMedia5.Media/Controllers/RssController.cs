using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/v1/[controller]")]
    public class RssController : EntityController<RssFeed> {

        public RssController(IRepository<RssFeed> repo)
            : base(repo) {
        }
    }
}