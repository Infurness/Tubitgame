using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class YouTubeVideoManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private PlayerDataManager playerDataManger;
    [Inject] private AlgorithmManager algorithmManager;
    [Inject] private ThemesManager themesManager;

    void Start()
    {
        _signalBus.Subscribe<PublishVideoSignal> (CreateVideo);
    }

    void Update()
    {
        
    }

    private void CreateVideo (PublishVideoSignal signal)
    {
        string videoName = signal.videoName;
        ThemeType[] videoThemes = signal.videoThemes;
        Video newVideo = new Video (GameClock.Instance.Now);

        newVideo.name = videoName;
        newVideo.themes = videoThemes;
        newVideo.quality = playerDataManger.GetQuality();
        var subscribers = playerDataManger.GetSubscribers();
        List<float> themeValues = new List<float> ();

        ulong videoViews = algorithmManager.GetVideoViews 
                            (
                             playerDataManger.GetSubscribers (), 
                             videoThemes, 
                             playerDataManger.GetQuality ());

        newVideo.maxViews = videoViews;
        newVideo.maxLikes = algorithmManager.GetVideoLikes(videoViews, playerDataManger.GetQuality ());
        newVideo.maxComments = algorithmManager.GetVideoComments(videoViews);
        newVideo.maxNewSubscribers = algorithmManager.GetVideoSubscribers(videoViews, playerDataManger.GetQuality ());
        newVideo.maxMoney = algorithmManager.GetVideoMoney ();
        newVideo.lifeTimeHours = Random.Range(1, 2);
        newVideo.lastUpdateTime = GameClock.Instance.Now;
        playerDataManger.AddVideo (newVideo);

        _signalBus.Fire<EndPublishVideoSignal> (new EndPublishVideoSignal () {videoName = videoName });
    }
    public string GetVideoNameByTheme (ThemeType[] _themeTypes)
    {
        string videoName ="";
        foreach(ThemeType themeType in _themeTypes)
        {
            videoName += Enum.GetName (themeType.GetType (), themeType);
        }
        videoName += $" {GetNumberOfVideoByThemes (_themeTypes)}";
        return videoName;
    }
    int GetNumberOfVideoByThemes (ThemeType[] _themeTypes)
    {
        if (playerDataManger==null)
        {
            print("Player DataManger is Null");
        }
        return playerDataManger.GetNumberOfVideoByThemes (_themeTypes);
    }
    public Video GetVideoByName (string _name)
    {
        return playerDataManger.GetVideoByName (_name);
    }
    public int GetVideoMoneyByName (string _name)
    {
        return GetVideoByName (_name).money;
    }
    public int RecollectVideoMoney (string _name)
    {
        return playerDataManger.RecollectVideoMoney (_name);
    }
    int GetTimeHour ()
    {
        return GameClock.Instance.Now.Hour;
    }
}
