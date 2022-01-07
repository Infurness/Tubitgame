using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public enum RewardType { SoftCurrency, HardCurrency, Energy, Theme};
public class AdPopUp_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    [SerializeField] private TMP_Text adsTitleText;
    [SerializeField] private TMP_Text adsPanelText;
    [SerializeField] private Image bigIcon;
    [SerializeField] private TMP_Text bigReward;
    [SerializeField] private Image smallIcon;
    [SerializeField] private TMP_Text smallReward;
    [SerializeField] private TMP_Text acceptButton;

    [SerializeField] private GameObject watchAnAdText;
    [SerializeField] private GameObject videoIcon;

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
    public void SetPanelUp(OpenAdsDefaultPopUpSignal signal)
    {
        adsTitleText.text = signal.title;
        adsPanelText.text = signal.message;
        bigReward.text = $"x{signal.reward}";
        smallReward.text = $"x{signal.rewardBonus}";
        if (signal.adsActive)
        {
            watchAnAdText.SetActive(true);
            smallIcon.gameObject.SetActive(true);
            smallReward.gameObject.SetActive(true);
            videoIcon.SetActive(true);
            acceptButton.text = "Watch ad";
        }
        else
        {
            watchAnAdText.SetActive(false);
            smallIcon.gameObject.SetActive(false);
            smallReward.gameObject.SetActive(false);
            videoIcon.SetActive(false);
            acceptButton.text = "Claim";
        }

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
        }
    }
}
