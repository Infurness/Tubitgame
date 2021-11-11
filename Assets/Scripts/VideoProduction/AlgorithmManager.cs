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
    private bool shouldUpdate=true;
    [SerializeField] private float updateTime = 15;
    [SerializeField] public int baseNum=800;
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
        ulong viewers = (ulong)(((ulong)baseNum + _subscribers) + (((ulong)baseNum + _subscribers) * themesPopularity * _videoQuality));
        if(isViral)
            viewers *= (ulong)GetVirality ();
        return viewers;
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
        return maxViews/100 ;
    }
    int GetVirality ()
    {
        return Random.Range (25, 101);
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
                print("Video Completeness" + video.IsMiningCompleted);
                if (!video.IsMiningCompleted)
                {
                   
                    double  dt =(double) (video.lastUpdateTime.Subtract(video.CreateDateTime)).TotalMinutes;
                    print("dt mins = "+dt);
                    
                    double completePercentage =Mathf.Min(((float)dt / (video.lifeTimeHours*60.0f)), 1.0f);
                    print("Complete percentage "+ completePercentage);

                    ulong previousViews = video.views;
                    video.views=(ulong)(video.maxViews*completePercentage);
                    PlayerDataManager.Instance.AddViews (video.views - previousViews);//Add the views gained this step for experience points calculation

                    video.likes = (ulong)(video.maxLikes*completePercentage);
                    video.comments =(ulong) (video.maxComments*completePercentage);
                    video.videoSoftCurrency =((ulong)(video.videoMaxSoftCurrency*completePercentage))-video.collectedCurrencies;

                    ulong previousSubs = video.newSubscribers;
                    video.newSubscribers = (ulong) (video.maxNewSubscribers*completePercentage);
                    PlayerDataManager.Instance.AddSubs (video.newSubscribers - previousSubs);//Add the subs gained this step for experience points calculation

                    subscribers += video.newSubscribers;
                    video.lastUpdateTime = GameClock.Instance.Now;
                    if (completePercentage==1.0)
                    {
                        video.IsMiningCompleted = true;
                    }
                }
                else
                {
                    subscribers += video.newSubscribers;
                }
            }
            signalBus.TryFire<OnVideosStatsUpdatedSignal>();
            PlayerDataManager.Instance.UpdatePlayerData(subscribers,videos);
            StartCoroutine(UpdateTimer());

        }
    }
}