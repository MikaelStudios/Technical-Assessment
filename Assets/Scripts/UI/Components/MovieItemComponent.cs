using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovieItemComponent : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text overviewText;
    [SerializeField] private TMP_Text releaseDateText;
    [SerializeField] private Image posterImage;
    [SerializeField] private Button detailsButton;
    private IMovieService _movieService;
    
    CancellationTokenSource _cts;
    public async UniTaskVoid Initialize(IMovieService movieService, MovieData movie)
    { 
        _cts = new CancellationTokenSource();
        _movieService = movieService;
        detailsButton.onClick.AddListener(() => MovieDetailController.instance.ShowMovieDetail(movieService, movie).Forget());
        
        titleText.text = movie.title;
        overviewText.text = movie.overview;
        releaseDateText.text = movie.release_date;
        var texture = await movieService.GetMoviePosterAsync(movie.poster_path,cts: _cts);
        if (texture != null)
        {
            if(posterImage == null) return;
            posterImage.sprite =Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0.5f,0.5f));
            movie.posterSprite = posterImage.sprite;
        } 
        
    }

    private void OnDestroy()
    {
        if(_cts == null) return;
        _cts.Cancel();
        _cts.Dispose();
    }
}