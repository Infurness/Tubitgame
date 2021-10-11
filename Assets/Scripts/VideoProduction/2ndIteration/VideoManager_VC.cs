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
    [Inject] ThemesManager themesManager;

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
    [SerializeField] private GameObject themeButtonsHolder;
    [SerializeField] private GameObject themeButtonPrefab;

    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenManageVideosPanel);

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
    void OpenPanel (VideoManagerPanels panel)
    {
        if (panel == VideoManagerPanels.MakeAVideo)
            makeAVideoPanel.SetActive (true);
        else
            makeAVideoPanel.SetActive (false);

        if (panel == VideoManagerPanels.ManageVideos)
            manageVideosPanel.SetActive (true);
        else
            manageVideosPanel.SetActive (false);
    }

    void OnRecordButtonPressed ()
    {
        //if (energyManager.GetEnergy () < 30)
        //    return;


        _signalBus.Fire<StartRecordingSignal> (new StartRecordingSignal ()
        {
            recordingTime = 3f
            //recordedThemes = selectedThemes.ToArray ()
        });
        _signalBus.Fire<AddEnergySignal> (new AddEnergySignal () { energyAddition = -30 });
    }
    void SetUpThemeButtons ()
    {
        foreach (ThemeType themeType in themesManager.GetThemes ())
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
    void OnThemeButtonPressed (TMP_Text themeText)
    {
        lastThemeButtonPressedText = themeText;
        themesScrollView.SetActive (true);
    }
    void OnThemeSelected (string themeName)
    {
        themeName = string.Concat (themeName.Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
        lastThemeButtonPressedText.text = themeName;
        themesScrollView.SetActive (false);
    }
}
