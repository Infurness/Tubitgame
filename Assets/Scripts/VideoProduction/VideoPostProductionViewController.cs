using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class VideoPostProductionViewController : MonoBehaviour
{
    [Inject] private SignalBus _SignalBus;
    [SerializeField] private GameObject postProductionPanel;

    [SerializeField] private TMP_Text videoName;
    [SerializeField] private TMP_Text videoViews;
    [SerializeField] private TMP_Text videoLikes;
    [SerializeField] private TMP_Text videoComments;
    [SerializeField] private TMP_Text videoNewSubscribers;

    void Start ()
    {
        _SignalBus.Subscribe<StartPublishSignal> (PublishVideo);
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
        ShowVideoStats (dummyVideo);
    }

    void ShowVideoStats (Video _video)
    {
        videoName.text = _video.name;
        videoViews.text = $"Views: {_video.views}";
        videoLikes.text = $"Likes: {_video.likes}";
        videoComments.text = $"Comments: {_video.comments}";
        videoNewSubscribers.text = $"Subscribers: {_video.newSubscribers}";
    }
}
