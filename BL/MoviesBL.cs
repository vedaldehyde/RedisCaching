using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedisCaching.Interfaces;
using RedisCaching.Services;
using RestSharp;

namespace RedisCaching.BL
{
    public class MoviesBL:IMoviesBL
    {
        private readonly IConfiguration _config;
        private readonly CacheConfigService _cacheService;

        public MoviesBL(IConfiguration config, CacheConfigService cacheService) 
        { 
            _config = config;
            _cacheService = cacheService;
        }

        public async Task<JObject> GetMoviesFromIDAsync(int id)
        {
            JObject obj = new();
            string url = $"{_config.GetSection("MoviesURL").Value}{id}/details/";
            string serializedData = string.Empty;

            try
            {
                var cachedMovie = await _cacheService.GetAsync(id.ToString());
                if (cachedMovie != null)
                {
                    return cachedMovie;
                }

                var client = new RestClient();
                var request = new RestRequest(url, Method.Get)
                    .AddQueryParameter("apiKey", _config.GetSection("APIKey").Value)
                    .AddQueryParameter("language", "en-US")
                    .AddQueryParameter("region", "US");

                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");

                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
                {
                    obj = JObject.Parse(response.Content);

                    // Save the successful response into cache
                    await _cacheService.SetAsync(id.ToString(), obj, TimeSpan.FromHours(1)); // Cache for 1 hour
                    return obj;
                }
                else
                {
                    obj.Add("status", "failed");
                    obj.Add("msg", $"API call failed. Status: {response.StatusCode}, Error: {response.ErrorMessage}");
                    return obj;
                }
            }
            catch (Exception ex) 
            {
                obj.Add("status", "failed");
                obj.Add("msg", $"Exception while fetching response: {ex.Message}");
                return obj;
            }
        }
    }
}
