using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class AlgorithmManager : MonoBehaviour
{
    [Inject] private ThemesManager themesManager;
    [Inject] private SignalBus signalBus;
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private ExperienceManager experienceManager;

    private bool shouldUpdate=true;
    [SerializeField] private float updateTime = 15;
    [SerializeField] public int baseNum=800;
    private float themeBonus = 0;
    private int viewsBonus = 1;
    [SerializeField] int[] baseTimeToBeProduced;
    private void Start()
    {
        shouldUpdate = false;
        StartCoroutine (UpdateTimer());
    }

    public ulong GetVideoViews (ulong _subscribers, ThemeType[] _themes, float _videoQuality, bool isViral)
    {
        float themesPopularity = 0;
        foreach (var theme in _themes)
        {
            themesPopularity += themesManager.GetThemePopularity (theme, GameClock.Instance.Now);
        }
        ulong viewers = (ulong)(((ulong)baseNum + _subscribers) + (((ulong)baseNum + _subscribers) * (themesPopularity + UseThemeBonus()) * _videoQuality));
        if(isViral)
            viewers *= (ulong)GetVirality ();
        return viewers * (ulong)UseViewsBonus ();
    }

    public ulong GetVideoLikes (ulong _views, float _videoQuality)
    {
        return (ulong)(_views * (_videoQuality * 0.05f));
    }
    public ulong GetVideoComments (ulong _views)
    {
        return (ulong)(_views * 0.04f);
    }
    public ulong GetVideoSubscribers (ulong _views, float _videoQuality)
    {
        return (ulong)(_views * ((_videoQuality * 0.2f) * 0.01f));
    }
    public ulong GetVideoSoftCurrency (ulong maxViews)
    {
        int level = experienceManager.GetPlayerLevel();
        int moneyMultiplier = 3;
        switch (level)
        {
            case 1:
                moneyMultiplier = 3;
                break;
            case 2:
                moneyMultiplier = 3;
                break;
            case 3:
                moneyMultiplier = 3;
                break;
            case 4:
                moneyMultiplier = 3;
                break;
            case 5:
                moneyMultiplier = 3;
                break;
            case 6:
                moneyMultiplier = 3;
                break;
            case 7:
                moneyMultiplier = 3;
                break;
            case 8:
                moneyMultiplier = 3;
                break;
            case 9:
                moneyMultiplier = 3;
                break;
            case 10:
                moneyMultiplier = 3;
                break;
        }
        return maxViews/1000 *(ulong)moneyMultiplier;
    }
    int GetVirality ()
    {
        return Random.Range (25, 101);
    }
    public int GetVideoSecondsToBeProduced (float qualityValue, int numberOfThemes)
    {
        int indexInArray = (int)(qualityValue * baseTimeToBeProduced.Length / 2)-1;
        return (int)(baseTimeToBeProduced[indexInArray] *((numberOfThemes * 0.1)+1));
    }
    public int GetVideoLifetime (ulong totalViews, float videoQuality, float balanceFactor)
    {
        int seconds = (int)Math.Pow ((totalViews * videoQuality), balanceFactor);
        Debug.Log ($"Seconds to mine the video: {seconds}");
        return seconds;
    }
    public void SetThemeBonus ( float bonus)
    {
        themeBonus = bonus;
    }
    float UseThemeBonus ()
    {
        float returnValue = themeBonus;
        themeBonus = 0;
        return returnValue;
    }
    public void SetViewsBonus (int bonus)
    {
        viewsBonus = bonus;
    }
    int UseViewsBonus ()
    {
        int returnValue = viewsBonus;
        viewsBonus = 1;
        return returnValue;
    }
    IEnumerator UpdateTimer()
    {
        yield return new WaitForSecondsRealtime(updateTime);
        shouldUpdate = true;
    }
    private void Update()
    {
        if (shouldUpdate)
        {
            shouldUpdate = false;
            var videos = PlayerDataManager.Instance.GetVideos();
            ulong subscribers=0;
            ulong softCurrency=0;
            foreach (var video in videos)
            {
                //print("Video Completeness" + video.IsMiningCompleted);
                if (!video.IsMiningCompleted)
                {
                   
                    double  dt =(double) (video.lastUpdateTime.Subtract(video.CreateDateTime)).TotalMinutes;
                    //print("dt mins = "+dt);
                    
                    double completePercentage =Mathf.Min(((float)dt / (video.lifeTimeHours*60.0f)), 1.0f);
                    //print("Complete percentage "+ completePercentage);

                    ulong previousViews = video.views;
                    video.views=(ulong)(video.maxViews*completePercentage);
                    signalBus.Fire<AddViewsForExperienceSignal> (new AddViewsForExperienceSignal () { views = video.views - previousViews });//Add the views gained this step for experience points calculation

                    video.likes = (ulong)(video.maxLikes*completePercentage);
                    video.comments =(ulong) (video.maxComments*completePercentage);
                    video.videoSoftCurrency =((ulong)(video.videoMaxSoftCurrency*completePercentage))-video.collectedCurrencies;

                    ulong previousSubs = video.newSubscribers;
                    video.newSubscribers = (ulong) (video.maxNewSubscribers*completePercentage);
                    signalBus.Fire<AddSubsForExperienceSignal> (new AddSubsForExperienceSignal () { subs = video.newSubscribers - previousSubs });//Add the subs gained this step for experience points calculation

                    subscribers += video.newSubscribers;
                    video.lastUpdateTime = GameClock.Instance.Now;
                    if (completePercentage==1.0)
                    {
                        video.IsMiningCompleted = true;
                        video.finishMinningDateTime = GameClock.Instance.Now;
                    }
                }
                else if(!video.isBonusStatsCompleted)
                {
                    int daysSinceMiningWasCompleted = (int)video.lastUpdateTime.Subtract(video.finishMinningDateTime).TotalDays;

                    if (video.daysSinceLastUpdate < daysSinceMiningWasCompleted) //Update bonuses
                    {
                        video.daysSinceLastUpdate = daysSinceMiningWasCompleted;

                        ulong currentTotalSubs = playerDataManager.GetSubscribers();
                        int percentage = 14;
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

                        video.bonusViews = video.views / 100 * (ulong)percentage;
                        video.bonusSubscribers = video.views / 50;
                        video.bonusLikes = video.views / 20;
                        video.bonusComments = video.views / 100;
                    }

                    double hoursSinceLastUpdate = (double)GameClock.Instance.Now.Subtract(video.lastUpdateTime).TotalHours;
                    hoursSinceLastUpdate = Mathf.Min((float)hoursSinceLastUpdate, video.bonusLifeTimeHours); // this hours cant be more than 240, since the max days for the bonus are 10, (just in case de game is reopened 1 month later)
                    double bonusMultiplier = hoursSinceLastUpdate / 24;

                    video.lastBonusViews += video.bonusViews * bonusMultiplier;
                    ulong previousViews = video.views;
                    video.views = video.maxViews + (ulong)video.lastBonusViews;
                    signalBus.Fire<AddViewsForExperienceSignal>(new AddViewsForExperienceSignal() { views = video.views - previousViews });//Add the views gained this step for experience points calculation

                    video.lastBonusLikes += video.bonusLikes * bonusMultiplier;
                    video.likes = video.maxLikes + (ulong)video.lastBonusLikes;

                    video.lastBonusComments += video.bonusComments * bonusMultiplier;
                    video.comments = video.maxComments + (ulong)video.lastBonusComments;

                    video.lastBonusSubscribers +=video.bonusSubscribers * bonusMultiplier;
                    ulong previousSubs = video.newSubscribers;
                    video.newSubscribers = video.maxNewSubscribers + (ulong)video.lastBonusSubscribers;
                    signalBus.Fire<AddSubsForExperienceSignal>(new AddSubsForExperienceSignal() { subs = video.newSubscribers - previousSubs });//Add the subs gained this step for experience points calculation

                    subscribers += video.newSubscribers;

                    video.videoSoftCurrency = (video.views/100) - video.collectedCurrencies;

                    video.lastUpdateTime = GameClock.Instance.Now;
                    if (daysSinceMiningWasCompleted >= video.bonusLifeTimeHours/24)
                    {
                        video.isBonusStatsCompleted = true;
                    }
                }
            }
            signalBus.TryFire<OnVideosStatsUpdatedSignal>();
            PlayerDataManager.Instance.UpdatePlayerData(subscribers,videos);
            StartCoroutine(UpdateTimer());

        }
    }
}