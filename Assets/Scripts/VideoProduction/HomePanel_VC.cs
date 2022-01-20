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


    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject productionBarInGamePivot;
    [SerializeField] private GameObject productionBar;
    [SerializeField] private Image productionBarFilling;
    private UnpublishedVideo videoBeingRecorded;

    [SerializeField] private Button hideUIButton;
    [SerializeField] private GameObject[] buttonsToHide;
    bool buttonsHidden;

    ThemeType[] selectedThemeTypes; //Dummy unused code
    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<StartRecordingSignal> (StartRecording);
        _signalBus.Subscribe<VideoStartedSignal>(SetVideoForRecordingBar);
        _signalBus.Subscribe<ChangeRestStateSignal> (RestButtonBehaviour);
        _signalBus.Subscribe<SnapToNeighborhoodViewSignal>(SetProductionBarPosition);
        // publishButton.onClick.AddListener (OnPublishVideoPressed);
        //   viewsScroll.onValueChanged.AddListener (OnViewsScroll);
        playerIconButton.onClick.AddListener (OpenSettingsPanel);
       // closeSettingsButton.onClick.AddListener (OpenSettingsPanel );
   //     viewsScroll.onValueChanged.AddListener (OnViewsScroll);
        playerIconButton.onClick.AddListener (OpenSettingsPanel);
        restButton.onClick.AddListener (RestButtonBehaviour);
        hideUIButton.onClick.AddListener(HideButtons);
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

        productionBar.SetActive(false);
        buttonsHidden = true;
        HideButtons();
        SetProductionBarPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (videoBeingRecorded!=null)
        {
            float secondsPassed = Mathf.Abs((int)(videoBeingRecorded.createdTime - GameClock.Instance.Now).TotalSeconds);
            float secondsLeft = videoBeingRecorded.secondsToBeProduced - secondsPassed;
            productionBarFilling.fillAmount = 1-(secondsLeft / videoBeingRecorded.secondsToBeProduced);
            if (productionBarFilling.fillAmount >=1)
            {
                videoBeingRecorded = null;
                productionBar.SetActive(false);
                _signalBus.Fire(new  ChangeIdleCharacterVisibilitySignal
                {
                    Visibility = false
                });
                _signalBus.Fire(new ChangeSeatedCharacterVisibilitySignal()
                {
                    Visibility = true
                });
            }
            else
            {
                _signalBus.Fire(new  ChangeIdleCharacterVisibilitySignal
                {
                    Visibility = true
                });
                _signalBus.Fire(new ChangeSeatedCharacterVisibilitySignal()
                {
                    Visibility = false
                });
            }         
        }
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

    void SetVideoForRecordingBar(VideoStartedSignal signal)
    {
        videoBeingRecorded = signal.startedVideo;
        productionBar.SetActive(true);
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

    void HideButtons()
    {
        buttonsHidden = !buttonsHidden;
        Vector3 newSize = hideUIButton.GetComponent<Transform>().localScale;
        newSize.y = -newSize.y;
        hideUIButton.GetComponent<Transform>().localScale = newSize;
        foreach (GameObject gO in buttonsToHide)
        {
            gO.SetActive(!buttonsHidden);
        }  
    }

    void SetProductionBarPosition()
    {
        Vector2 viewportPosition = mainCamera.WorldToViewportPoint(productionBarInGamePivot.transform.position);
        viewportPosition *= canvas.GetComponent<RectTransform>().sizeDelta;
        productionBar.GetComponent<RectTransform>().anchoredPosition = viewportPosition;
    }
}
