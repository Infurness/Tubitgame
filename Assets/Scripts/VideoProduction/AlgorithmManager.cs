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
        StartCoroutine(UpdateTimer());
    }

    public ulong GetVideoViews (ulong _subscribers, ThemeType[] _themes, float _videoQuality)
    {
        float themesPopularity = 0;
        foreach (var theme in _themes)
        {
            themesPopularity += themesManager.GetThemePopularity (theme, GameClock.Instance.Now.Hour);
        }
        ulong viewers = (ulong)(((ulong)baseNum + _subscribers) + (((ulong)baseNum + _subscribers) * themesPopularity * _videoQuality));
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
    public int GetVideoMoney ()
    {
        return 5;
    }
    int GetVirality ()
    {
        if(Random.Range(0,101) >=95)
        {
            return Random.Range (25, 101);
        }
        else
        {
            return 1;
        }
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
            var subscribers = PlayerDataManager.Instance.GetSubscribers();
            foreach (var video in videos)
            {
                print("Video Completeness" + video.IsMiningCompleted);
                if (!video.IsMiningCompleted)
                {
                   
                    double  dt =(double) (video.lastUpdateTime.Subtract(video.CreateDateTime)).Minutes;
                    print("dt mins = "+dt);
                    
                    double completePercentage =Mathf.Min(((float)dt / (video.lifeTimeHours*60.0f)), 1.0f);
                    print("Complete percentage "+ completePercentage);
                    video.views=(ulong)(video.maxViews*completePercentage);
                    video.likes = (ulong)(video.maxLikes*completePercentage);
                    video.comments =(ulong) (video.maxComments*completePercentage);
                    video.money =(int) (video.maxMoney*completePercentage);
                    video.newSubscribers = (ulong) (video.maxNewSubscribers*completePercentage);
                    subscribers += video.newSubscribers;
                    video.lastUpdateTime = GameClock.Instance.Now;
                    if (completePercentage==1.0)
                    {
                        video.IsMiningCompleted = true;
                    }
                }
             

            }
            signalBus.TryFire<OnVideosStatsUpdatedSignal>();
            PlayerDataManager.Instance.UpdateSubscribersAndVideos(subscribers,videos);
            StartCoroutine(UpdateTimer());

        }
    }
}