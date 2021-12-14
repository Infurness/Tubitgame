using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Ads_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] AdsRewardsManager adsRewardsManager;

    [SerializeField] Button popUpConfirmButton;

    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenSoftCurrencyAdSignal> (OpenSoftCurrencyAd);
        signalBus.Subscribe<OpenHardCurrencyAdSignal> (OpenHardCurrencyAd);
        signalBus.Subscribe<OpenThemeBonusAdSignal> (OpenThemeBonusAd);
        signalBus.Subscribe<OpenEnergyAdSignal> (OpenEnergyAd);
        signalBus.Subscribe<OpenTimeShortenAdSignal> (OpenVideoShortenAd);
        signalBus.Subscribe<OpenDoubleViewsAdSignal> (OpenDoubleViewsAd);
    }

    void ClosePopUp ()
    {
        signalBus.Fire<CloseAdsDefaultPopUpSignal> ();
    }
    void OpenSoftCurrencyAd ()
    {
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { message = $"You have been rewarded with <b><color=#CFCD00FF>{adsRewardsManager.GetSoftCurrencyBonus()}</color></b> coins!!\nWatch an Ad to recieve <b><color=#CFCD00FF>{adsRewardsManager.GetSoftCurrencyBonus ()*4}</color></b> more?" });
        adsRewardsManager.InitialSoftCurrencyReward ();
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.SoftCurrencyReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void OpenHardCurrencyAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"You have been rewarded with <b><color=purple>{adsRewardsManager.GetHardCurrencyBonus ()}</color></b> gems!!\nWatch an Ad to recieve <b><color=purple>{adsRewardsManager.GetHardCurrencyBonus () * 2}</color></b> more?" });
        adsRewardsManager.InitialHardCurrencyReward ();
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.HardCurrencyReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void OpenThemeBonusAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"You have been rewarded with <b><color=green>{(float)adsRewardsManager.GetThemeBonusReward (2)}</color></b> bonus popularity on your next theme!!\nWatch an Ad to recieve <b><color=green>{(float)adsRewardsManager.GetThemeBonusReward (1)}</color></b> more?" });
        popUpConfirmButton.onClick.RemoveAllListeners ();
        adsRewardsManager.InitialThemeBonusReward ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.ThemeBonusReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void OpenEnergyAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"You seem a bit short on energy for that video, do you want to see an ad to recieve <b><color=red>{adsRewardsManager.GetEnergyBonus() * 2}</color> energy"});
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.EnergyReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }

    void OpenVideoShortenAd (OpenTimeShortenAdSignal signal)
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = $"Watch an Ad to shorten the production time of this video"});
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (()=>adsRewardsManager.VideoShortenReward(signal.video));
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }

    void OpenDoubleViewsAd ()
    {
        signalBus.Fire (new OpenAdsDefaultPopUpSignal { message = "Watch an Ad to double the views for this video" });
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.ViewsBonusReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
}
