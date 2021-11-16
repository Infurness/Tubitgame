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

    bool isRecording;

    void Start()
    {
        _signalBus.Subscribe<PublishVideoSignal> (CreateVideo);
        _signalBus.Subscribe<StartRecordingSignal> (() => SetIsRecording (true));
        _signalBus.Subscribe<CancelVideoRecordingSignal> (() => SetIsRecording (false));
        _signalBus.Subscribe<GetMoneyFromVideoSignal> (RecollectVideoMoney);
    }

    void Update()
    {
        
    }
    public bool IsRecording ()
    {
        return isRecording;
    }
    void SetIsRecording (bool recording)
    {
        isRecording = recording;
    }
    private void CreateVideo (PublishVideoSignal signal)
    {
        string videoName = signal.videoName;
        ThemeType[] videoThemes = signal.videoThemes;
        Video newVideo = new Video (GameClock.Instance.Now);
        newVideo.name = videoName;
        newVideo.themes = (ThemeType[]) videoThemes.Clone();
        newVideo.quality = playerDataManger.GetQuality();
        if (Random.Range (0, 101) >= 95) //5% chance of being viral
        {
            newVideo.isViral = true;
        }
        var subscribers = playerDataManger.GetSubscribers();
        List<float> themeValues = new List<float> ();

        ulong videoViews = algorithmManager.GetVideoViews 
                                (
                                playerDataManger.GetSubscribers (), 
                                videoThemes, 
                                playerDataManger.GetQuality (),
                                newVideo.isViral
                                );

        newVideo.maxViews = videoViews;
        newVideo.maxLikes = algorithmManager.GetVideoLikes(videoViews, playerDataManger.GetQuality ());
        newVideo.maxComments = algorithmManager.GetVideoComments(videoViews);
        newVideo.maxNewSubscribers = algorithmManager.GetVideoSubscribers(videoViews, playerDataManger.GetQuality ());
        newVideo.videoMaxSoftCurrency = algorithmManager.GetVideoSoftCurrency(videoViews);
        newVideo.lifeTimeDays = Random.Range(1, 2);
        newVideo.lastUpdateTime = GameClock.Instance.Now;
        playerDataManger.AddVideo (newVideo);

        isRecording = false;
        _signalBus.Fire<EndPublishVideoSignal> ();
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
    public ulong GetVideoMoneyByName (string _name)
    {
        return GetVideoByName (_name).videoSoftCurrency;
    }
    void RecollectVideoMoney (GetMoneyFromVideoSignal signal)
    {
        playerDataManger.RecollectVideoMoney (signal.videoName);
        _signalBus.Fire<UpdateSoftCurrency> ();
    }
    int GetTimeHour () //Dummy Not being used
    {
        return GameClock.Instance.Now.Hour;
    }
    
}
