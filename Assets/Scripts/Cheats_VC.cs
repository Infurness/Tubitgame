using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Cheats_VC : MonoBehaviour
{
    [Inject] CheatsManager cheatsManager;

    [SerializeField] private GameObject cheatButtonsPanel;
    [SerializeField] private Button hideCheatButtons;
    float hideMeter;
    int touchCount;

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
        hideCheatButtons.onClick.AddListener(()=>EnableButtonsVisibility(false));
        add100EnergyButton.onClick.AddListener (cheatsManager.Add100Energy);
        levelUpButton.onClick.AddListener (cheatsManager.Add1Level);
        resetXp.onClick.AddListener (cheatsManager.ResetExperience);
        AddEnergyItem.onClick.AddListener (cheatsManager.AddEnergyItem);
        softCurrencyRewardedAd.onClick.AddListener (cheatsManager.SoftCurrencyRewardAd);
        softCurrencyCheatButton.onClick.AddListener(cheatsManager.Add100SoftCurrency);
        hardCurrencyCheatButton.onClick.AddListener(cheatsManager.Add100HardCurrency);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hideMeter = 0;
            touchCount += 1;
        }
        hideMeter += Time.deltaTime;
        if (hideMeter >0.2f)
            touchCount = 0;
        if (touchCount >= 3)
            EnableButtonsVisibility(true);
    }
    void EnableButtonsVisibility(bool enable)
    {
        cheatButtonsPanel.SetActive(enable);
        touchCount = 0;
    }
}
