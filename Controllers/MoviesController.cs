using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using RedisCaching.BL;
using RedisCaching.Services;

namespace RedisCaching.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly CacheConfigService _cacheService;

        public MoviesController(IConfiguration config, CacheConfigService cacheService)
        {
            _config = config;
            _cacheService = cacheService;
        }

        [Route("/api/Movies/GetMoviesFromID")]
        [HttpPost]
        public async Task<IActionResult> GetMoviesFromID([FromBody]int id)
        {
            MoviesBL moviesBL = new(_config, _cacheService);
            try
            {
                JObject res = await moviesBL.GetMoviesFromIDAsync(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
