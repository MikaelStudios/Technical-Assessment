using Cysharp.Threading.Tasks;

public interface ICache
{
    UniTask<byte[]> FetchAsync(string url);
    UniTask StoreAsync(string url, byte[] data);
}