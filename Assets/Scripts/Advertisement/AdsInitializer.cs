using UnityEngine;
using UnityEngine.Advertisements;
using Zenject;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [Inject] SignalBus signalBus;

    [SerializeField] string _androidGameId;
    [SerializeField] string _iOsGameId;
    [SerializeField] bool _testMode = true;
    [SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId;

    void Awake ()
    {
        DontDestroyOnLoad (this.gameObject);
        InitializeAds ();
    }

    public void InitializeAds ()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsGameId
            : _androidGameId;
        Advertisement.Initialize (_gameId, _testMode, _enablePerPlacementMode, this);
    }

    public void OnInitializationComplete ()
    {
        Debug.Log ("Unity Ads initialization complete.");
        signalBus.Fire<AdsInitializedSignal> ();
    }

    public void OnInitializationFailed (UnityAdsInitializationError error, string message)
    {
        Debug.Log ($"Unity Ads Initialization Failed: {error.ToString ()} - {message}");
    }
}