using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RedisCaching.BL;

namespace RedisCaching.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IConfiguration _config;
        public MoviesController(IConfiguration config)
        {
            _config = config;
        }

        [Route("/api/Movies/GetMoviesFromID")]
        [HttpPost]
        public async Task<IActionResult> GetMoviesFromID(int id)
        {
            MoviesBL moviesBL = new(_config);
            JObject res = await moviesBL.GetMoviesFromIDAsync(id);
            return Ok(res);
        }
    }
}
