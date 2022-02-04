using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public enum RewardType { SoftCurrency, HardCurrency, Energy, Theme, ShortenProduction};
public class AdPopUp_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    [SerializeField] private TMP_Text adsTitleText;
    [SerializeField] private TMP_Text adsPanelText;
    [SerializeField] private TMP_Text adsFullPanelText;
    [SerializeField] private Image bigIcon;
    [SerializeField] private TMP_Text bigReward;
    [SerializeField] private Image smallIcon;
    [SerializeField] private TMP_Text smallReward;
    [SerializeField] private TMP_Text acceptButton;

    [SerializeField] private GameObject watchAnAdText;
    [SerializeField] private GameObject videoIcon;
    [SerializeField] private GameObject bgIcon;

    [SerializeField] private Sprite SCBigIcon;
    [SerializeField] private Sprite SCSmallIcon;

    [SerializeField] private Sprite HCBigIcon;
    [SerializeField] private Sprite HCSmallIcon;

    [SerializeField] private Sprite energyBigIcon;
    [SerializeField] private Sprite energySmallIcon;

    [SerializeField] private Sprite themeBigIcon;
    [SerializeField] private Sprite themeSmallIcon;
    private void Start()
    {
        signalBus.Subscribe<OpenAdsDefaultPopUpSignal>(SetPanelUp);
    }
    void InitialState(bool adsActive)
    {
        if (adsActive)
        {
            watchAnAdText.SetActive(true);
            smallIcon.gameObject.SetActive(true);
            smallReward.gameObject.SetActive(true);
            videoIcon.SetActive(true);
            acceptButton.text = "Watch ad";
            if (TutorialManager.Instance != null)
                acceptButton.text = "Free";
        }
        else
        {
            watchAnAdText.SetActive(false);
            smallIcon.gameObject.SetActive(false);
            smallReward.gameObject.SetActive(false);
            videoIcon.SetActive(false);
            acceptButton.text = "Claim";
        }
        adsFullPanelText.gameObject.SetActive(false);
        adsPanelText.gameObject.SetActive(true);
        bigIcon.gameObject.SetActive(true);
        bigReward.gameObject.SetActive(true);
        bgIcon.SetActive(true);
    }
    public void SetPanelUp(OpenAdsDefaultPopUpSignal signal)
    {
        adsTitleText.text = signal.title;
        adsPanelText.text = signal.message;
        bigReward.text = $"x{signal.reward}";
        smallReward.text = $"x{signal.rewardBonus}";

        InitialState(signal.adsActive);

        switch (signal.rewardType)
        {
            case RewardType.SoftCurrency:
                bigIcon.sprite = SCBigIcon;
                smallIcon.sprite = SCSmallIcon;
                break;

            case RewardType.HardCurrency:
                bigIcon.sprite = HCBigIcon;
                smallIcon.sprite = HCSmallIcon;
                break;

            case RewardType.Energy:
                watchAnAdText.SetActive(false);
                smallIcon.gameObject.SetActive(false);
                smallReward.gameObject.SetActive(false);
                bigIcon.sprite = energyBigIcon;
                smallIcon.sprite = energySmallIcon;
                break;

            case RewardType.Theme:
                bigIcon.sprite = themeBigIcon;
                smallIcon.sprite = themeSmallIcon;
                break;

            case RewardType.ShortenProduction:
                watchAnAdText.SetActive(false);
                smallIcon.gameObject.SetActive(false);
                smallReward.gameObject.SetActive(false);
                adsPanelText.gameObject.SetActive(false);
                bigIcon.gameObject.SetActive(false);
                bigReward.gameObject.SetActive(false);
                bgIcon.SetActive(false);

                adsFullPanelText.gameObject.SetActive(true);
                adsFullPanelText.text = signal.message;
                break;
        }
        AspectRatioFitter fitter = smallIcon.GetComponent<AspectRatioFitter>();
        fitter.aspectRatio = smallIcon.sprite.rect.width / smallIcon.sprite.rect.height;
        fitter = bigIcon.GetComponent<AspectRatioFitter>();
        fitter.aspectRatio = bigIcon.sprite.rect.width / bigIcon.sprite.rect.height;
    }
}
