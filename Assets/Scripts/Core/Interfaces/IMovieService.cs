using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IMovieService
{
    UniTask<List<MovieData>> SearchMoviesAsync(string query, CancellationTokenSource cts = null);
    UniTask<MovieData> GetMovieDetailsAsync(int movieId, CancellationTokenSource cts = null);
    
    UniTask<Texture2D> GetMoviePosterAsync(string posterPath, CancellationTokenSource cts = null);
}
