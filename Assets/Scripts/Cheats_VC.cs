using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Cheats_VC : MonoBehaviour
{
    [Inject] CheatsManager cheatsManager;

    [SerializeField] private Button add100EnergyButton;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Button resetXp;
    [SerializeField] private Button AddEnergyItem;
    [SerializeField] private Button softCurrencyRewardedAd;

    [SerializeField] private Button softCurrencyCheatButton;

    [SerializeField] private Button hardCurrencyCheatButton;
    // Start is called before the first frame update
    void Start ()
    {
        add100EnergyButton.onClick.AddListener (cheatsManager.Add100Energy);
        levelUpButton.onClick.AddListener (cheatsManager.Add1Level);
        resetXp.onClick.AddListener (cheatsManager.ResetExperience);
        AddEnergyItem.onClick.AddListener (cheatsManager.AddEnergyItem);
        softCurrencyRewardedAd.onClick.AddListener (cheatsManager.SoftCurrencyRewardAd);
        softCurrencyCheatButton.onClick.AddListener(cheatsManager.Add100SoftCurrency);
        hardCurrencyCheatButton.onClick.AddListener(cheatsManager.Add100HardCurrency);
    }
}
