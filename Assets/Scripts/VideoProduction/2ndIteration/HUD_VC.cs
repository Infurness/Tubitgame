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
    [SerializeField] private Button homeButton;
    [SerializeField] private Button videoManagerButton;
    private int softCurrency = 0; //Dummy Here until player data has this field
    [SerializeField] private TMP_Text softCurrencyText;

    // Start is called before the first frame update
    void Start()
    {
        OpenHomePanel ();
        homeButton.onClick.AddListener (OpenHomePanel);
        videoManagerButton.onClick.AddListener (OpenVideoManagerPanel);

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
    void OpenScreenPanel (HUDScreen _screenToOpen)
    {
        if(_screenToOpen == HUDScreen.Home)
            homePanel.SetActive (true);
        else
            homePanel.SetActive (false);

        if (_screenToOpen == HUDScreen.VideoManager)
            videoManagerPanel.SetActive (true);
        else
            videoManagerPanel.SetActive (false);

        //if (screenToOpen == HUDScreen.Events)
        //    homePanel.SetActive (true);
        //else
        //    homePanel.SetActive (false);

        //if (screenToOpen == HUDScreen.Store)
        //    homePanel.SetActive (true);
        //else
        //    homePanel.SetActive (false);
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
