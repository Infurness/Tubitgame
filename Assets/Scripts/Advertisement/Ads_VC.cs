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
        signalBus.Subscribe<OpenDoubleViewsAdSignal> (OpenDoubleViewsAd);
    }
    void ClosePopUp ()
    {
        signalBus.Fire<CloseAdsDefaultPopUpSignal> ();
    }
    void OpenSoftCurrencyAd ()
    {
        signalBus.Fire(new OpenAdsDefaultPopUpSignal { message = "Watch an Ad to recieve a x4 reward?"});
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.SoftCurrencyReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void OpenHardCurrencyAd ()
    {
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.HardCurrencyReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void OpenThemeBonusAd ()
    {
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.ThemeBonusReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }
    void OpenEnergyAd ()
    {
        popUpConfirmButton.onClick.RemoveAllListeners ();
        popUpConfirmButton.onClick.AddListener (adsRewardsManager.EnergyReward);
        popUpConfirmButton.onClick.AddListener (ClosePopUp);
    }

    void OpenVideoShortenAd ()
    {
        popUpConfirmButton.onClick.RemoveAllListeners ();
        //popUpConfirmButton.onClick.AddListener (adsRewardsManager.VideoShortenReward);
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
