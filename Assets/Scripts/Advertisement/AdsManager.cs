using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Inject] private SignalBus signalBus;
    private RewardedAdLogic rewardedAdLogic;

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
        rewardedAdLogic = gameObject.GetComponent<RewardedAdLogic> ();
    }

    public void ShowRewardedAd ()
    {
        rewardedAdLogic.ShowAd ();
    }
}
