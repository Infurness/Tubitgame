using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Ads_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] AdsRewardsManager adsRewardsManager;

    [SerializeField] Button showAdButton;
    bool isAdLoaded;
    // Start is called before the first frame update
    void Start()
    {
        showAdButton.onClick.AddListener (adsRewardsManager.SoftCurrencyReward);
        signalBus.Subscribe<RewardAdLoadedSignal> (EnableAdButton);
        signalBus.Subscribe<StartShowingAdSignal> (DisableAddButton);
    }
    void EnableAdButton ()
    {
        showAdButton.interactable = true;
    }
    void DisableAddButton ()
    {
        showAdButton.interactable = false;
    }
}
