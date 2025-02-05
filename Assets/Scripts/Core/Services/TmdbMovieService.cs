using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TmdbMovieService : IMovieService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    
    private readonly string _baseUrl = "https://api.themoviedb.org/3/";
    private CacheSystem _cache;
    public TmdbMovieService(string apiKey, CacheSystem cache)
    {
        this._apiKey = apiKey;
        this._httpClient = new HttpClient();

        this._cache = cache;
    }
    public async UniTask<bool> IsApiKeyValidAsync(string apiKey, CancellationTokenSource cts = null)
    {
        try
        {
            var url = $"{_baseUrl}authentication";
            var response = await _cache.FetchStringAsync(url, new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {apiKey}" },
                },
            },cts, true);
            var success = JObject.Parse(response)["success"]?.ToObject<bool>();
            return success != null && success.Value;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to validate API key: {ex.Message}");
            return false;
        }
    }

    public async UniTask<List<MovieData>> SearchMoviesAsync(string query, CancellationTokenSource cts = null)
    {
        if(string.IsNullOrEmpty(query)) return new List<MovieData>();
        try
        {
            var url = $"{_baseUrl}/search/movie?query={Uri.EscapeDataString(query)}&language=en-US&page=1";
            var response = await _cache.FetchStringAsync(url, new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {_apiKey}" },
                },
            },cts);

            var result = JObject.Parse(response);
            return result["results"]?.ToObject<List<MovieData>>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Search failed: {ex.Message}");
            throw new Exception("Failed to search movies", ex);
        }
        
    }
    public async UniTask<MovieData> GetMovieDetailsAsync(int movieId, CancellationTokenSource cts = null)
    {
        if(movieId <= 0) return null;

        try
        {
            var url = $"{_baseUrl}/movie/{movieId}";
            
            var response = await _cache.FetchStringAsync(url, new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {_apiKey}" },
                },
            },cts);
            
            var movie = JObject.Parse(response).ToObject<MovieData>();
            return movie;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get movie details: {ex.Message}");
            throw new Exception("Failed to retrieve movie details", ex);
        }
    }
    
    public async UniTask<Texture2D> GetMoviePosterAsync(string posterPath, CancellationTokenSource cts = null)
    {
        if(string.IsNullOrEmpty(posterPath)) return null;
        
        try
        {
            var response = await _cache.FetchTextureAsync($"https://image.tmdb.org/t/p/original{posterPath}");
            if (response == null) return null;
            
            return response as Texture2D;
        }
        catch (Exception ex)
        {
            throw new Exception($"$Failed to retrieve movie poster {posterPath}", ex);
        }
    }
}
