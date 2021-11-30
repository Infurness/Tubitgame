using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using Zenject;

public class RewardedAdLogic : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Inject] SignalBus signalBus;

    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOsAdUnitId = "Rewarded_iOS";
    string _adUnitId;

    void Awake ()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

        //Disable button until ad is ready to show
        //_showAdButton.interactable = false;
        DontDestroyOnLoad (gameObject);
    }

    private void Start ()
    {
        signalBus.Subscribe<AdsInitializedSignal> (LoadAd);
    }
    // Load content to the Ad Unit:
    public void LoadAd ()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log ("Loading Ad: " + _adUnitId);
        Advertisement.Load (_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded (string adUnitId)
    {
        Debug.Log ("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals (_adUnitId))
        {
            Debug.Log ("Ad Ready to be shown");
            signalBus.Fire<RewardAdLoadedSignal> ();
            // Configure the button to call the ShowAd() method when clicked:
            //_showAdButton.onClick.AddListener (ShowAd);
            // Enable the button for users to click:
            // _showAdButton.interactable = true;
        }
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd ()
    {
        Debug.Log ("Start showing ad from logic");
        // Disable the button: 
        //_showAdButton.interactable = false;
        signalBus.Fire<StartShowingAdSignal> ();
        // Then show the ad:
        Advertisement.Show (_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete (string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log ("Show completed");
        if (adUnitId.Equals (_adUnitId) && showCompletionState.Equals (UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log ("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            signalBus.Fire<GrantRewardSignal> ();
            // Load another ad:
            
        }
        else
        {
            signalBus.Fire<NotGrantedRewardSignal> ();
        }
        Advertisement.Load (_adUnitId, this);
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad (string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log ($"Error loading Ad Unit {adUnitId}: {error.ToString ()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure (string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log ($"Error showing Ad Unit {adUnitId}: {error.ToString ()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart (string adUnitId) { Debug.Log ("Start Showing "); }
    public void OnUnityAdsShowClick (string adUnitId) { }

    void OnDestroy ()
    {
        // Clean up the button listeners:
        //_showAdButton.onClick.RemoveAllListeners ();
    }
}
