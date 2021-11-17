using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

enum VideoManagerPanels {MakeAVideo, ManageVideos }
public class VideoManager_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private YouTubeVideoManager _youTubeVideoManager;
    [Inject] private ThemesManager _themesManager;
    [Inject] private EnergyManager _energyManager;

    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private GameObject makeAVideoPanel;
    [SerializeField] private GameObject manageVideosPanel;
    [SerializeField] private Button makeAVideoButton;
    [SerializeField] private Button manageVideosButton;

    [SerializeField] private Button recordVideoButton;
    //[SerializeField] private Button theme1Button;
    //[SerializeField] private Button theme2Button;
    //[SerializeField] private Button theme3Button;
    //[SerializeField] private GameObject themesScrollView;
    //[SerializeField] private GameObject themeSelecctionBlocker;
    //private TMP_Text lastThemeButtonPressedText;
    //private int lastThemeButtonPressedIndex;

    [SerializeField] private TMP_Text videoNameText;
    [SerializeField] private Button generateVideoName;

    [SerializeField] private Button[] themeSelectionButtons;

    private ThemeType lastThemePressed;
    private ThemeType[] selectedThemes;
    private VideoQuality selectedQuality;

    [SerializeField] private Transform videoInfoHolder;
    [SerializeField] private GameObject videoInfoPrefab;
    private Dictionary<string, GameObject> videosShown = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject skipRecodingPanelPopUp;
    [SerializeField] private Button skipRecodingPopUpCancelButton;

    [SerializeField] private TMP_Text subsText;
    [SerializeField] private TMP_Text uploadedVideosText;

    [SerializeField] private GameObject[] qualitiesTags;
    [SerializeField] private Slider qualitySelector;
    [SerializeField] private Color qualitySelectedColor;
    [SerializeField] private Sprite qualitySelectedImage;
    [SerializeField] private TMP_FontAsset qualitySelectedFont;
    [SerializeField] private Color qualityNonSelectedColor;
    [SerializeField] private Sprite qualityNonSelectedImage;
    [SerializeField] private TMP_FontAsset qualityNonSelectedFont;

    [SerializeField] private TMP_Text[] graphHourTexts;

    private int videoCreationEnergyCost;
    [SerializeField] private GameObject energyCostPanel;
    [SerializeField] private TMP_Text energyCostText;

    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenManageVideosPanel);
        _signalBus.Subscribe<OpenVideoManagerSignal> (InitialState);
        _signalBus.Subscribe<ConfirmThemesSignal> (SetConfirmedThemes);
        _signalBus.Subscribe<EndPublishVideoSignal> (ResetVideoCreationInfo);
        _signalBus.Subscribe<CancelVideoRecordingSignal> (ResetVideoCreationInfo);
        _signalBus.Subscribe<CancelVideoRecordingSignal> (CancelVideoRecording);
        _signalBus.Subscribe<ChangePlayerSubsSignal> (UpdateGlobalSubsFromSignal);
        _signalBus.Subscribe<UpdateThemesGraphSignal> (SetGraphHourTexts);
        _signalBus.Subscribe<ChangeUsernameSignal> (UpdateUsername);

        makeAVideoButton.onClick.AddListener (OpenMakeAVideoPanel);
        manageVideosButton.onClick.AddListener (OpenManageVideosPanel);
        recordVideoButton.onClick.AddListener (OnRecordButtonPressed);

        qualitySelector.onValueChanged.AddListener (SetQualityTag);

        foreach(Button button in themeSelectionButtons)
        {
            button.onClick.AddListener (OpenThemeSelectorPopUp);
        }
        skipRecodingPopUpCancelButton.onClick.AddListener (() =>  OpenSkipRecordginPopUp (false));
        //theme1Button.onClick.AddListener (() => { OnThemeButtonPressed (1, theme1Button.GetComponentInChildren<TMP_Text>());});
        //theme2Button.onClick.AddListener (() => { OnThemeButtonPressed (2, theme2Button.GetComponentInChildren<TMP_Text> ()); });
        //theme3Button.onClick.AddListener (() => { OnThemeButtonPressed (3, theme3Button.GetComponentInChildren<TMP_Text> ()); });
    }
    void InitialState ()
    {
        OpenManageVideosPanel ();
        recordVideoButton.interactable = false;
        energyCostPanel.SetActive (false);
        skipRecodingPanelPopUp.SetActive (false);
        SetQualityTagVisual (0);
        UpdateUsername ();
    }
    void UpdateUsername ()
    {
        if (PlayerDataManager.Instance != null)
        {
            playerNameText.text = PlayerDataManager.Instance.GetPlayerName ().ToUpper ();
        }
    }
    void ResetVideoCreationInfo ()
    {
        Array.Clear (selectedThemes, 0, selectedThemes.Length);
        foreach (Button button in themeSelectionButtons)
        {
            button.GetComponentInChildren<TMP_Text> ().text = "";
            button.transform.GetChild(1).GetComponentInChildren<Image> ().enabled = false;
        }
           
        recordVideoButton.interactable = false;
        energyCostPanel.SetActive (false);
    }
    void UpdateGlobalSubsFromSignal (ChangePlayerSubsSignal signal)
    {
        subsText.text = $"{signal.subs}";
    }
    void UpdateGlobalSubs ()
    {
        subsText.text = $"{PlayerDataManager.Instance.GetSubscribers()}";
    }
    void UpdateGlobalVideos ()
    {
        uploadedVideosText.text = $"{PlayerDataManager.Instance.GetPlayerTotalVideos()}";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void OpenMakeAVideoPanel ()
    {
        if (_youTubeVideoManager.IsRecording ())
            OpenSkipRecordginPopUp (true);
        else
            OpenPanel (VideoManagerPanels.MakeAVideo);
    }
    void OpenSkipRecordginPopUp (bool open)
    {
        skipRecodingPanelPopUp.SetActive (open);
    }
    void OpenManageVideosPanel ()
    {
        OpenPanel (VideoManagerPanels.ManageVideos);
        UpdateGlobalSubs ();
        UpdateGlobalVideos ();
        SetGraphHourTexts ();
    }
    void OpenPanel (VideoManagerPanels _panel)
    {
        if (_panel == VideoManagerPanels.MakeAVideo)
        {
            makeAVideoPanel.SetActive (true);
            _signalBus.Fire<OpenVideoCreationSignal> ();
        }
        else
        {
            makeAVideoPanel.SetActive (false);
            makeAVideoButton.GetComponentInChildren<Image> ().color = Color.white;
            _signalBus.Fire<CloseVideoCreationSignal> ();
        }

        if (_panel == VideoManagerPanels.ManageVideos)
        {
            manageVideosPanel.SetActive (true);
            _signalBus.TryUnsubscribe<OnVideosStatsUpdatedSignal> (UpdateVideoList);
            _signalBus.Subscribe<OnVideosStatsUpdatedSignal> (UpdateVideoList);
        }
        else
        {
            manageVideosPanel.SetActive (false);
            _signalBus.TryUnsubscribe<OnVideosStatsUpdatedSignal> (UpdateVideoList);
            manageVideosButton.GetComponentInChildren<Image> ().color = Color.white;

        }
    }

    void OnRecordButtonPressed ()
    {
        if (_energyManager.GetEnergy () <= videoCreationEnergyCost || selectedThemes.Length==0)
            return;

        _signalBus.Fire<StartRecordingSignal> (new StartRecordingSignal ()
        {
            recordedThemes = selectedThemes,
            videoName = _youTubeVideoManager.GetVideoNameByTheme (selectedThemes)
            //Dummy set quality selected for video here too
        });
        _signalBus.Fire<AddEnergySignal> (new AddEnergySignal () { energyAddition = -videoCreationEnergyCost });
        StartRecordingVideo ();
    }
    
    void StartRecordingVideo ()
    {
        //Create video info in the video manager screen
        CreateVideoToRecord ();
        //Change to video manager screen
        OpenManageVideosPanel ();
    }
    void CreateVideoToRecord ()
    {
        GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);
        string newVideoName = _youTubeVideoManager.GetVideoNameByTheme (selectedThemes);
        VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
        vc.SetReferences (_signalBus, _youTubeVideoManager);
        vc.SetVideoInfoUp (newVideoName,
                            3f,
                            selectedThemes,
                            selectedQuality
                            );
        videosShown.Add (newVideoName, videoInfoObject);
    }
    void CreateVideo (Video video)
    {
        GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);
        VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
        vc.SetReferences (_signalBus, _youTubeVideoManager);
        vc.SetVideoInfoUp (video);
        videosShown.Add (video.name, videoInfoObject);
    }

    void UpdateVideoList ()
    {
        Debug.Log ("Update videos");
        Video[] playerVideos = PlayerDataManager.Instance.GetVideos ().ToArray ();
        foreach (Video video in playerVideos)
        {
            if (videosShown.ContainsKey (video.name))
            {
                videosShown[video.name].GetComponent<VideoInfo_VC> ().UpdateVideoInfo ();
            }
            else
            {
                CreateVideo (video);
            }
        }
    }

    void OpenThemeSelectorPopUp()
    {
        _signalBus.Fire<OpenThemeSelectorPopUpSignal> ();
    }

    void SetConfirmedThemes (ConfirmThemesSignal signal)
    {
        for(int i=0; i < themeSelectionButtons.Length; i++)
        {
            if (signal.selectedThemesSlots.ContainsKey (i))
            {
                ThemeType themeType = signal.selectedThemesSlots[i];
                themeSelectionButtons[i].GetComponentInChildren<TMP_Text>().text = string.Concat (Enum.GetName (themeType.GetType (), themeType).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
                themeSelectionButtons[i].transform.GetChild (1).GetComponentInChildren<Image> ().enabled = false;
            }
            else
            {
                themeSelectionButtons[i].GetComponentInChildren<TMP_Text> ().text = "";
                themeSelectionButtons[i].transform.GetChild (1).GetComponentInChildren<Image> ().enabled = true;
            }
        }
        selectedThemes = signal.selectedThemesSlots.Values.ToArray();

        if(selectedThemes.Length > 0)
        {
            recordVideoButton.interactable = true;
            energyCostPanel.SetActive (true);
        } 
        else
        {
            recordVideoButton.interactable = false;
            energyCostPanel.SetActive (false);
        }
            
    }

    void CancelVideoRecording (CancelVideoRecordingSignal signal)
    {
        videosShown.Remove (signal.name);
    }
    void SetQualityTag (float value)
    {
        float qualityStep = 1f / Enum.GetValues (typeof(VideoQuality)).Length;
        int qualityTagIndex = (int)(value / qualityStep);
        selectedQuality = (VideoQuality)qualityTagIndex+1;
        SetQualityTagVisual (qualityTagIndex);
        videoCreationEnergyCost = _energyManager.GetVideoEnergyCost (selectedQuality);
        energyCostText.text = videoCreationEnergyCost.ToString();
    }
    void SetQualityTagVisual (int index)
    {
        Debug.Log (index);
        int i = 0;
        foreach(GameObject qualityTag in qualitiesTags)
        {
            if(i == index)
            {
                qualityTag.GetComponentInChildren<Image> ().sprite = qualitySelectedImage;
                qualityTag.GetComponentInChildren<TMP_Text> ().color = qualitySelectedColor;
                qualityTag.GetComponentInChildren<TMP_Text> ().font = qualitySelectedFont;
            }       
            else
            {
                qualityTag.GetComponentInChildren<Image> ().sprite = qualityNonSelectedImage;
                qualityTag.GetComponentInChildren<TMP_Text> ().color = qualityNonSelectedColor;
                qualityTag.GetComponentInChildren<TMP_Text> ().font = qualityNonSelectedFont;
            }
            i++;
        }
    }

    void SetGraphHourTexts ()
    {
        int hourPeriod = Math.DivRem (GameClock.Instance.Now.Hour-1, 6, out int remaindesr);
        int i = 0;
        foreach (TMP_Text text in graphHourTexts)
        {
            text.text = $"{i + hourPeriod*6}:00";
            i++;
        }
    }
}
