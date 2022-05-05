using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using Zenject;

public class RewardedAdLogic : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsListener
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
        Advertisement.Load (_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded (string adUnitId)
    {
        if (adUnitId.Equals (_adUnitId))
        {
            signalBus.Fire<RewardAdLoadedSignal> ();
        }
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd ()
    {
        GameAnalytics.NewAdEvent(GAAdAction.Clicked,GAAdType.Video,"UnityAds",_adUnitId);
        signalBus.Fire<StartShowingAdSignal> ();
        Advertisement.Show (_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete (string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals (_adUnitId) && showCompletionState.Equals (UnityAdsShowCompletionState.COMPLETED))
        {
            GameAnalytics.NewDesignEvent("ad_rv_success");
            GameAnalytics.NewAdEvent(GAAdAction.RewardReceived,GAAdType.Video,"UnityAds",_adUnitId);

            signalBus.Fire<GrantRewardSignal> ();
        }
        else if (adUnitId.Equals (_adUnitId) && showCompletionState.Equals (UnityAdsShowCompletionState.SKIPPED))
        {
            GameAnalytics.NewDesignEvent("ad_rv_skip");
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow,GAAdType.Video,"UnityAds",_adUnitId);


            signalBus.Fire<NotGrantedRewardSignal> ();
        }else
        {
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow,GAAdType.Video,"UnityAds",_adUnitId);

            GameAnalytics.NewDesignEvent("ad_retry");
            Debug.LogWarning("Unity Ads Rewarded Ad NOT Completed");
            signalBus.Fire<NotGrantedRewardSignal> ();
        }
        Advertisement.Load (_adUnitId, this);
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad (string adUnitId, UnityAdsLoadError error, string message)
    {
        GameAnalytics.NewDesignEvent("ad_rv_fail");
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow,GAAdType.Video,"UnityAds",_adUnitId);

        Debug.LogError ($"Error loading Ad Unit {adUnitId}: {error.ToString ()} - {message}");
        signalBus.Fire<NotGrantedRewardSignal> ();
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure (string adUnitId, UnityAdsShowError error, string message)
    {
        GameAnalytics.NewDesignEvent("ad_rv_fail");
        GameAnalytics.NewAdEvent(GAAdAction.FailedShow,GAAdType.Video,"UnityAds",_adUnitId);

        Debug.LogError ($"Error showing Ad Unit {adUnitId}: {error.ToString ()} - {message}");
        signalBus.Fire<NotGrantedRewardSignal> ();
    }

    public void OnUnityAdsShowStart (string adUnitId) { Debug.Log ("Start Showing "); }
    public void OnUnityAdsShowClick (string adUnitId) { }

    void OnDestroy ()
    {
    }

    public void OnUnityAdsReady (string placementId)
    {
    }

    public void OnUnityAdsDidError (string message)
    {
    }

    public void OnUnityAdsDidStart (string placementId)
    {
    }

    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult)
    {
    }
}
