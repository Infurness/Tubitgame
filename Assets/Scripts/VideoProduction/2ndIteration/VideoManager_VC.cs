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
    [SerializeField] private Button theme1Button;
    [SerializeField] private Button theme2Button;
    [SerializeField] private Button theme3Button;
    [SerializeField] private GameObject themesScrollView;
    private TMP_Text lastThemeButtonPressedText;
    private List<ThemeType> selectedThemes = new List<ThemeType>();
    [SerializeField] private GameObject themeButtonsHolder;
    [SerializeField] private GameObject themeButtonPrefab;

    [SerializeField] private Transform videoInfoHolder;
    [SerializeField] private GameObject videoInfoPrefab;

    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenManageVideosPanel);
        _signalBus.Subscribe<EndPublishVideoSignal> (CreateVideo);

        makeAVideoButton.onClick.AddListener (OpenMakeAVideoPanel);
        manageVideosButton.onClick.AddListener (OpenManageVideosPanel);
        recordVideoButton.onClick.AddListener (OnRecordButtonPressed);
        theme1Button.onClick.AddListener (() => { OnThemeButtonPressed (theme1Button.GetComponentInChildren<TMP_Text>());});
        theme2Button.onClick.AddListener (() => { OnThemeButtonPressed (theme2Button.GetComponentInChildren<TMP_Text> ()); });
        theme3Button.onClick.AddListener (() => { OnThemeButtonPressed (theme3Button.GetComponentInChildren<TMP_Text> ()); });

        InitialState ();
    }
    void InitialState ()
    {
        OpenMakeAVideoPanel ();
        themesScrollView.SetActive (false);
        SetUpThemeButtons ();
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
            makeAVideoPanel.SetActive (true);
        else
            makeAVideoPanel.SetActive (false);

        if (_panel == VideoManagerPanels.ManageVideos)
            manageVideosPanel.SetActive (true);
        else
            manageVideosPanel.SetActive (false);
    }

    void OnRecordButtonPressed ()
    {
        if (_energyManager.GetEnergy () < 30)
            return;

        _signalBus.Fire<StartRecordingSignal> (new StartRecordingSignal ()
        {
            recordingTime = 3f,
            recordedThemes = selectedThemes.ToArray ()
        });
        _signalBus.Fire<AddEnergySignal> (new AddEnergySignal () { energyAddition = -30 });
    }
    void SetUpThemeButtons ()
    {
        foreach (ThemeType themeType in _themesManager.GetThemes ())
        {
            CreateThemeButton (themeType);
        }
    }
    void CreateThemeButton (ThemeType _themeType)
    {
        GameObject button = Instantiate (themeButtonPrefab, themeButtonsHolder.transform);
        button.GetComponent<ButtonThemePreProductionView> ().themeType = _themeType;
        button.GetComponent<Button> ().onClick.AddListener (() => OnThemeSelected (button.GetComponentInChildren<TMP_Text>().text));
    }
    void OnThemeButtonPressed (TMP_Text _themeText)
    {
        lastThemeButtonPressedText = _themeText;
        themesScrollView.SetActive (true);
    }
    void OnThemeSelected (string _themeName)
    {
        _themeName = string.Concat (_themeName.Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
        lastThemeButtonPressedText.text = _themeName;
        themesScrollView.SetActive (false);
    }
    void CreateVideo (EndPublishVideoSignal _signal)
    {
        GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);
        VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
        vc.SetReferences (_signalBus, _youTubeVideoManager);
        vc.SetVideoInfoUp (_signal.videoName);
    }
}
