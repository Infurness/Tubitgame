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
    private ulong softCurrency = 0; //Dummy Here until player data has this field
    [SerializeField] private TMP_Text softCurrencyText;
    [SerializeField] private TMP_Text clockTimeText;
    int timeMinutes;

    private void Awake ()
    {
        _signalBus.Subscribe<EnergyValueSignal> (SetEnergy);
        //_signalBus.Subscribe<StartRecordingSignal> (OpenHomePanel);
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenVideoManagerPanel);
        _signalBus.Subscribe<GetMoneyFromVideoSignal> (AddSoftCurrency);

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
        playerName.text = PlayerDataManager.Instance.GetPlayerName ().ToUpper();
        playerSubscribers.text = PlayerDataManager.Instance.GetSubscribers ().ToString();
        OpenHomePanel ();
    }
    void OpenHomePanel ()
    {
        OpenScreenPanel (HUDScreen.Home);
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
        }
        else
        {
            homePanel.SetActive (false);
            playerPanel.SetActive (false);
            leaderboardsPanel.SetActive (false);
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
    void AddSoftCurrency (GetMoneyFromVideoSignal _signal) //Dummy This should be in player manager, will be here until currency is set in player data
    {
        softCurrency =PlayerDataManager.Instance.GetSoftCurrency();
        softCurrencyText.text = $"{softCurrency}$";
    }
}
