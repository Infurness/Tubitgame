using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class NameInfoByTheme
{
    public ThemeType themeType;
    public VideoNameWords words;
}
[Serializable]
public class VideoNameWords
{
    public string[] noun1;
    public string[] noun2;
    public string[] adjetive;
}
public class YouTubeVideoManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private AlgorithmManager algorithmManager;
    [Inject] private ThemesManager themesManager;
    [Inject] private EnergyManager energyManager;
    [Inject] private ExperienceManager experienceManager;
    [Inject] private GameAnalyticsManager gameAnalyticsManager;
    bool isRecording;

    [SerializeField] NameInfoByTheme[] videoNamingInfo;

    void Start()
    {
        _signalBus.Subscribe<PublishVideoSignal> (CreateVideo);
        _signalBus.Subscribe<StartRecordingSignal> (() => SetIsRecording (true));
        _signalBus.Subscribe<CancelVideoRecordingSignal> (() => SetIsRecording (false));
        _signalBus.Subscribe<GetMoneyFromVideoSignal> (RecollectVideoMoney);
        _signalBus.Subscribe<LevelUpSignal> (playerDataManager.GetLevelUpRewards);
        _signalBus.Subscribe<CancelVideoRecordingSignal> ((signal) => DeleteUnpublishedVideo (signal.name));
    }

    void Update()
    {
        
    }
    public bool IsRecording ()
    {
        return isRecording;
    }
    public void SetIsRecording (bool recording)
    {
        print("iS Recorded S");
        isRecording = recording;
    }
   
    private void CreateVideo (PublishVideoSignal signal)
    {
        string videoName = signal.videoName;
        ThemeType[] videoThemes = signal.videoThemes;
        Video newVideo = new Video (GameClock.Instance.Now);
        newVideo.name = videoName;
        newVideo.themes = (ThemeType[]) videoThemes.Clone();
        newVideo.selectedQuality = signal.videoSelectedQuality;
        newVideo.quality = playerDataManager.GetQuality(); //Dummy not yet implement lacks of themes quality and selected quality

        int level = experienceManager.GetPlayerLevel();
        int percentage = 5;
        switch (level)
        {
            case 1:
                percentage = 4;
                break;
            case 2:
                percentage = 7;
                break;
            case 3:
                percentage = 9;
                break;
            case 4:
                percentage = 11;
                break;
            case 5:
                percentage = 14;
                break;
            case 6:
                percentage = 17;
                break;
            case 7:
                percentage = 19;
                break;
            case 8:
                percentage = 21;
                break;
            case 9:
                percentage = 18;
                break;
            case 10:
                percentage = 17;
                break;
        }

        if (Random.Range (0, 101) >= 100 - percentage) //5% chance of being viral
        {
            newVideo.isViral = true;
            gameAnalyticsManager.SendCustomEvent("viral_event");
        }
        var subscribers = playerDataManager.GetSubscribers();
        List<float> themeValues = new List<float> ();

        ulong videoViews = algorithmManager.GetVideoViews 
                                (
                                playerDataManager.GetSubscribers (), 
                                videoThemes, 
                                playerDataManager.GetQuality (),
                                newVideo.isViral
                                );

        newVideo.maxViews = videoViews;
        newVideo.maxLikes = algorithmManager.GetVideoLikes(videoViews, playerDataManager.GetQuality ());
        newVideo.maxComments = algorithmManager.GetVideoComments(videoViews);
        newVideo.maxNewSubscribers = algorithmManager.GetVideoSubscribers(videoViews, playerDataManager.GetQuality (), newVideo.isViral);
        newVideo.videoMaxSoftCurrency = algorithmManager.GetVideoSoftCurrency(videoViews);
        float qualityNumber = (float)newVideo.selectedQuality / (float) Enum.GetValues (typeof(VideoQuality)).Length * 2;
        newVideo.lifeTimeHours = (float)(algorithmManager.GetVideoLifetime (videoViews, qualityNumber, 0.9f))/3600f; //Fromseconds to hours
        newVideo.lastUpdateTime = GameClock.Instance.Now;

        ulong currentTotalSubs = playerDataManager.GetSubscribers();
        percentage = 14;
        if (currentTotalSubs < 1000)
            percentage = 20;
        else if (currentTotalSubs < 100000)
            percentage = 18;
        else if (currentTotalSubs < 1000000)
            percentage = 16;
        else if (currentTotalSubs < 10000000)
            percentage = 15;
        else
            percentage = 14;

        newVideo.bonusViews = videoViews / 100 * (ulong)percentage;
        newVideo.bonusSubscribers = videoViews / 50;
        newVideo.bonusLikes = videoViews / 20; 
        newVideo.bonusComments = videoViews / 100;
        newVideo.bonusLifeTimeHours = 24 * 10; //10 days
        newVideo.daysSinceLastUpdate = 0;

        playerDataManager.AddVideo (newVideo);
        DeleteUnpublishedVideo (newVideo.name);

        _signalBus.Fire<EndPublishVideoSignal> ();
    }
    public void DeleteUnpublishedVideo (string name)
    {
        playerDataManager.DeleteUnpublishVideo (name);
    }
    public string GetVideoNameByTheme (ThemeType[] _themeTypes)
    {
        string videoName ="";
        int themeUsed = 0;
        foreach(ThemeType themeType in _themeTypes)
        {
            string selectedWord = "";
            foreach(NameInfoByTheme themeInfo in videoNamingInfo)
            {
                if(themeInfo.themeType == themeType)
                {
                    int rndIndex = 0;
                    switch (themeUsed)
                    {
                        case 0:
                            rndIndex = Random.Range(0, themeInfo.words.adjetive.Length);
                            selectedWord = themeInfo.words.adjetive[rndIndex];
                            break;
                        case 1:
                            rndIndex = Random.Range(0, themeInfo.words.adjetive.Length);
                            selectedWord = themeInfo.words.noun2[rndIndex];
                            break;
                        case 2:
                            rndIndex = Random.Range(0, themeInfo.words.adjetive.Length);
                            selectedWord = themeInfo.words.noun1[rndIndex];
                            break;
                    }
                }
            }
            themeUsed++;
            videoName += $"{selectedWord} ";
        }
        int videoNumber = GetNumberOfVideoByName(videoName);
        if(videoNumber>0)
            videoName += $"{videoNumber}";
        return videoName;
    }
    public int GetNumberOfVideoByName(string videoName)
    {
        if (playerDataManager == null)
        {
            print("Player DataManger is Null");
        }
        return playerDataManager.GetNumberOfVideoByName(videoName);
    }

    //int GetNumberOfVideoByThemes (ThemeType[] _themeTypes)
    //{
    //    if (playerDataManager==null)
    //    {
    //        print("Player DataManger is Null");
    //    }
    //    return playerDataManager.GetNumberOfVideoByThemes (_themeTypes);
    //}
    public Video GetVideoByName (string _name)
    {
        return playerDataManager.GetVideoByName (_name);
    }
    public UnpublishedVideo GetUnpublishedVideoByName(string _name)
    {
        return playerDataManager.GetUnpublishedVideoByName(_name);
    }
    public ulong GetVideoMoneyByName (string _name)
    {
        return GetVideoByName (_name).videoSoftCurrency;
    }
    void RecollectVideoMoney (GetMoneyFromVideoSignal signal)
    {
       var money= playerDataManager.RecollectVideoMoney (signal.videoName);
       gameAnalyticsManager.SendCustomEvent("claim_button",new []{money.ToString()});
        _signalBus.Fire<UpdateSoftCurrencySignal> ();
    }
    int GetTimeHour () //Dummy Not being used
    {
        return GameClock.Instance.Now.Hour;
    }
    public bool IsPlayerResting ()
    {
        return energyManager.GetPlayerIsResting ();
    }
    public void UpdateUnpublishedVideos()
    {
        playerDataManager.UpdateUnpublishedVideos();
    }
    public bool ConsumeHardCurrency( int amount)
    {
        bool canConsume = playerDataManager.CanConsumeHardCurrency((ulong)amount);
        if (canConsume)
            playerDataManager.ConsumeHardCurrency((ulong)amount,()=>{ });
        return canConsume;
    }
}
