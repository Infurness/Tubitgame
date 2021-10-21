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

    [SerializeField] private GameObject makeAVideoPanel;
    [SerializeField] private GameObject manageVideosPanel;
    [SerializeField] private Button makeAVideoButton;
    [SerializeField] private Button manageVideosButton;

    [SerializeField] private Button recordVideoButton;
    bool isRecording;
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


    [SerializeField] private Transform videoInfoHolder;
    [SerializeField] private GameObject videoInfoPrefab;
    private Dictionary<string, GameObject> videosShown = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenManageVideosPanel);
        _signalBus.Subscribe<EndPublishVideoSignal> (CreateVideo);
        _signalBus.Subscribe<OpenVideoManagerSignal> (InitialState);
        _signalBus.Subscribe<ConfirmThemesSignal> (SetConfirmedThemes);
        _signalBus.Subscribe<EndPublishVideoSignal> (ResetVideoCreationInfo);

        makeAVideoButton.onClick.AddListener (OpenMakeAVideoPanel);
        manageVideosButton.onClick.AddListener (OpenManageVideosPanel);
        recordVideoButton.onClick.AddListener (OnRecordButtonPressed);
        foreach(Button button in themeSelectionButtons)
        {
            button.onClick.AddListener (OpenThemeSelectorPopUp);
        }
        //theme1Button.onClick.AddListener (() => { OnThemeButtonPressed (1, theme1Button.GetComponentInChildren<TMP_Text>());});
        //theme2Button.onClick.AddListener (() => { OnThemeButtonPressed (2, theme2Button.GetComponentInChildren<TMP_Text> ()); });
        //theme3Button.onClick.AddListener (() => { OnThemeButtonPressed (3, theme3Button.GetComponentInChildren<TMP_Text> ()); });
    }
    void InitialState ()
    {
        OpenManageVideosPanel ();
        recordVideoButton.interactable = false;
    }
    void ResetVideoCreationInfo ()
    {
        Array.Clear (selectedThemes, 0, selectedThemes.Length);
        foreach (Button button in themeSelectionButtons)
            button.GetComponentInChildren<TMP_Text> ().text = "+";
        recordVideoButton.interactable = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void OpenMakeAVideoPanel ()
    {
        OpenPanel (VideoManagerPanels.MakeAVideo);
    }
    void OpenManageVideosPanel ()
    {
        OpenPanel (VideoManagerPanels.ManageVideos);
    }
    void OpenPanel (VideoManagerPanels _panel)
    {
        if (_panel == VideoManagerPanels.MakeAVideo)
        {
            makeAVideoPanel.SetActive (true);
        }
        else
        {
            makeAVideoPanel.SetActive (false);
            makeAVideoButton.GetComponentInChildren<Image> ().color = Color.white;

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
        if (_energyManager.GetEnergy () < 30 || selectedThemes.Length==0)
            return;

        _signalBus.Fire<StartRecordingSignal> (new StartRecordingSignal ()
        {
            recordingTime = 3f,
            recordedThemes = selectedThemes
        });
        _signalBus.Fire<AddEnergySignal> (new AddEnergySignal () { energyAddition = -30 });
    }


    void OpenThemeSelectorPopUp()
    {
        _signalBus.Fire<OpenThemeSelectorPopUpSignal> ();
    }

    //void OpenThemeSelector (bool open)
    //{
    //    themesScrollView.SetActive (open);
    //    themeSelecctionBlocker.SetActive (open);
    //    if (open)
    //        StartCoroutine (CloseThemeSelector ());
    //    else
    //        StopAllCoroutines ();
    //}
    //void OnThemeButtonPressed (int buttonIndex, TMP_Text _themeText)
    //{
    //    lastThemeButtonPressedIndex = buttonIndex;
    //    lastThemeButtonPressedText = _themeText;
    //    OpenThemeSelector (true);
    //}
    //void OnThemeSelected (ThemeType _themeType, string _themeName)
    //{
    //    _themeName = string.Concat (_themeName.Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
    //    lastThemeButtonPressedText.text = _themeName;
    //    selectedThemes[lastThemeButtonPressedIndex] = _themeType;
    //    recordVideoButton.interactable = true;
    //    OpenThemeSelector (false);
    //}
    void CreateVideo (EndPublishVideoSignal _signal)
    {
        GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);
        VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
        vc.SetReferences (_signalBus, _youTubeVideoManager);
        vc.SetVideoInfoUp (_signal.videoName);
        videosShown.Add (_signal.videoName, videoInfoObject);
    }

    void UpdateVideoList ()
    {
        Debug.Log ("Update videos");
        Video[] playerVideos = PlayerDataManager.Instance.GetVideos ().ToArray();
        foreach(Video video in playerVideos)
        {
            if (videosShown.ContainsKey (video.name))
            {
                videosShown[video.name].GetComponent<VideoInfo_VC> ().UpdateVideoInfo ();
            }
            else
            {
                CreateVideo (new EndPublishVideoSignal { videoName = video.name});
            }
        }
    }

    //IEnumerator CloseThemeSelector ()
    //{
    //    yield return WaitForAnyInput();
    //    OpenThemeSelector (false);
    //}

    //IEnumerator WaitForAnyInput ()
    //{
    //    bool done = false;
    //    while(!done)
    //    {
    //        Vector2 mousePos = Input.mousePosition;
    //        if (Input.anyKeyDown && !RectTransformUtility.RectangleContainsScreenPoint (themesScrollView.GetComponent<RectTransform> (), mousePos))
    //            done = true;
    //        yield return null;
    //    }
    //}
    void SetConfirmedThemes (ConfirmThemesSignal signal)
    {
        for(int i=0; i < themeSelectionButtons.Length; i++)
        {
            if (signal.selectedThemesSlots.ContainsKey (i))
            {
                ThemeType themeType = signal.selectedThemesSlots[i];
                themeSelectionButtons[i].GetComponentInChildren<TMP_Text>().text = string.Concat (Enum.GetName (themeType.GetType (), themeType).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
            }
            else
            {
                themeSelectionButtons[i].GetComponentInChildren<TMP_Text> ().text = "+";
            }
        }
        selectedThemes = signal.selectedThemesSlots.Values.ToArray();

        if(selectedThemes.Length > 0)
            recordVideoButton.interactable = true;
        else
            recordVideoButton.interactable = false;
    }
}
