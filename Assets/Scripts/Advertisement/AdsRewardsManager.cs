using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AdsRewardsManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] ExperienceManager xpManager;
    [Inject] AlgorithmManager algorithmManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    int GetSoftCurrencyBonus ()
    {
       return 10 * (xpManager.GetPlayerLevel ()+1);
    }

    public void SoftCurrencyReward ()
    {
        signalBus.Subscribe<GrantRewardSignal> (SoftCurrencyAdCompletedReward);
        signalBus.Subscribe<NotGrantedRewardSignal> (NoSoftCurrencyReward);
        PlayerDataManager.Instance.AddSoftCurrency ((ulong)GetSoftCurrencyBonus ());
        signalBus.Fire<UpdateSoftCurrencySignal> ();
        AdsManager.Instance.ShowRewardedAd ();
    }

    void SoftCurrencyAdCompletedReward ()
    {
        PlayerDataManager.Instance.AddSoftCurrency ((ulong)GetSoftCurrencyBonus ()*4);
        signalBus.Fire<UpdateSoftCurrencySignal> ();
        NoSoftCurrencyReward ();
    }
    void NoSoftCurrencyReward ()
    {
        signalBus.TryUnsubscribe<GrantRewardSignal> (SoftCurrencyAdCompletedReward);
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (NoSoftCurrencyReward);
    }

    int GetHardCurrencyBonus ()
    {
        return 10 * (xpManager.GetPlayerLevel () + 1);
    }

    public void HardCurrencyReward ()
    {
        signalBus.Subscribe<GrantRewardSignal> (HardCurrencyAdCompletedReward);
        signalBus.Subscribe<NotGrantedRewardSignal> (NoHardCurrencyReward);
        PlayerDataManager.Instance.AddHardCurrency (GetHardCurrencyBonus());
        signalBus.Fire<UpdateHardCurrencySignal> ();
        AdsManager.Instance.ShowRewardedAd ();
    }

    void HardCurrencyAdCompletedReward ()
    {
        PlayerDataManager.Instance.AddHardCurrency (GetHardCurrencyBonus ()*2);
        signalBus.Fire<UpdateHardCurrencySignal> ();
        NoHardCurrencyReward ();
    }
    void NoHardCurrencyReward ()
    {
        signalBus.TryUnsubscribe<GrantRewardSignal> (HardCurrencyAdCompletedReward);
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (NoHardCurrencyReward);
    }

    float GetThemeBonusReward ( int rewardValue)
    {
        return (0.1f * xpManager.GetPlayerLevel ())/ rewardValue;
    }

    public void ThemeBonusReward ()
    {
        signalBus.Subscribe<GrantRewardSignal> (ThemeBonusAdCompletedReward);
        signalBus.Subscribe<NotGrantedRewardSignal> (NoThemeBonusReward);
        AdsManager.Instance.ShowRewardedAd ();
    }

    void ThemeBonusAdCompletedReward ()
    {
        algorithmManager.SetThemeBonus (GetThemeBonusReward (1));
        NoThemeBonusReward ();
    }
    void NoThemeBonusReward ()
    {
        algorithmManager.SetThemeBonus (GetThemeBonusReward (2));
        signalBus.TryUnsubscribe<GrantRewardSignal> (ThemeBonusAdCompletedReward);
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (NoThemeBonusReward);
    }

    int GetEnergyBonus ()
    {
        return 15;
    }

    public void EnergyReward ()
    {
        signalBus.Subscribe<GrantRewardSignal> (EnergyAdCompletedReward);
        signalBus.Subscribe<NotGrantedRewardSignal> (NoEnergyReward);

        AdsManager.Instance.ShowRewardedAd ();
    }

    void EnergyAdCompletedReward ()
    {
        signalBus.Fire<AddEnergySignal> (new AddEnergySignal { energyAddition = GetEnergyBonus () * 2 });
        NoEnergyReward ();
    }
    void NoEnergyReward ()
    {
        signalBus.TryUnsubscribe<GrantRewardSignal> (EnergyAdCompletedReward);
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (NoEnergyReward);
    }

    void VideoShorterProdTime (Video video)
    {
        float videoQualityValue = (2 / Enum.GetValues (typeof (VideoQuality)).Length) * (int)video.selectedQuality;
        int[] shortenVideoValues = { 10,37,75,112,150,187,225,259,276,300,356,390,410,435,450,506,539,572,580,600};
        int timeReduced = 0;
        float qualityLoop = 0.1f;
        foreach(int shortenValue in shortenVideoValues)
        {
            if(qualityLoop>=videoQualityValue)
            {
                timeReduced = shortenValue;
                break;
            }
            qualityLoop += 0.1f;
        }
        //video.timeToBeProduced -= timeReduced;
    }

    public void VideoShortenReward (Video video)
    {
        signalBus.Subscribe<GrantRewardSignal> (()=>VideoShortenAdCompletedReward(video));
        signalBus.Subscribe<NotGrantedRewardSignal> (()=>NoVideoShortenReward(video));
        AdsManager.Instance.ShowRewardedAd ();
    }

    void VideoShortenAdCompletedReward (Video video)
    {
        VideoShorterProdTime (video);
        NoVideoShortenReward (video);
    }
    void NoVideoShortenReward (Video video)
    {
        signalBus.TryUnsubscribe<GrantRewardSignal> (()=>VideoShortenAdCompletedReward(video));
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (()=>NoVideoShortenReward(video));
    }

    public void ViewsBonusReward ()
    {
        signalBus.Subscribe<GrantRewardSignal> (ViewsBonusAdCompletedReward);
        signalBus.Subscribe<NotGrantedRewardSignal> (NoViewsBonusReward);
        AdsManager.Instance.ShowRewardedAd ();
    }

    void ViewsBonusAdCompletedReward ()
    {
        algorithmManager.SetViewsBonus (2);
        NoViewsBonusReward ();
    }
    void NoViewsBonusReward ()
    {
        signalBus.TryUnsubscribe<GrantRewardSignal> (ViewsBonusAdCompletedReward);
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (NoViewsBonusReward);
    }
}