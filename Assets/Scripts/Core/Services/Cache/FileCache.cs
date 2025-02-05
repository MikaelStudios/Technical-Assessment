
using System;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FileCache : ICache
{
    private readonly string _cacheRoot;

    public FileCache(string cacheDirectory)
    {
        _cacheRoot = Path.Combine(Application.persistentDataPath, cacheDirectory);
        Directory.CreateDirectory(_cacheRoot);
    }

    public async UniTask<byte[]> FetchAsync(string url)
    {
        var path = GetCachePath(url);
        if (!File.Exists(path)) return null;

        // Verify URL matches stored data
        var metaPath = GetMetaPath(url);
        if (!File.Exists(metaPath)) return null;
    
        var storedUrl = await File.ReadAllTextAsync(metaPath);
        if (storedUrl != url) return null;

        return await File.ReadAllBytesAsync(path);
    }

    public async UniTask StoreAsync(string url, byte[] data)
    {
        var path = GetCachePath(url);
        var metaPath = GetMetaPath(url);
        //Debug.Log($"Stored Data {url} IN {path}");
        await File.WriteAllBytesAsync(path, data);
        await File.WriteAllTextAsync(metaPath, url); // Store original URL as metadata
    }

    private string GetCachePath(string url)
    {
        var hash = ComputeSHA1(url);
        return Path.Combine(_cacheRoot, $"{hash}.cache");
    }

    private string GetMetaPath(string url)
    {
        var hash = ComputeSHA1(url);
        return Path.Combine(_cacheRoot, $"{hash}.meta");
    }

    private string ComputeSHA1(string input)
    {
        using var sha1 = SHA1.Create();
        var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
