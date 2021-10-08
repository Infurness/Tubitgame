using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class VideoPostProductionViewController : MonoBehaviour
{
    [Inject] private SignalBus _SignalBus;
    [Inject] private YouTubeVideoManager youTubeVideoManager;

    [SerializeField] private GameObject postProductionPanel;
    [SerializeField] private TMP_Text videoName;
    [SerializeField] private TMP_Text videoViews;
    [SerializeField] private TMP_Text videoLikes;
    [SerializeField] private TMP_Text videoComments;
    [SerializeField] private TMP_Text videoNewSubscribers;

    [SerializeField] private Button backToRecordingButton; //Dummy: just for testing purposes

    void Start ()
    {
        _SignalBus.Subscribe<StartPublishSignal> (PublishVideo);
        backToRecordingButton.onClick.AddListener (BackToThemeView);
    }

    void Update()
    {
        
    }

    void PublishVideo ()
    {
        postProductionPanel.SetActive (true);
        Debug.Log ("Video published");

        Video dummyVideo = new Video ();
        dummyVideo.name = "Dummy video";
        dummyVideo.views = 1234;
        dummyVideo.likes = 82;
        dummyVideo.comments = 30;
        dummyVideo.newSubscribers = 4;
        ShowVideoStats (youTubeVideoManager.GetVideoByName("DummyVideoName"));
    }

    void ShowVideoStats (Video _video)
    {
        if (_video == null)
            return;
        videoName.text = _video.name;
        videoViews.text = $"Views: {_video.views}";
        videoLikes.text = $"Likes: {_video.likes}";
        videoComments.text = $"Comments: {_video.comments}";
        videoNewSubscribers.text = $"Subscribers: {_video.newSubscribers}";
    }

    void BackToThemeView ()
    {
        postProductionPanel.SetActive (false);
        _SignalBus.Fire<OpenThemeSelectionSignal> ();
    }
}
