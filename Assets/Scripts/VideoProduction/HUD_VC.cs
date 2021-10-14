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

    [SerializeField] private Image energyBar;
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject videoManagerPanel;
    [SerializeField] private GameObject eventsPanel;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button videoManagerButton;
    [SerializeField] private Button eventsButton;
    [SerializeField] private Button storeButton;
    private int softCurrency = 0; //Dummy Here until player data has this field
    [SerializeField] private TMP_Text softCurrencyText;
    [SerializeField] private Color buttonsHighlightColor;

    // Start is called before the first frame update
    void Start()
    {
        OpenHomePanel ();
        homeButton.onClick.AddListener (OpenHomePanel);
        videoManagerButton.onClick.AddListener (OpenVideoManagerPanel);
        eventsButton.onClick.AddListener (OpenEventsPanel);
        storeButton.onClick.AddListener (OpenStorePanel);

        _signalBus.Subscribe<EnergyValueSignal> (SetEnergy);
        _signalBus.Subscribe<StartRecordingSignal> (OpenHomePanel);
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenVideoManagerPanel);
        _signalBus.Subscribe<GetMoneyFromVideoSignal> (AddSoftCurrency);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            homeButton.GetComponentInChildren<Image> ().color = buttonsHighlightColor;
        }
        else
        {
            homePanel.SetActive (false);
            homeButton.GetComponentInChildren<Image> ().color = Color.white;
        }


        if (_screenToOpen == HUDScreen.VideoManager)
        {
            videoManagerPanel.SetActive (true);
            videoManagerButton.GetComponentInChildren<Image> ().color = buttonsHighlightColor;
        }
        else
        {
            videoManagerPanel.SetActive (false);
            videoManagerButton.GetComponentInChildren<Image> ().color = Color.white;

        }


        if (_screenToOpen == HUDScreen.Events)
        { 
            eventsPanel.SetActive (true);
            eventsButton.GetComponentInChildren<Image> ().color = buttonsHighlightColor;
        }
        else
        { 
            eventsPanel.SetActive (false);
            eventsButton.GetComponentInChildren<Image> ().color = Color.white;
        }

        if (_screenToOpen == HUDScreen.Store)
        {
            storePanel.SetActive (true);
            storeButton.GetComponentInChildren<Image> ().color = buttonsHighlightColor;
        }      
        else
        {
            storePanel.SetActive (false);
            storeButton.GetComponentInChildren<Image> ().color = Color.white;
        }          
    }
    void SetEnergy (EnergyValueSignal _signal)
    {
        energyBar.fillAmount = _signal.energy / 100;
    }
    void AddSoftCurrency (GetMoneyFromVideoSignal _signal) //Dummy This should be in player manager, will be here until currency is set in player data
    {
        softCurrency+= youTubeVideoManager.RecollectVideoMoney (_signal.videoName);
        softCurrencyText.text = $"{softCurrency}$";
    }
}
