using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class AlgorithmManager : MonoBehaviour
{
    [Inject] private ThemesManager themesManager;
    private bool shouldUpdate;
    [SerializeField] private float updateTime = 15;
    [SerializeField] public int baseNum=800;
    private void Start()
    {
        StartCoroutine(UpdateTimer());
    }

    public ulong GetVideoViews (int _base, ulong _subscribers, ThemeType[] _themes, float _videoQuality)
    {
        float themesPopularity = 0;
        foreach (var theme in _themes)
        {
            themesPopularity += themesManager.GetThemePopularity (theme, GameClock.Instance.Now.Hour);
        }
        ulong viewers = (ulong)(((ulong)_base + _subscribers) + (((ulong)_base + _subscribers) * themesPopularity * _videoQuality));
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
              video.views+=GetVideoViews(baseNum,subscribers,video.themes, video.quality);
              video.likes += GetVideoLikes(video.views, video.quality);
              video.comments += GetVideoComments(video.views);
              video.money += GetVideoMoney();
              
              video.newSubscribers += GetVideoSubscribers(video.views, video.quality);
              subscribers += video.newSubscribers;
              
            }
            PlayerDataManager.Instance.UpdateSubscribersAndVideos(subscribers,videos);
            StartCoroutine(UpdateTimer());

        }
    }
}