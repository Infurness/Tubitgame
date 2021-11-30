using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Inject] private SignalBus signalBus;
    private RewardedAdLogic rewardedAdLogic;

    bool adLoaded;
    private void Awake ()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad (gameObject);
        }
        else
        {
            Destroy (this);
        }
    }
    private void Start ()
    {
        signalBus.Subscribe<RewardAdLoadedSignal> (AdLoaded);
        signalBus.Subscribe<StartShowingAdSignal> (UnloadAd);

        rewardedAdLogic = gameObject.GetComponent<RewardedAdLogic> ();
    }

    public void ShowRewardedAd ()
    {
        rewardedAdLogic.ShowAd ();
    }
    void AdLoaded ()
    {
        adLoaded = true;
    }
    void UnloadAd ()
    {
        adLoaded = false;
    }
    public bool IsAdLoaded ()
    {
        return adLoaded;
    }
}
