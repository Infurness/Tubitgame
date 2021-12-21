using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Ads_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] AdsRewardsManager adsRewardsManager;

    [SerializeField] Button adPopUpConfirmButton;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenSoftCurrencyAdSignal> (SoftCurrencyAd);
        signalBus.Subscribe<OpenHardCurrencyAdSignal> (HardCurrencyAd);
        signalBus.Subscribe<OpenThemeBonusAdSignal> (ThemeBonusAd);
        signalBus.Subscribe<OpenEnergyAdSignal> (EnergyAd);
        signalBus.Subscribe<OpenTimeShortenAdSignal> (VideoShortenAd);
        signalBus.Subscribe<OpenDoubleViewsAdSignal> (DoubleViewsAd);
    }

    void ClosePopUp ()
    {
        signalBus.Fire<CloseAdsDefaultPopUpSignal> ();
    }
    void SoftCurrencyAd()
    {
        if (AdsManager.Instance.AreAdsDeactive())
            OpenSoftCurrencyNoAd();
        else
            OpenSoftCurrencyAd();
    }
    void OpenSoftCurrencyNoAd ()
    {
        signalBus.Fire (new OpenDefaultMessagePopUpSignal { message = $"You have been granted with <b><color=green>{adsRewardsManager.GetSoftCurrencyBonus ()*5}</color></b> TubeDollars!!"});
        adsRewardsManager.SoftCurrencyRewardNoAd();
    }
    void OpenSoftCurrencyAd ()
    {
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { message = $"You have been rewarded with <b><color=green>{adsRewardsManager.GetSoftCurrencyBonus()}</color></b> TubeDollars!!\nWatch an Ad to recieve <b><color=#CFCD00FF>{adsRewardsManager.GetSoftCurrencyBonus ()*4}</color></b> more?" });
        adsRewardsManager.InitialSoftCurrencyReward ();
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adPopUpConfirmButton.onClick.AddListener (adsRewardsManager.SoftCurrencyReward);
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void HardCurrencyAd()
    {
        if (AdsManager.Instance.AreAdsDeactive())
            OpenHardCurrencyNoAd();
        else
            OpenHardCurrencyAd();
    }
    void OpenHardCurrencyNoAd()
    {
        signalBus.Fire(new OpenDefaultMessagePopUpSignal { message = $"You have been granted with <b><color=#CFCD00FF>{adsRewardsManager.GetHardCurrencyBonus() * 2}</color></b> coins!!" });
        adsRewardsManager.HardCurrencyRewardNoAds();
    }
    void OpenHardCurrencyAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"You have been rewarded with <b><color=#CFCD00FF>{adsRewardsManager.GetHardCurrencyBonus ()}</color></b> gems!!\nWatch an Ad to recieve <b><color=purple>{adsRewardsManager.GetHardCurrencyBonus () * 2}</color></b> more?" });
        adsRewardsManager.InitialHardCurrencyReward ();
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adPopUpConfirmButton.onClick.AddListener (adsRewardsManager.HardCurrencyReward);
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void ThemeBonusAd()
    {
        if (AdsManager.Instance.AreAdsDeactive())
            OpenThemeBonusNoAd();
        else
            OpenThemeBonusAd();
    }
    void OpenThemeBonusNoAd()
    {
        signalBus.Fire(new OpenDefaultMessagePopUpSignal { message = $"You have been granted with a <b><color=purple>{(float)adsRewardsManager.GetThemeBonusReward(2)}</color></b> popularity bonus on your next theme!!" });
        adsRewardsManager.ThemeBonusRewardNoAds();
    }
    void OpenThemeBonusAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"You have been rewarded with a <b><color=purple>{(float)adsRewardsManager.GetThemeBonusReward (2)}</color></b> popularity bonus on your next theme!!\nWatch an Ad to recieve <b><color=green>{(float)adsRewardsManager.GetThemeBonusReward (1)}</color></b> more?" });
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adsRewardsManager.InitialThemeBonusReward ();
        adPopUpConfirmButton.onClick.AddListener (adsRewardsManager.ThemeBonusReward);
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void EnergyAd()
    {
        if (AdsManager.Instance.AreAdsDeactive())
            OpenEnergyNoAd();
        else
            OpenEnergyAd();
    }
    void OpenEnergyNoAd()
    {
        signalBus.Fire(new OpenDefaultMessagePopUpSignal { message = $"You seem a bit short on energy for that video, this <b><color=red>{adsRewardsManager.GetEnergyBonus() * 2}</color> energy recharge is for you!" });
        adsRewardsManager.EnergyRewardNoAds();
    }
    void OpenEnergyAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"You seem a bit short on energy for that video, do you want to see an ad to recieve <b><color=red>{adsRewardsManager.GetEnergyBonus() * 2}</color> energy"});
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adPopUpConfirmButton.onClick.AddListener (adsRewardsManager.EnergyReward);
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void VideoShortenAd(OpenTimeShortenAdSignal signal)
    {
        if (AdsManager.Instance.AreAdsDeactive())
            OpenVideoShortenNoAd(signal);
        else
            OpenVideoShortenAd(signal);
    }
    void OpenVideoShortenNoAd(OpenTimeShortenAdSignal signal)
    {
        signalBus.Fire(new OpenDefaultMessagePopUpSignal { message = $"The video production time has been automatically shortened" });
        adsRewardsManager.VideoShortenRewardNoAds(signal.video);
    }
    void OpenVideoShortenAd (OpenTimeShortenAdSignal signal)
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"Watch an Ad to shorten the production time of this video"});
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adPopUpConfirmButton.onClick.AddListener (()=>adsRewardsManager.VideoShortenReward(signal.video));
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void DoubleViewsAd()
    {
        if (AdsManager.Instance.AreAdsDeactive())
            OpenDoubleViewsNoAd();
        else
            OpenDoubleViewsAd();
    }
    void OpenDoubleViewsNoAd()
    {
        signalBus.Fire(new OpenDefaultMessagePopUpSignal { message = $"The views for this video will be double!!" });
        adsRewardsManager.ViewsBonusReward();
    }
    void OpenDoubleViewsAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = "Watch an Ad to double the views for this video" });
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adPopUpConfirmButton.onClick.AddListener (adsRewardsManager.ViewsBonusReward);
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
}
