using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx.Triggers;
using Zenject;

public class HomePanel_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private YouTubeVideoManager youTubeVideoManager;
    [Inject] private EnergyManager energyManager;

    //[SerializeField] private Button publishButton;
   // [SerializeField] private ScrollRect viewsScroll;
    [SerializeField] private Button playerIconButton;

    [SerializeField] private Button restButton; 
       [SerializeField] GameObject shopButtonPanel;
    [SerializeField] private GameObject videoMangerButtonPanel;
    [SerializeField] private GameObject customizationButtonsPanel;
    [SerializeField] private Canvas mainCanvas;
    ThemeType[] selectedThemeTypes; //Dummy unused code
    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<StartRecordingSignal> (StartRecording);
        _signalBus.Subscribe<ChangeRestStateSignal> (RestButtonBehaviour);
       // publishButton.onClick.AddListener (OnPublishVideoPressed);
     //   viewsScroll.onValueChanged.AddListener (OnViewsScroll);
        playerIconButton.onClick.AddListener (OpenSettingsPanel);
       // closeSettingsButton.onClick.AddListener (OpenSettingsPanel );
   //     viewsScroll.onValueChanged.AddListener (OnViewsScroll);
        playerIconButton.onClick.AddListener (OpenSettingsPanel);
        restButton.onClick.AddListener (RestButtonBehaviour);

        InitialScreenState ();
        _signalBus.Subscribe<RoomCustomizationVisibilityChanged>((signal =>
        {
            if (signal.Visibility)
            {
                videoMangerButtonPanel.gameObject.SetActive(false);
                shopButtonPanel.gameObject.SetActive(false);
                restButton.gameObject.SetActive(false);
                playerIconButton.gameObject.SetActive(false);
                customizationButtonsPanel.gameObject.SetActive(false);
                mainCanvas.sortingOrder = 0;

            }
            else
            {
                videoMangerButtonPanel.gameObject.SetActive(true);
                shopButtonPanel.gameObject.SetActive(true);
                restButton.gameObject.SetActive(true);
                playerIconButton.gameObject.SetActive(true);
                customizationButtonsPanel.gameObject.SetActive(true);
                mainCanvas.sortingOrder = 30;


            }
        } ));
        
        _signalBus.Subscribe<CharacterCustomizationVisibilityChanged>((signal) =>
        {
            if (signal.Visibility)
            {
                videoMangerButtonPanel.gameObject.SetActive(false);
                shopButtonPanel.gameObject.SetActive(false);
                restButton.gameObject.SetActive(false);
                playerIconButton.gameObject.SetActive(false);
                customizationButtonsPanel.gameObject.SetActive(false);

                
            }
            else
            {
                videoMangerButtonPanel.gameObject.SetActive(true);
                shopButtonPanel.gameObject.SetActive(true);
                restButton.gameObject.SetActive(true);
                playerIconButton.gameObject.SetActive(true);
                customizationButtonsPanel.gameObject.SetActive(true);


            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitialScreenState ()
    {
        //publishButton.gameObject.SetActive (false);
    }
    void StartRecording (StartRecordingSignal _recordingSignal)
    {
        //selectedThemeTypes = _recordingSignal.recordedThemes;
        //StopAllCoroutines ();
        //StartCoroutine (FillTheRecordImage (_recordingSignal.recordingTime));   
    }



    //void OnPublishVideoPressed ()
    //{
    //    publishButton.gameObject.SetActive (false);
    //    PublishVideo ();
    //}
    //void PublishVideo ()
    //{
    //    _signalBus.Fire<ShowVideosStatsSignal> (new ShowVideosStatsSignal ());     
    //    _signalBus.Fire<PublishVideoSignal> (new PublishVideoSignal () { videoName = youTubeVideoManager.GetVideoNameByTheme(selectedThemeTypes), videoThemes = selectedThemeTypes});
    //}
    void OnViewsScroll (Vector2 vector)
    {
        if(vector!=Vector2.zero)
        {
            //Debug.Log (viewsScroll.content.position);
            //Debug.Log (viewsScroll.content.position);
        }

    }
    void OpenSettingsPanel ()
    {
        _signalBus.Fire<OpenSettingPanelSignal> ();
    }

    void RestButtonBehaviour ()
    {
        energyManager.ChangePlayerRestingState ();

        if (!energyManager.GetPlayerIsResting ())
            restButton.GetComponentInChildren<TMP_Text> ().text = "Rest";
        else
            restButton.GetComponentInChildren<TMP_Text> ().text = "Stop\nResting";

        
    }
}
