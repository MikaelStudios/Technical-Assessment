using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

[TestFixture]
public class TmdbMovieServiceTest
{

    private TmdbMovieService service;

    [SetUp]
    public void Setup()
    {
        service = new TmdbMovieService("eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJlMTc2YTY4ZjUwNGZiYWE4N2M4NzM4YWUwMDA1NjFiOSIsIm5iZiI6MTczODYwNTk5NC4xMzEsInN1YiI6IjY3YTEwNWFhNzE2YmZkNzRlY2UyYjFiMCIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.DW4qiTro7lLhQC2sTAziSC7TRsyW9X2dOoztupy0FK0", 
            new CacheSystem(new MemoryCache()));
    }

    [UnityTest]
    public IEnumerator SearchMovies() => UniTask.ToCoroutine(async ()=>
    {
        var results = await service.SearchMoviesAsync("test");
        Assert.IsNotEmpty(results);
    });

    [UnityTest]
    public IEnumerator GetMovieDetails() => UniTask.ToCoroutine(async () =>
    {
        var result = await service.GetMovieDetailsAsync(617120);
        Assert.IsNotNull(result);
        Assert.NotZero(result.id);
    });

    [UnityTest]
    public IEnumerator GetMoviePoster() => UniTask.ToCoroutine(async () =>
    {
        var movie = await service.GetMovieDetailsAsync(617120);
        var result = await service.GetMoviePosterAsync(movie.poster_path);
        Assert.IsNotNull(result);
    });
}
