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
    [SerializeField] private Button theme1Button;
    [SerializeField] private Button theme2Button;
    [SerializeField] private Button theme3Button;
    [SerializeField] private GameObject themesScrollView;
    [SerializeField] private GameObject themeSelecctionBlocker;
    private TMP_Text lastThemeButtonPressedText;
    private int lastThemeButtonPressedIndex;
    private Dictionary<int,ThemeType> selectedThemes = new Dictionary<int,ThemeType>();
    [SerializeField] private GameObject themeButtonsHolder;
    [SerializeField] private GameObject themeButtonPrefab;

    [SerializeField] private Transform videoInfoHolder;
    [SerializeField] private GameObject videoInfoPrefab;

    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenManageVideosPanel);
        _signalBus.Subscribe<EndPublishVideoSignal> (CreateVideo);
        _signalBus.Subscribe<OpenVideoManager> (InitialState);

        makeAVideoButton.onClick.AddListener (OpenMakeAVideoPanel);
        manageVideosButton.onClick.AddListener (OpenManageVideosPanel);
        recordVideoButton.onClick.AddListener (OnRecordButtonPressed);
        theme1Button.onClick.AddListener (() => { OnThemeButtonPressed (1, theme1Button.GetComponentInChildren<TMP_Text>());});
        theme2Button.onClick.AddListener (() => { OnThemeButtonPressed (2, theme2Button.GetComponentInChildren<TMP_Text> ()); });
        theme3Button.onClick.AddListener (() => { OnThemeButtonPressed (3, theme3Button.GetComponentInChildren<TMP_Text> ()); });
    }
    void InitialState ()
    {
        OpenManageVideosPanel ();
        themesScrollView.SetActive (false);
        SetUpThemeButtons ();
        recordVideoButton.interactable = false;
        OpenThemeSelector (false);
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
        }
        else
        {
            manageVideosPanel.SetActive (false);
            manageVideosButton.GetComponentInChildren<Image> ().color = Color.white;

        }
    }

    void OnRecordButtonPressed ()
    {
        if (_energyManager.GetEnergy () < 30 || selectedThemes.Count==0)
            return;
        recordVideoButton.interactable = false;
        _signalBus.Fire<StartRecordingSignal> (new StartRecordingSignal ()
        {
            recordingTime = 3f,
            recordedThemes = selectedThemes.Values.ToArray()
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
        button.GetComponent<Button> ().onClick.AddListener (() => OnThemeSelected (_themeType, button.GetComponentInChildren<TMP_Text>().text));
    }
    void OpenThemeSelector (bool open)
    {
        themesScrollView.SetActive (open);
        themeSelecctionBlocker.SetActive (open);
        if (open)
            StartCoroutine (CloseThemeSelector ());
        else
            StopAllCoroutines ();
    }
    void OnThemeButtonPressed (int buttonIndex, TMP_Text _themeText)
    {
        lastThemeButtonPressedIndex = buttonIndex;
        lastThemeButtonPressedText = _themeText;
        OpenThemeSelector (true);
    }
    void OnThemeSelected (ThemeType _themeType, string _themeName)
    {
        _themeName = string.Concat (_themeName.Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
        lastThemeButtonPressedText.text = _themeName;
        selectedThemes[lastThemeButtonPressedIndex] = _themeType;
        recordVideoButton.interactable = true;
        OpenThemeSelector (false);
    }
    void CreateVideo (EndPublishVideoSignal _signal)
    {
        theme1Button.GetComponentInChildren<TMP_Text> ().text = "Theme 1";
        theme2Button.GetComponentInChildren<TMP_Text> ().text = "Theme 2";
        theme3Button.GetComponentInChildren<TMP_Text> ().text = "Theme 3";
        selectedThemes.Clear ();
        GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);
        VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
        vc.SetReferences (_signalBus, _youTubeVideoManager);
        vc.SetVideoInfoUp (_signal.videoName);
    }

    IEnumerator CloseThemeSelector ()
    {
        yield return WaitForAnyInput();
        OpenThemeSelector (false);
    }

    IEnumerator WaitForAnyInput ()
    {
        bool done = false;
        while(!done)
        {
            Vector2 mousePos = Input.mousePosition;
            if (Input.anyKeyDown && !RectTransformUtility.RectangleContainsScreenPoint (themesScrollView.GetComponent<RectTransform> (), mousePos))
                done = true;
            yield return null;
        }
    }
}
