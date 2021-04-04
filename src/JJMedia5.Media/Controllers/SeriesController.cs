using JJMedia5.Core.Database;
using JJMedia5.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace JJMedia5.MediaSubscription.Controllers {

    [Route("api/v1/[controller]")]
    public class SeriesController : EntityController<Series> {

        public SeriesController(IRepository<Series> repo)
            : base(repo) {
        }
    }
}