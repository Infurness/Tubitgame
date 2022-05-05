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

    [SerializeField] float rewardsStartDelaySeconds;
    [SerializeField] float rewardsSecondsFrequency;
    // Start is called before the first frame update
    void Start()
    {
        if(TutorialManager.Instance==null)
            InvokeRepeating ("RandomReward", rewardsStartDelaySeconds, rewardsSecondsFrequency);
    }

    public int GetSoftCurrencyBonus ()
    {
       return 10 * (xpManager.GetPlayerLevel ()+1);
    }

    public void SoftCurrencyRewardNoAd ()
    {
        PlayerDataManager.Instance.AddSoftCurrency ((ulong)GetSoftCurrencyBonus ()*5);
        signalBus.Fire<UpdateSoftCurrencySignal> ();
    }

    public void InitialSoftCurrencyReward ()
    {
        PlayerDataManager.Instance.AddSoftCurrency ((ulong)GetSoftCurrencyBonus ());
    }
    public void SoftCurrencyReward ()
    {
        signalBus.Subscribe<GrantRewardSignal> (SoftCurrencyAdCompletedReward);
        signalBus.Subscribe<NotGrantedRewardSignal> (NoSoftCurrencyReward);
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

    public int GetHardCurrencyBonus ()
    {
        return 10 * (xpManager.GetPlayerLevel () + 1);
    }
    public void HardCurrencyRewardNoAds ()
    {
        PlayerDataManager.Instance.AddHardCurrency (GetHardCurrencyBonus ()*2);
        signalBus.Fire<UpdateHardCurrencySignal> ();
    }
    public void InitialHardCurrencyReward ()
    {
        PlayerDataManager.Instance.AddHardCurrency (GetHardCurrencyBonus ());
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

    public float GetThemeBonusReward ( int rewardValue)
    {
        return (0.1f * xpManager.GetPlayerLevel ())/ rewardValue;
    }
    public void ThemeBonusRewardNoAds ()
    {
        algorithmManager.SetThemeBonus (GetThemeBonusReward (1));
    }
    public void InitialThemeBonusReward ()
    {
        algorithmManager.SetThemeBonus (GetThemeBonusReward (2));
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
        signalBus.TryUnsubscribe<GrantRewardSignal> (ThemeBonusAdCompletedReward);
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (NoThemeBonusReward);
    }

    public int GetEnergyBonus ()
    {
        return 15;
    }
    public void EnergyRewardNoAds ()
    {
        signalBus.Fire<AddEnergySignal> (new AddEnergySignal { energyAddition = GetEnergyBonus () * 2 });
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

    void VideoShorterProdTime (UnpublishedVideo video)
    {
        float videoQualityValue = (2 / Enum.GetValues (typeof (VideoQuality)).Length) * (int)video.videoQuality;
        int[] shortenVideoValues = { 10,37,75,112,150,187,225,259,276,300,356,390,410,435,450,506,539,572,580,600};
        int timeReduced = 10;
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
        video.secondsToBeProduced -= timeReduced;
    }


    public void VideoShortenRewardNoAds (UnpublishedVideo video)
    {
        VideoShorterProdTime (video);
    }


    public void VideoShortenReward (UnpublishedVideo video)
    {
        signalBus.Subscribe<GrantRewardSignal> (()=>VideoShortenAdCompletedReward(video));
        signalBus.Subscribe<NotGrantedRewardSignal> (()=>NoVideoShortenReward(video));
        AdsManager.Instance.ShowRewardedAd ();
    }

    void VideoShortenAdCompletedReward (UnpublishedVideo video)
    {
        VideoShorterProdTime (video);
        NoVideoShortenReward (video);
    }
    void NoVideoShortenReward (UnpublishedVideo video)
    {
        signalBus.TryUnsubscribe<GrantRewardSignal> (()=>VideoShortenAdCompletedReward(video));
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (()=>NoVideoShortenReward(video));
    }

    public void ViewsBonusRewardNoAds ()
    {
        algorithmManager.SetViewsBonus (2);
    }

    public void ViewsBonusReward ()
    {
        signalBus.TryUnsubscribe<GrantRewardSignal> (ViewsBonusAdCompletedReward);
        signalBus.TryUnsubscribe<NotGrantedRewardSignal> (NoViewsBonusReward);
        signalBus.Subscribe<GrantRewardSignal> (ViewsBonusAdCompletedReward);
        signalBus.Subscribe<NotGrantedRewardSignal> (NoViewsBonusReward);
        if(TutorialManager.Instance==null)
            AdsManager.Instance.ShowRewardedAd ();
        else//Tutorial
        {
            signalBus.Fire<GrantRewardSignal>();
            signalBus.Fire<OnHitConfirmAdButtonSignal>();
        }
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
        signalBus.Fire<FinishedAdVisualitationRewardSignal> ();
    }

    void RandomReward()
    {
        int randomNumber = UnityEngine.Random.Range (0,101);
        if (randomNumber < 40)
        {
            signalBus.Fire<OpenSoftCurrencyAdSignal> ();
        }
        else if(randomNumber < 75)
        {
            signalBus.Fire<OpenThemeBonusAdSignal> ();
        }
        else
        {
            signalBus.Fire<OpenHardCurrencyAdSignal> ();
        }
    }
}
