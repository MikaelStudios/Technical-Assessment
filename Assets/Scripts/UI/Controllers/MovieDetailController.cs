using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovieDetailController : MonoBehaviour
{
    public static MovieDetailController instance;
    
    private void Awake() => instance = this;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text overviewText;
    [SerializeField] private TMP_Text releaseDateText;
    [SerializeField] private Image posterImage;
    [SerializeField] private TMP_Text genreText;
    
    [Space]
    [SerializeField] private GameObject loadingSpinner;
    [SerializeField] private GameObject detailsPanel;

    private CancellationTokenSource cts;
    public async UniTaskVoid ShowMovieDetail(IMovieService movieService, MovieData movie)
    {
        cts = new CancellationTokenSource();
        detailsPanel.SetActive(true);
        loadingSpinner.SetActive(true);
        titleText.text = movie.title;
        overviewText.text = movie.overview;
        releaseDateText.text = movie.release_date;
        //UniTask.Create(async ()=>{Debug.Log("sa");}).Forget();    
        if (movie.genres == null)
        {
            MovieData movieD = await movieService.GetMovieDetailsAsync(movie.id, cts);
            genreText.text = string.Join(", ", movieD.genres.Where(x => x != null).Select(x => x.name));
        }
        posterImage.sprite = movie.posterSprite;
        loadingSpinner.SetActive(false);
        
    }

    private void OnDestroy()
    {
        if(cts == null) return;
        cts.Cancel();
        cts.Dispose();
    }

    public void Close()
    {
        detailsPanel.SetActive(false);
        titleText.text = "";
        overviewText.text = "";
        releaseDateText.text = "";
        genreText.text = "";
        posterImage.sprite = null;
        cts.Cancel();
        cts.Dispose();
    }
}
