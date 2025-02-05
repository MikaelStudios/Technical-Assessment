using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CacheSystem
{
    private readonly ICache _cache;
    private readonly HttpClient _httpClient;

    public CacheSystem(ICache cache, HttpClient httpClient = null)
    {
        this._cache = cache;
        this._httpClient = httpClient ?? new HttpClient();
    }

    public async UniTask<string> FetchStringAsync(string url, HttpRequestMessage httpRequest = null, CancellationTokenSource cts = null, bool strictlyFromHttp = false)
    {
        var data = !strictlyFromHttp ? await FetchAsync(url,httpRequest) : await HttpFetchBytes(url, httpRequest);
        return System.Text.Encoding.UTF8.GetString(data);
    }
    
    public async UniTask<Texture> FetchTextureAsync(string url, HttpRequestMessage httpRequest = null, CancellationTokenSource cts = null)
    {
        var data = await FetchAsync(url,httpRequest);
        var texture = new Texture2D(0, 0)
        {
            filterMode = FilterMode.Bilinear,
            name = url
        };
        texture.LoadImage(data);
        return texture;
    }
    async UniTask<byte[]> FetchAsync(string url, HttpRequestMessage httpRequest = null, CancellationTokenSource cts = null)
    {
        try
        {
            var data = await _cache.FetchAsync(url);
            if (data != null)
            {
                return data;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Cache fetch error: {ex.Message}");
        }
        

        // Not found in cache, download
        try
        {
            var data = await HttpFetchBytes(url, httpRequest);

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Download failed: {ex.Message}");
            throw;
        }
    }
    
    async UniTask<byte[]> HttpFetchBytes(string url, HttpRequestMessage httpRequest)
    {
        var response = await _httpClient.SendAsync(httpRequest ?? new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
        });
        var data = await response.Content.ReadAsByteArrayAsync();
            
        _ = _cache.StoreAsync(url, data);
        return data;
    }
}