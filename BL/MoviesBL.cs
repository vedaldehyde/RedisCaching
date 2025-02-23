using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedisCaching.Interfaces;
using RestSharp;

namespace RedisCaching.BL
{
    public class MoviesBL:IMoviesBL
    {
        private readonly IConfiguration _config;
        public MoviesBL(IConfiguration config) 
        { 
            _config = config;
        }

        public async Task<JObject> GetMoviesFromIDAsync(int id)
        {
            JObject obj = new();
            string url = $"{_config.GetSection("MoviesURL").Value}{id}/details/";
            try
            {
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
                obj.Add("msg", $"Exception while fetching response: {ex}");
                return obj;
            }
        }
    }
}
