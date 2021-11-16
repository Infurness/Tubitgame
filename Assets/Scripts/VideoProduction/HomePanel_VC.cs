using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HomePanel_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private YouTubeVideoManager youTubeVideoManager;

    //[SerializeField] private Button publishButton;
   // [SerializeField] private ScrollRect viewsScroll;
    [SerializeField] private Button playerIconButton;

    ThemeType[] selectedThemeTypes; //Dummy unused code
    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<StartRecordingSignal> (StartRecording);
       // publishButton.onClick.AddListener (OnPublishVideoPressed);
     //   viewsScroll.onValueChanged.AddListener (OnViewsScroll);
        playerIconButton.onClick.AddListener (OpenSettingsPanel);
       // closeSettingsButton.onClick.AddListener (OpenSettingsPanel );
   //     viewsScroll.onValueChanged.AddListener (OnViewsScroll);
        playerIconButton.onClick.AddListener (OpenSettingsPanel);

        InitialScreenState ();
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
}
