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
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "Get more for free!", message = "You have been rewarded", rewardType = RewardType.SoftCurrency, reward = adsRewardsManager.GetSoftCurrencyBonus()*5, rewardBonus = 0, adsActive = false });
        adsRewardsManager.SoftCurrencyRewardNoAd();
    }
    void OpenSoftCurrencyAd ()
    {
        signalBus.Fire(new OpenAdsDefaultPopUpSignal {title = "Get more for free!", message = "You have been rewarded", rewardType=RewardType.SoftCurrency, reward= adsRewardsManager.GetSoftCurrencyBonus(), rewardBonus = adsRewardsManager.GetSoftCurrencyBonus() * 4, adsActive = true });
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
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "Get more for free!", message = "You have been rewarded", rewardType = RewardType.HardCurrency, reward = adsRewardsManager.GetHardCurrencyBonus()*2, adsActive = false });
        adsRewardsManager.HardCurrencyRewardNoAds();
    }
    void OpenHardCurrencyAd ()
    {
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "Get more for free!", message = "You have been rewarded", rewardType = RewardType.HardCurrency, reward = adsRewardsManager.GetHardCurrencyBonus(), rewardBonus = adsRewardsManager.GetHardCurrencyBonus() * 2, adsActive = true });
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
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "Theme bonus reward!", message = "Your next video will be more popular!", rewardType = RewardType.Theme, reward = adsRewardsManager.GetThemeBonusReward(1), rewardBonus = 0, adsActive = false });
        adsRewardsManager.ThemeBonusRewardNoAds();
    }
    void OpenThemeBonusAd ()
    {
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "Theme bonus reward!", message = "Your next video will be more popular!", rewardType = RewardType.Theme, reward = adsRewardsManager.GetThemeBonusReward(2), rewardBonus = adsRewardsManager.GetThemeBonusReward(1), adsActive = true});
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
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "You are low on energy!", message = "This is for you!", rewardType = RewardType.Energy, reward = adsRewardsManager.GetEnergyBonus() * 2, rewardBonus = 0,adsActive=false });
        adsRewardsManager.EnergyRewardNoAds();
    }
    void OpenEnergyAd ()
    {
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "You are low on energy!", message = "Watch an ad to receive more", rewardType = RewardType.Energy, reward = adsRewardsManager.GetEnergyBonus() * 2, rewardBonus = 0, adsActive = true });
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adPopUpConfirmButton.onClick.AddListener (adsRewardsManager.EnergyReward);
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void VideoShortenAd(OpenTimeShortenAdSignal signal)
    {
        StartCoroutine(WaitToShowAd(signal));
    }
    IEnumerator WaitToShowAd(OpenTimeShortenAdSignal signal)
    {
        yield return new WaitForSeconds(0.5f);
        if (AdsManager.Instance.AreAdsDeactive())
            OpenVideoShortenNoAd(signal);
        else
            OpenVideoShortenAd(signal);
    }
    void OpenVideoShortenNoAd(OpenTimeShortenAdSignal signal)
    {
        signalBus.Fire(new OpenDefaultMessagePopUpSignal {message = $"The video production time has been automatically shortened" });
        adsRewardsManager.VideoShortenRewardNoAds(signal.video);
    }
    void OpenVideoShortenAd (OpenTimeShortenAdSignal signal)
    {
        if(TutorialManager.Instance==null)
        {
            signalBus.Fire(new OpenAdsDefaultPopUpSignal { title = "Speed up!", message = $"Watch an Ad to shorten the production time of this video", rewardType = RewardType.ShortenProduction, adsActive = true });
            adPopUpConfirmButton.onClick.RemoveAllListeners();
            adPopUpConfirmButton.onClick.AddListener(() => adsRewardsManager.VideoShortenReward(signal.video));
            adPopUpConfirmButton.onClick.AddListener(ClosePopUp);
        }
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
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { title = "Boost views!", message = "Watch an Ad to double the views for this video", rewardType = RewardType.ShortenProduction, adsActive = true});
        adPopUpConfirmButton.onClick.RemoveAllListeners ();
        adPopUpConfirmButton.onClick.AddListener (adsRewardsManager.ViewsBonusReward);
        adPopUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
}
