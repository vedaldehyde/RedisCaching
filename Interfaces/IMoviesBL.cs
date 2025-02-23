using Newtonsoft.Json.Linq;

namespace RedisCaching.Interfaces
{
    public interface IMoviesBL
    {
        Task<JObject> GetMoviesFromIDAsync(int id);
    }
}
