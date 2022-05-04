using System;
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
    [Inject] private EnergyManager energyManager;
    [Inject] private GlobalAudioManager audioManager;
    [Inject] private SoundsHolder soundsHolder;
    GameClock gameClock;

    [SerializeField] private Night_Transition night_Transition;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text energyTimeText;
    private float energyTimeSecondsCount;
    [SerializeField] private Image energyFillBar;
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerSubscribers;
    [SerializeField] private GameObject leaderboardsPanel;
    [SerializeField] private GameObject videoManagerPanel;
    [SerializeField] private GameObject eventsPanel;
    [SerializeField] private GameObject storePanel;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject backButtonIcon;
    [SerializeField] private GameObject homeButtonIcon;
    [SerializeField] private Button[] videoManagerButtons;
    [SerializeField] private Button eventsButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button storeSCButton;
    [SerializeField] private Button storeHCButton;
    [SerializeField] private TMP_Text softCurrencyText;
    [SerializeField] private TMP_Text hardCurrencyText;
    [SerializeField] private TMP_Text clockTimeText;
    int timeMinutes;
    [SerializeField] private GameObject backButtonsPanel;
    [SerializeField] private GameObject xpBarPanel;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image xpFillBar;
    [Inject] private GameAnalyticsManager gameAnalyticsManager;
    private void Awake ()
    {
        _signalBus.Subscribe<EnergyValueSignal> (SetEnergy);
        //_signalBus.Subscribe<StartRecordingSignal> (OpenHomePanel);
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenVideoManagerPanel);
        _signalBus.Subscribe<UpdateSoftCurrencySignal> (UpdateSoftCurrency);
        _signalBus.Subscribe<UpdateHardCurrencySignal> (UpdateHardCurrency);
        _signalBus.Subscribe<UpdateExperienceSignal> (UpdateExperienceBar);
        _signalBus.Subscribe<ChangeUsernameSignal> (UpdateUsername);
        _signalBus.Subscribe<LevelUpSignal> (LevelUpUpdateHUD);
        _signalBus.Subscribe<ChangeBackButtonSignal> (ChangeBackButton);
        _signalBus.Subscribe<OpenRealEstateShopSignal>(OpenStorePanel);
        _signalBus.Subscribe<OpenHomeScreenSignal>(OpenHomePanel);
        _signalBus.Subscribe<HUDStartingEnergySignal>(StartEnergy);
        gameClock = GameClock.Instance;
    }
   
    // Start is called before the first frame update
    void Start()
    {
        gameAnalyticsManager.SendCustomEvent("MainScreen");
        foreach (Button button in videoManagerButtons)
            button.onClick.AddListener (OpenVideoManagerPanel);
        if(eventsButton)
            eventsButton.onClick.AddListener (OpenEventsPanel);

        storeButton.onClick.AddListener (OpenStorePanel);
        storeSCButton.onClick.AddListener(OpenStoreSC);
        storeHCButton.onClick.AddListener(OpenStoreHC);

        InitialState ();
        _signalBus.Subscribe<RoomCustomizationVisibilityChanged>((signal =>
        {
            if (signal.Visibility)
            {
                gameAnalyticsManager.SendCustomEvent("RoomCustomizationScreen");
                leaderboardsPanel.gameObject.SetActive(false);
                playerPanel.gameObject.SetActive(false);
                
            }
            else
            {
                leaderboardsPanel.gameObject.SetActive(false);
                playerPanel.gameObject.SetActive(true);

            }
        } ));
        
        _signalBus.Subscribe<CharacterCustomizationVisibilityChanged>((signal) =>
        {
            if (signal.Visibility)
            {
                gameAnalyticsManager.SendCustomEvent("CharacterCustomizationScreen");

                leaderboardsPanel.gameObject.SetActive(false);
                playerPanel.gameObject.SetActive(false);
                
            }
            else
            {
                leaderboardsPanel.gameObject.SetActive(false);
                playerPanel.gameObject.SetActive(true);

            }
        });
        //StopAllCoroutines ();
        //StartCoroutine (DecreaseSeconds());
        //audioManager.PlaySound(soundsHolder.generalMusic, AudioType.Music, true);

        //Sleep sound
        _signalBus.Subscribe<RestStateChangedSignal>(() => GlobalAudioManager.Instance.PlaySound(SoundsHolder.Instance.pushButton, AudioType.Effect));

    }

    // Update is called once per frame
    void Update()
    {     
        if(gameClock && timeMinutes != gameClock.Now.Minute)
        {
            timeMinutes = gameClock.Now.Minute;
            clockTimeText.text = gameClock.Now.ToString("H:mm");// ToString("t", CultureInfo.CreateSpecificCulture ("en-US").DateTimeFormat);
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
        night_Transition.DisableElements();
        gameAnalyticsManager.SendCustomEvent("VideoMangerScreen");
        OpenScreenPanel (HUDScreen.VideoManager);
    }
    void OpenEventsPanel ()
    {
        OpenScreenPanel (HUDScreen.Events);
    }
    void OpenStorePanel ()
    {
        gameAnalyticsManager.SendCustomEvent("ShopScreen");
        OpenScreenPanel (HUDScreen.Store);       
    }
    void OpenStoreSC()
    {
        gameAnalyticsManager.SendCustomEvent("ShopScreen");
        OpenScreenPanel(HUDScreen.Store);
        OpenScreenPanel(HUDScreen.Store);

        _signalBus.Fire<OpenSCCurrenciesPanelSignal>();
    }
    void OpenStoreHC()
    {
        gameAnalyticsManager.SendCustomEvent("ShopScreen");
        OpenScreenPanel(HUDScreen.Store);
        OpenScreenPanel(HUDScreen.Store);

        _signalBus.Fire<OpenHCCurrenciesPanelSignal>();
    }
    void OpenScreenPanel (HUDScreen _screenToOpen)
    {
        if (_screenToOpen == HUDScreen.Home)
        {
            homePanel.SetActive (true);
            playerPanel.SetActive (true);
            leaderboardsPanel.SetActive (false);
            xpBarPanel.SetActive (true);
            backButtonsPanel.SetActive (false);
            _signalBus.Fire<UpdateExperienceSignal> ();
            _signalBus.Fire<OpenHomePanelSignal> ();
            _signalBus.Fire<CanUseItemsInRoom>(new CanUseItemsInRoom { canUse = true }) ;
        }
        else
        {
            homePanel.SetActive (false);
            playerPanel.SetActive (false);
            leaderboardsPanel.SetActive (false);
            xpBarPanel.SetActive (false);
            backButtonsPanel.SetActive (true);
            _signalBus.Fire<ChangeBackButtonSignal> (new ChangeBackButtonSignal { changeToHome = true });
            _signalBus.Fire<CanUseItemsInRoom>(new CanUseItemsInRoom { canUse = false });
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
            if (!storePanel.activeSelf)
            {
                audioManager.PlaySound(soundsHolder.enterShop, AudioType.Effect);
                //audioManager.PlaySound(soundsHolder.shopMusic, AudioType.Music, true);
            }
                
            storePanel.SetActive (true);
            playerPanel.SetActive(false);
        }      
        else
        {
            //if (storePanel.activeSelf)
            //    audioManager.PlaySound(soundsHolder.generalMusic, AudioType.Music, true);
            storePanel.SetActive (false);      
        }          
    }
    void StartEnergy(HUDStartingEnergySignal signal)
    {
        SetEnergy(new EnergyValueSignal { energy = signal.energy });
    }
    void SetEnergy (EnergyValueSignal _signal)
    {
        energyText.text = $"{(int)_signal.energy}";
        energyFillBar.fillAmount = _signal.energy / energyManager.GetMaxEnergy();
        energyTimeSecondsCount = (energyManager.GetEnergy()-energyManager.GetMaxEnergy())/ energyManager.GetEnergyGainedPerSecond ();
        TimeSpan time = TimeSpan.FromSeconds (energyTimeSecondsCount);
        string timeStr = time.ToString (@"hh\:mm\:ss");
        energyTimeText.text = $"Energy will be fulfilled in { timeStr }";

        //VFX
        if (_signal.energy <1)
            _signalBus.Fire<VFX_NoEnergyParticlesSignal>();
        if (_signal.energy / energyManager.GetMaxEnergy() < 0.4f)
            _signalBus.Fire<VFX_LowEnergyBlinkSignal>();
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
    void LevelUpUpdateHUD ()
    {
        _signalBus.Fire<AddEnergySignal> (new AddEnergySignal { energyAddition = energyManager.GetMaxEnergy ()}); ; //To refresh energy
    }

    ////IEnumerator DecreaseSeconds ()
    ////{
    ////    while (energyManager.GetEnergy () < energyManager.GetMaxEnergy ())
    ////    {
            
    ////        yield return null;
    ////    }
    ////    energyTimeText.text = "00:00:00";
    ////}
    
    void ChangeBackButton (ChangeBackButtonSignal signal)
    {
        backButtonsPanel.SetActive(true);

        backButton.onClick.RemoveAllListeners ();
        if (signal.changeToHome)
        {
            backButtonIcon.SetActive (false);
            homeButtonIcon.SetActive (true);
            backButtonsPanel.SetActive(true);
            backButton.onClick.AddListener (OpenHomePanel);
            if(signal.buttonAction!=null)
                backButton.onClick.AddListener(signal.buttonAction);
            backButton.onClick.AddListener(FireBackButtonSignal);//Tutorial
        }
        else
        {
            backButtonIcon.SetActive (true);
            homeButtonIcon.SetActive (false);
            //  backButton.onClick.AddListener (OpenVideoManagerPanel);
            if (signal.buttonAction != null)
                backButton.onClick.AddListener(signal.buttonAction);

        }
    }

    void FireBackButtonSignal()
    {
        _signalBus.Fire<BackButtonClickedSignal>();
    }
}
