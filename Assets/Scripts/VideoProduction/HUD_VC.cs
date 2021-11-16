using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

enum HUDScreen { Home, VideoManager, Events, Store}
public class HUD_VC : MonoBehaviour
{
    [Inject] SignalBus _signalBus;
    [Inject] YouTubeVideoManager youTubeVideoManager;
    [Inject] private ExperienceManager xpManager;
    GameClock gameClock;

    [SerializeField] private TMP_Text energyText;
    [SerializeField] private Image energyFillBar;
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerSubscribers;
    [SerializeField] private GameObject leaderboardsPanel;
    [SerializeField] private GameObject videoManagerPanel;
    [SerializeField] private GameObject eventsPanel;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private Button[] homeButtons;
    [SerializeField] private Button videoManagerButton;
    [SerializeField] private Button eventsButton;
    [SerializeField] private Button[] storeButtons;
    [SerializeField] private TMP_Text softCurrencyText;
    [SerializeField] private TMP_Text hardCurrencyText;
    [SerializeField] private TMP_Text clockTimeText;
    int timeMinutes;
    [SerializeField] private GameObject backButtonsPanel;
    [SerializeField] private GameObject xpBarPanel;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image xpFillBar;

    private void Awake ()
    {
        _signalBus.Subscribe<EnergyValueSignal> (SetEnergy);
        //_signalBus.Subscribe<StartRecordingSignal> (OpenHomePanel);
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenVideoManagerPanel);
        _signalBus.Subscribe<UpdateSoftCurrencySignal> (UpdateSoftCurrency);
        _signalBus.Subscribe<UpdateHardCurrencySignal> (UpdateHardCurrency);
        _signalBus.Subscribe<UpdateExperienceSignal> (UpdateExperienceBar);
        _signalBus.Subscribe<ChangeUsernameSignal> (UpdateUsername);

        gameClock = GameClock.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {     
        foreach(Button button in homeButtons)
            button.onClick.AddListener (OpenHomePanel);
        videoManagerButton.onClick.AddListener (OpenVideoManagerPanel);
        eventsButton.onClick.AddListener (OpenEventsPanel);
        foreach(Button button in storeButtons)
            button.onClick.AddListener (OpenStorePanel);

        

        InitialState ();
    }

    // Update is called once per frame
    void Update()
    {     
        if(gameClock && timeMinutes != gameClock.Now.Minute)
        {
            timeMinutes = gameClock.Now.Minute;
            clockTimeText.text = gameClock.Now.ToString("t", CultureInfo.CreateSpecificCulture ("en-US").DateTimeFormat);
        }
    }

    void InitialState ()
    {
        UpdateUsername ();
        UpdateSubs ();
        OpenHomePanel ();
        _signalBus.Fire<UpdateSoftCurrencySignal> ();
        _signalBus.Fire<UpdateHardCurrencySignal> ();
    }
    void UpdateUsername ()
    {
        if (PlayerDataManager.Instance != null)
        {
            playerName.text = PlayerDataManager.Instance.GetPlayerName ().ToUpper ();
        }
    }
    void UpdateSubs ()
    {
        playerSubscribers.text = PlayerDataManager.Instance.GetSubscribers ().ToString ();
    }
    void OpenHomePanel ()
    {
        OpenScreenPanel (HUDScreen.Home);
        UpdateSubs ();
    }
    void OpenVideoManagerPanel ()
    {
        OpenScreenPanel (HUDScreen.VideoManager);
    }
    void OpenEventsPanel ()
    {
        OpenScreenPanel (HUDScreen.Events);
    }
    void OpenStorePanel ()
    {
        OpenScreenPanel (HUDScreen.Store);
    }
    void OpenScreenPanel (HUDScreen _screenToOpen)
    {
        if (_screenToOpen == HUDScreen.Home)
        {
            homePanel.SetActive (true);
            playerPanel.SetActive (true);
            leaderboardsPanel.SetActive (true);
            xpBarPanel.SetActive (true);
            backButtonsPanel.SetActive (false);
            _signalBus.Fire<UpdateExperienceSignal> ();
        }
        else
        {
            homePanel.SetActive (false);
            playerPanel.SetActive (false);
            leaderboardsPanel.SetActive (false);
            xpBarPanel.SetActive (false);
            backButtonsPanel.SetActive (true);
        }


        if (_screenToOpen == HUDScreen.VideoManager)
        {
            videoManagerPanel.SetActive (true);
            _signalBus.Fire<OpenVideoManagerSignal> ();
        }
        else
        {
            videoManagerPanel.SetActive (false);
        }


        if (_screenToOpen == HUDScreen.Events)
        { 
            eventsPanel.SetActive (true);
        }
        else
        { 
            eventsPanel.SetActive (false);
        }

        if (_screenToOpen == HUDScreen.Store)
        {
            storePanel.SetActive (true);
        }      
        else
        {
            storePanel.SetActive (false);
        }          
    }
    void SetEnergy (EnergyValueSignal _signal)
    {
        energyText.text = $"{(int)_signal.energy}";
        energyFillBar.fillAmount = _signal.energy / 100; //Dummy : to be replaced by max energy amount
    }
    void UpdateSoftCurrency ()
    {
        softCurrencyText.text = $"{PlayerDataManager.Instance.GetSoftCurrency ()}";
    }
    void UpdateHardCurrency ()
    {
        hardCurrencyText.text = $"{PlayerDataManager.Instance.GetHardCurrency ()}";
    }
    void UpdateExperienceBar ()
    {
        int level = xpManager.GetPlayerLevel ();
        levelText.text = level.ToString();
        xpFillBar.fillAmount =(float)xpManager.GetPlayerXp() / (float)xpManager.GetXpThreshold (level);
    }
}
