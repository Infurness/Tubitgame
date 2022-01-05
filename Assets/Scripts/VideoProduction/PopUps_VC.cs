using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class PopUps_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private GlobalAudioManager audioManager;
    [Inject] private SoundsHolder soundsHolder;

    [SerializeField] private GameObject popUpsBlockBackgroundPanel;

    [SerializeField] private GameObject defaultDisclaimerPanelPopUp;
    [SerializeField] private TMP_Text defaultDisclaimerText;
    [SerializeField] private Button defaultDisclaimerPanelCloseButton;

    [SerializeField] private GameObject themeSelectionPanel;
    [SerializeField] private Button themeSelectionPanelCloseButton;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsPanelCloseButton;

    [SerializeField] private GameObject deleteAccountPanel;
    [SerializeField] private Button[] deleteAccountPanelCloseButtons;

    [SerializeField] private GameObject leaderboardsPanel;
    [SerializeField] private Button leaderboardsPanelCloseButton;

    [SerializeField] private GameObject LevelUpPanel;
    [SerializeField] private Button levelUpPanelCloseButton;

    [SerializeField] private GameObject energyInventoryPanel;
    [SerializeField] private Button energyInventoryPanelCloseButton;

    [SerializeField] private GameObject energyTimePanel;
    [SerializeField] private Button energyTimePanelButton;

    [SerializeField] private GameObject adsDefaultPanelPopUp;
    [SerializeField] private TMP_Text adsPanelText;
    [SerializeField] private Button adsDefaultPanelCancelButton;


    Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenDefaultMessagePopUpSignal> (OpenDefaultDisclaimer);
        signalBus.Subscribe<OpenAdsDefaultPopUpSignal> (OpenAdsDefaultPanel);
        signalBus.Subscribe<CloseAdsDefaultPopUpSignal> (CloseAdsDefaultPanel);
        signalBus.Subscribe<OpenThemeSelectorPopUpSignal> (OpenThemeSelector);
        signalBus.Subscribe<ConfirmThemesSignal> (CloseThemeSelector);
        signalBus.Subscribe<OpenSettingPanelSignal> (OpenSettings);
        signalBus.Subscribe<OpenDeleteAccountSignal> (OpenDeleteAccount);
        signalBus.Subscribe<OpenLeaderboardsSignal> (OpenLeaderboards);
        signalBus.Subscribe<LevelUpSignal> (OpenLevelUp);

        defaultDisclaimerPanelCloseButton.onClick.AddListener (CloseDefaultDisclaimer);
        adsDefaultPanelCancelButton.onClick.AddListener (CancelAdsDefaultPanel);
        themeSelectionPanelCloseButton.onClick.AddListener (CloseThemeSelector);
        settingsPanelCloseButton.onClick.AddListener (CloseSettings);
        foreach(Button button in deleteAccountPanelCloseButtons)
        {
            button.onClick.AddListener (CloseDeleteAccount);
        }
        leaderboardsPanelCloseButton.onClick.AddListener (CloseLeaderboards);
        levelUpPanelCloseButton.onClick.AddListener (CloseLevelUp);
        energyInventoryPanelCloseButton.onClick.AddListener (OpenEnergyInventory);
        energyTimePanelButton.onClick.AddListener (OpenCloseEnergyTimeLeftPanel);

        mainCam = Camera.main;

        InitialState ();
    }
    void InitialState ()
    {
        CloseThemeSelector ();
        CloseSettings ();
        CloseDeleteAccount ();
        CloseLeaderboards ();
        CloseLevelUp ();
        CloseDefaultDisclaimer ();
        energyInventoryPanel.SetActive (false);
        energyTimePanel.SetActive (false);
        CloseAdsDefaultPanel ();
    }

    void OpenDefaultDisclaimer(OpenDefaultMessagePopUpSignal signal)
    {
        audioManager.PlaySound(soundsHolder.popUp, AudioType.Effect);
        
        popUpsBlockBackgroundPanel.SetActive (true);
        defaultDisclaimerPanelPopUp.SetActive (true);
        StartCoroutine(OpenPanelAnimation (defaultDisclaimerPanelPopUp));
        defaultDisclaimerText.text = signal.message;
    }
    IEnumerator OpenPanelAnimation (GameObject panel)
    {
        RectTransform rect = panel.GetComponent<RectTransform> ();
        Vector3 panelScale = rect.localScale;
        rect.localScale = Vector3.zero;
        float i = 0;
        while (i < 1)
        {
            rect.localScale = Vector3.Lerp (rect.localScale, panelScale, i);
            i += Time.deltaTime;
            yield return null;
        }
    }

    void CloseDefaultDisclaimer()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        defaultDisclaimerPanelPopUp.SetActive(false);
    }
    void OpenThemeSelector ()
    {
        popUpsBlockBackgroundPanel.SetActive (true);
        themeSelectionPanel.SetActive (true);
    }
    void CloseThemeSelector ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        themeSelectionPanel.SetActive (false);
    }

    void OpenSettings ()
    {
        popUpsBlockBackgroundPanel.SetActive (true);
        settingsPanel.SetActive (true);
    }
    void CloseSettings ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        settingsPanel.SetActive (false);
    }

    void OpenDeleteAccount ()
    {
        CloseSettings ();
        popUpsBlockBackgroundPanel.SetActive (true);
        deleteAccountPanel.SetActive (true);
    }
    void CloseDeleteAccount ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        deleteAccountPanel.SetActive (false);
    }

    void OpenLeaderboards ()
    {
        LeaderboardManager.Instance.GetTop10InLeaderboard ();
        //popUpsBlockBackgroundPanel.SetActive (true);
        leaderboardsPanel.SetActive (true);
    }
    void CloseLeaderboards ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        leaderboardsPanel.SetActive (false);
    }


    void OpenLevelUp ()
    {
        audioManager.PlaySound(soundsHolder.popUp, AudioType.Effect);
        audioManager.PlaySound(soundsHolder.rankUp, AudioType.Effect);
        popUpsBlockBackgroundPanel.SetActive (true);
        LevelUpPanel.SetActive (true);
        signalBus.Fire<OpenLevelUpPanelSignal> ();
    }
    void CloseLevelUp ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        LevelUpPanel.SetActive (false);
        signalBus.Fire<UpdateSoftCurrencySignal> ();
        signalBus.Fire<UpdateHardCurrencySignal> ();
    }

    void OpenEnergyInventory ()
    {
        energyInventoryPanel.SetActive (true);
        signalBus.Fire<OpenEnergyInventorySignal> ();
        StartCoroutine (TapOutsideToClosePanel (energyInventoryPanel, CloseEnergyInventory));
    }
    void CloseEnergyInventory ()
    {
        energyInventoryPanel.SetActive (false);
    }
    IEnumerator TapOutsideToClosePanel(GameObject panel, Action callback)
    {
        yield return null;
        bool dragging = false;
        RectTransform rect = panel.GetComponent<RectTransform> ();
        while (panel.activeSelf == true)
        {
            if (Input.GetMouseButtonDown (0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint (rect, Input.mousePosition, Camera.main))
                {
                    dragging = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (!dragging)
                {
                    if (!RectTransformUtility.RectangleContainsScreenPoint (rect, Input.mousePosition, Camera.main))
                    {
                        callback.Invoke ();
                    }
                }
                else
                {
                    dragging = false;
                } 
            }
            yield return null;
        }
    }

    void OpenCloseEnergyTimeLeftPanel ()
    {
        energyTimePanel.SetActive (!energyTimePanel.activeSelf);
    }

    void OpenAdsDefaultPanel (OpenAdsDefaultPopUpSignal signal)
    {
        audioManager.PlaySound(soundsHolder.popUp, AudioType.Effect);
        adsDefaultPanelPopUp.SetActive (true);
        StartCoroutine (OpenPanelAnimation (adsDefaultPanelPopUp));
        adsPanelText.text = signal.message;
    }
    void CloseAdsDefaultPanel ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        adsDefaultPanelPopUp.SetActive (false);
        signalBus.TryFire<OnVideosStatsUpdatedSignal> ();
    }
    void CancelAdsDefaultPanel ()
    {
        signalBus.Fire<FinishedAdVisualitationRewardSignal> ();
        CloseAdsDefaultPanel ();
    }
}
