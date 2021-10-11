using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HomePanel_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;

    [SerializeField] private Button publishButton;
    [SerializeField] private Image videoProgressBar;

    // Start is called before the first frame update
    void Start()
    {
        _signalBus.Subscribe<StartRecordingSignal> (StartRecording);

        publishButton.onClick.AddListener (OnPublishVideoPressed);

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
    void StartRecording (StartRecordingSignal recordingSignal)
    {
        StopAllCoroutines ();
        StartCoroutine (FillTheRecordImage (recordingSignal.recordingTime));   
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
    }
}
