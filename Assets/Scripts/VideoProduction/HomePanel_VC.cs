using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HomePanel_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private YouTubeVideoManager youTubeVideoManager;

    [SerializeField] private Button publishButton;
    [SerializeField] private Image videoProgressBar;
    [SerializeField] private ScrollRect viewsScroll;

    ThemeType[] selectedThemeTypes;
    // Start is called before the first frame update
    void Start()
    {
        _signalBus.Subscribe<StartRecordingSignal> (StartRecording);

        publishButton.onClick.AddListener (OnPublishVideoPressed);
        viewsScroll.onValueChanged.AddListener (OnViewsScroll);

        InitialScreenState ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitialScreenState ()
    {
        videoProgressBar.gameObject.SetActive (false);
        publishButton.gameObject.SetActive (false);
    }
    void StartRecording (StartRecordingSignal _recordingSignal)
    {
        selectedThemeTypes = _recordingSignal.recordedThemes;
        StopAllCoroutines ();
        StartCoroutine (FillTheRecordImage (_recordingSignal.recordingTime));   
    }

    IEnumerator FillTheRecordImage (float time)
    {
        videoProgressBar.gameObject.SetActive (true);
        float tACC = 0;
        while (tACC < time)
        {
            yield return new WaitForEndOfFrame ();
            tACC += Time.deltaTime;
            videoProgressBar.fillAmount = tACC / time;
        }
        videoProgressBar.gameObject.SetActive (false);
        publishButton.gameObject.SetActive (true);
    }

    void OnPublishVideoPressed ()
    {
        publishButton.gameObject.SetActive (false);
        PublishVideo ();
    }
    void PublishVideo ()
    {
        _signalBus.Fire<ShowVideosStatsSignal> (new ShowVideosStatsSignal ());     
        _signalBus.Fire<PublishVideoSignal> (new PublishVideoSignal () { videoName = youTubeVideoManager.GetVideoNameByTheme(selectedThemeTypes), videoThemes = selectedThemeTypes});
    }
    void OnViewsScroll (Vector2 vector)
    {
        if(vector!=Vector2.zero)
        {
            //Debug.Log (viewsScroll.content.position);
            //Debug.Log (viewsScroll.content.position);
        }

    }
}
