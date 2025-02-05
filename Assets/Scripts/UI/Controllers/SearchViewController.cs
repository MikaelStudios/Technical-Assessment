using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SearchViewController : MonoBehaviour
{
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private Transform resultsContainer;
    [SerializeField] private GameObject movieItemPrefab;
    [SerializeField] private GameObject loadingSpinner;
    
    private IMovieService movieService;

    [Header("API Key Validation")] [SerializeField]
    private UnityEvent onAuthStarted;
    [SerializeField] private UnityEvent onAuthSuccess;
    [SerializeField] private UnityEvent onAuthFailed;
    private CacheSystem _cacheSystem;
    private void Awake()
    {
        _cacheSystem = new CacheSystem(new FileCache("PersistentCache"));
        movieService = new TmdbMovieService("",
            _cacheSystem);
    }
    public async void Initialize(TMP_InputField inputField)
    {
        if(inputField.text == "") return;
        onAuthStarted.Invoke();
        await Task.Delay(TimeSpan.FromSeconds(2.5f));
        try
        {
            bool success = await ((TmdbMovieService)movieService).IsApiKeyValidAsync(inputField.text);
            if (success)
            {
                onAuthSuccess.Invoke();
                movieService = new TmdbMovieService(inputField.text, _cacheSystem);
            }
            else
            {
                throw new Exception();
            }
        }
        catch (Exception e)
        {
            onAuthFailed.Invoke();
        }
    }
    public void OnSearch()
    {
        OnSearchAsync().Forget();
    }
    async UniTaskVoid OnSearchAsync()
    {
        
        loadingSpinner.SetActive(true);
        ClearResults();
        await UniTask.Yield();
        try
        {
            var results = await movieService.SearchMoviesAsync(searchInput.text);
            foreach (var movie in results)
            {
                var item = Instantiate(movieItemPrefab, resultsContainer);
                item.GetComponent<MovieItemComponent>().Initialize(movieService, movie).Forget();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Search failed: " + ex.Message);
        }
        finally
        {
            loadingSpinner.SetActive(false);
        }
    }

    private void ClearResults()
    {
        foreach (Transform child in resultsContainer)
        {
            Destroy(child.gameObject);
        }
    }
}