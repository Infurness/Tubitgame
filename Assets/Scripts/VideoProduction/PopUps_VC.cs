using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PopUps_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private GameObject popUpsBlockBackgroundPanel;

    [SerializeField] private GameObject themeSelectionPanel;
    [SerializeField] private Button themeSelectionPanelCloseButton;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button settingsPanelCloseButton;

    [SerializeField] private GameObject deleteAccountPanel;
    [SerializeField] private Button[] deleteAccountPanelCloseButtons;

    [SerializeField] private GameObject leaderboardsPanel;
    [SerializeField] private Button leaderboardsPanelCloseButton;

    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenThemeSelectorPopUpSignal> (OpenThemeSelector);
        signalBus.Subscribe<ConfirmThemesSignal> (CloseThemeSelector);
        signalBus.Subscribe<OpenSettingPanelSignal> (OpenSettings);
        signalBus.Subscribe<OpenDeleteAccountSignal> (OpenDeleteAccount);
        signalBus.Subscribe<OpenLeaderboardsSignal> (OpenLeaderboards);

        themeSelectionPanelCloseButton.onClick.AddListener (CloseThemeSelector);
        settingsPanelCloseButton.onClick.AddListener (CloseSettings);
        foreach(Button button in deleteAccountPanelCloseButtons)
        {
            button.onClick.AddListener (CloseDeleteAccount);
        }
        leaderboardsPanelCloseButton.onClick.AddListener (CloseLeaderboards);

        InitialState ();
    }
    void InitialState ()
    {
        CloseThemeSelector ();
        CloseSettings ();
        CloseDeleteAccount ();
        CloseLeaderboards ();
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
        CloseSettings ();
        LeaderboardManager.Instance.GetTop10InLeaderboard ();
        popUpsBlockBackgroundPanel.SetActive (true);
        leaderboardsPanel.SetActive (true);
    }
    void CloseLeaderboards ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        leaderboardsPanel.SetActive (false);
    }
}
