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
    [Header("Viral Rate")]
    [SerializeField] float baseRate = 5f;
    [SerializeField] float factor = 0.00002f;
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

    public bool IsRecording ()
    {
        return isRecording;
    }
    public void SetIsRecording (bool recording)
    {
        isRecording = recording;
    }
   
    private void CreateVideo (PublishVideoSignal signal)
    {
        string videoName = signal.videoName;
        ThemeType[] videoThemes = signal.videoThemes;
        Video newVideo = new Video(GameClock.Instance.Now);
        newVideo.name = videoName;
        newVideo.themes = (ThemeType[])videoThemes.Clone();
        newVideo.selectedQuality = signal.videoSelectedQuality;
        newVideo.quality = playerDataManager.GetQuality();

        float themesPopularity = GetThemesPopularity(videoThemes);
        var percentage = GetViralPercentage(themesPopularity, newVideo.selectedQuality);

        if (Random.Range(0, 101) >= 100 - percentage)
        {
            newVideo.isViral = true;
            gameAnalyticsManager.SendCustomEvent("viral_event");
        }

        var subscribers = playerDataManager.GetSubscribers();
        List<float> themeValues = new List<float>();

        ulong videoViews = algorithmManager.GetVideoViews
                                (
                                playerDataManager.GetSubscribers(),
                                playerDataManager.GetQuality(),
                                newVideo.isViral,
                                themesPopularity
                                );

        newVideo.maxViews = videoViews;
        newVideo.maxLikes = algorithmManager.GetVideoLikes(videoViews, playerDataManager.GetQuality());
        newVideo.maxComments = algorithmManager.GetVideoComments(videoViews);
        newVideo.maxNewSubscribers = algorithmManager.GetVideoSubscribers(videoViews, playerDataManager.GetQuality(), newVideo.isViral);
        newVideo.videoMaxSoftCurrency = algorithmManager.GetVideoSoftCurrency(videoViews);
        float qualityNumber = (float)newVideo.selectedQuality / (float)Enum.GetValues(typeof(VideoQuality)).Length * 2;
        newVideo.lifeTimeHours = (float)(algorithmManager.GetVideoLifetime(videoViews, qualityNumber, 0.9f)) / 3600f; //Fromseconds to hours
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

        playerDataManager.AddVideo(newVideo);
        DeleteUnpublishedVideo(newVideo.name);

        _signalBus.Fire<EndPublishVideoSignal>();
    }

    private float GetThemesPopularity(ThemeType[] videoThemes)
    {
        var themesPopularity = 0f;
        foreach (var theme in videoThemes)
        {
            themesPopularity += themesManager.GetThemePopularity(theme, GameClock.Instance.Now);
        }

        return themesPopularity;
    }

    private float GetViralPercentage(float themesPopularity, VideoQuality quality)
    {
        var baseTimeToCompleteVideo = algorithmManager.GetBaseTimeToBeProduced((int)quality);
        return baseRate + (themesPopularity * baseTimeToCompleteVideo * factor);
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
                            rndIndex = Random.Range(0, themeInfo.words.noun2.Length);
                            selectedWord = themeInfo.words.noun2[rndIndex];
                            break;
                        case 2:
                            rndIndex = Random.Range(0, themeInfo.words.noun1.Length);
                            selectedWord = themeInfo.words.noun1[rndIndex];
                            break;
                    }
                }
            }
            themeUsed++;
            if (themeUsed > 1)
                videoName += $" ";
            videoName += $"{selectedWord}";
        }
        int videoNumber = GetNumberOfVideoByName(videoName);
        if (videoNumber>0)
        {
            videoName += $" {videoNumber}";
        }
            
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
