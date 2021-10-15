using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
        Video newVideo = new Video ();

        newVideo.name = videoName;
        newVideo.themes = videoThemes;
        newVideo.quality = playerDataManger.GetQuality();

        List<float> themeValues = new List<float> ();
        foreach(ThemeType themeType in videoThemes)
        {
            themeValues.Add(themesManager.GetThemePopularity (themeType, GetTimeHour ()));
        }

        ulong videoViews = algorithmManager.GetVideoViews 
                            (800, 
                             playerDataManger.GetSubscribers (), 
                             themeValues.ToArray (), 
                             playerDataManger.GetQuality ());

        newVideo.views = videoViews * (ulong)algorithmManager.GetVirality ();
        newVideo.likes = algorithmManager.GetVideoLikes(videoViews, playerDataManger.GetQuality ());
        newVideo.comments = algorithmManager.GetVideoComments(videoViews);
        newVideo.newSubscribers = algorithmManager.GetVideoSubscribers(videoViews, playerDataManger.GetQuality ());
        newVideo.money = algorithmManager.GetVideoMoney ();

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
        return System.DateTime.Now.Hour;
    }
}
