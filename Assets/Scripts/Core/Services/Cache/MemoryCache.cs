using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;

public class MemoryCache : ICache
{
    private readonly ConcurrentDictionary<string, byte[]> cache 
        = new ConcurrentDictionary<string, byte[]>();

    public async UniTask<byte[]> FetchAsync(string url)
    {
        cache.TryGetValue(url, out var data);
        return data;
    }

    public async UniTask StoreAsync(string url, byte[] data)
    {
        cache[url] = data;
    }
}