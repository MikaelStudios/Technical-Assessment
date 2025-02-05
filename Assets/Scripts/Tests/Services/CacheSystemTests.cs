using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class CacheSystemTests
{
    private Mock<ICache> mockCache;
    private CacheSystem cacheSystem;
    private HttpClient httpClient;

    [SetUp]
    public void SetUp()
    {
        mockCache = new Mock<ICache>();
        httpClient = new HttpClient();
        cacheSystem = new CacheSystem(mockCache.Object);
    }

    [Test]
    public async Task FetchStringAsync_ReturnsStringFromCache()
    {
        var url = "http://example.com"; //The URL doesnt matter, we are just testing the cache
        
        var cachedData = System.Text.Encoding.UTF8.GetBytes("cached data");
        mockCache.Setup(c => c.FetchAsync(url)).Returns(new UniTask<byte[]>(cachedData));

        var result = await cacheSystem.FetchStringAsync(url);

        Assert.AreEqual("cached data", result);
    }

    [Test]
    public async Task FetchStringAsync_DownloadsAndReturnsString()
    {
        var url = "http://example.com";
        var downloadedData = System.Text.Encoding.UTF8.GetBytes("downloaded data");
        mockCache.Setup(c => c.FetchAsync(url)).Returns(new UniTask<byte[]>((byte[])null)); // cache is empty
        
        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new ByteArrayContent(downloadedData)
            });
        httpClient = new HttpClient(httpMessageHandler.Object);
        cacheSystem = new CacheSystem(mockCache.Object, httpClient);

        var result = await cacheSystem.FetchStringAsync(url);

        Assert.AreEqual("downloaded data", result);
    }

    [Test]
    public async Task FetchTextureAsync_ReturnsTextureFromCache()
    {
        var url = "http://example.com";
        var cachedData = new byte[] { 0, 1, 2, 3 };
        mockCache.Setup(c => c.FetchAsync(url)).Returns(new UniTask<byte[]>(cachedData));

        var result = await cacheSystem.FetchTextureAsync(url);

        Assert.IsNotNull(result);
        Assert.AreEqual(url, result.name);
    }

    [Test]
    public async Task FetchTextureAsync_DownloadsAndReturnsTexture()
    {
        var url = "http://example.com";
        var downloadedData = new byte[] { 0, 1, 2, 3 };
        mockCache.Setup(c => c.FetchAsync(url)).Returns(new UniTask<byte[]>((byte[])null));
        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new ByteArrayContent(downloadedData)
            });
        httpClient = new HttpClient(httpMessageHandler.Object);
        cacheSystem = new CacheSystem(mockCache.Object, httpClient);

        var result = await cacheSystem.FetchTextureAsync(url);

        Assert.IsNotNull(result);
        Assert.AreEqual(url, result.name);
    }

}