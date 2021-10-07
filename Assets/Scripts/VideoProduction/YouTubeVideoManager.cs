using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class YouTubeVideoManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private PlayerDataManager playerDataManger;
    [Inject] private AlgorithmManager algorithmManager;

    void Start()
    {
        playerDataManger.gameObject.SetActive (false);
        _signalBus.Subscribe<PublishVideoSignal> (CreateVideo);
    }

    void Update()
    {
        
    }

    private void CreateVideo (PublishVideoSignal signal)
    {
        string videoName = signal.videoName;
        Theme[] videoThemes = signal.videoThemes;
        Video newVideo = new Video ();

        newVideo.name = videoName;
        newVideo.themes = videoThemes;
        newVideo.quality = playerDataManger.GetQuality();

        List<float> themeValues = new List<float> ();
        foreach(Theme theme in videoThemes)
        {
            themeValues.Add (theme.popularity);
        }
        ulong videoViews = algorithmManager.GetVideoViews 
                            (800, 
                             playerDataManger.GetSubscribers (), 
                             themeValues.ToArray (), 
                             playerDataManger.GetQuality ());

        newVideo.views = videoViews;
        newVideo.likes = algorithmManager.GetVideoLikes(videoViews, playerDataManger.GetQuality ());
        newVideo.comments = algorithmManager.GetVideoComments(videoViews);
        newVideo.newSubscribers = algorithmManager.GetVideoSubscribers(videoViews, playerDataManger.GetQuality ());

        playerDataManger.AddVideo (newVideo);
    }
    public Video GetVideoByName (string _name)
    {
        return playerDataManger.GetVideoByName (_name);
    }
}
