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

    [SerializeField] private TMP_Text energyText;
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject videoManagerPanel;
    [SerializeField] private GameObject eventsPanel;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private Button[] homeButtons;
    [SerializeField] private Button videoManagerButton;
    [SerializeField] private Button eventsButton;
    [SerializeField] private Button[] storeButtons;
    private int softCurrency = 0; //Dummy Here until player data has this field
    [SerializeField] private TMP_Text softCurrencyText;
    [SerializeField] private TMP_Text clockTimeText;
    int timeMinutes;
    // Start is called before the first frame update
    void Start()
    {     
        foreach(Button button in homeButtons)
            button.onClick.AddListener (OpenHomePanel);
        videoManagerButton.onClick.AddListener (OpenVideoManagerPanel);
        eventsButton.onClick.AddListener (OpenEventsPanel);
        foreach(Button button in storeButtons)
            button.onClick.AddListener (OpenStorePanel);

        _signalBus.Subscribe<EnergyValueSignal> (SetEnergy);
        _signalBus.Subscribe<StartRecordingSignal> (OpenHomePanel);
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenVideoManagerPanel);
        _signalBus.Subscribe<GetMoneyFromVideoSignal> (AddSoftCurrency);

        InitialState ();
    }

    // Update is called once per frame
    void Update()
    {
        if(timeMinutes!= System.DateTime.Now.Minute)
        {
            SetTime ();
        }
            
    }

    void InitialState ()
    {
        OpenHomePanel ();
        SetTime ();
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
        }
        else
        {
            homePanel.SetActive (false);
        }


        if (_screenToOpen == HUDScreen.VideoManager)
        {
            videoManagerPanel.SetActive (true);
            _signalBus.Fire<OpenVideoManager> ();
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
        energyText.text = $"Energy: { (int)_signal.energy}";
    }
    void AddSoftCurrency (GetMoneyFromVideoSignal _signal) //Dummy This should be in player manager, will be here until currency is set in player data
    {
        softCurrency+= youTubeVideoManager.RecollectVideoMoney (_signal.videoName);
        softCurrencyText.text = $"Soft Currency: {softCurrency}$";
    }
    void SetTime ()
    {
        System.DateTime time = System.DateTime.Now;
        string timeHourText = time.Hour<10?$"0{time.Hour}": time.Hour.ToString();
        timeMinutes = time.Minute;
        string timeMinutesText = timeMinutes < 10 ? $"0{timeMinutes}" : timeMinutes.ToString ();

        clockTimeText.text = $"{timeHourText}:{timeMinutesText}";
    }
}
