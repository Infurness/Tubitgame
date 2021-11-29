﻿using System;
using System.Collections.Generic;
using UnityEngine.Serialization;


public enum VideoQuality {Lowest = 1, UltraLow, MegaLow, VeryLow, Low, Medium, High, VeryHigh, Mega, Ultra, Extreme}
[System.Serializable]
public class Video
{
    public string name;
    public ThemeType[] themes = new ThemeType[3];
    public VideoQuality selectedQuality;
    public float quality;
    public ulong views,maxViews;
    public bool isViral;
    public ulong likes,maxLikes;
    public ulong comments,maxComments;
    public ulong newSubscribers,maxNewSubscribers;
    public ulong videoSoftCurrency;
    public ulong videoMaxSoftCurrency;
    public ulong collectedCurrencies;
    public float lifeTimeHours;
    public bool IsMiningCompleted=false;
    public DateTime lastUpdateTime;
    
    public DateTime CreateDateTime
    {
        get;
       private set;
    }

    public Video(DateTime createDateTime)
    {
        CreateDateTime = createDateTime;
    }
}

public class UnpublishedVideo
{
    public string videoName;
    public ThemeType[] videoThemes;
    public VideoQuality videoQuality;
    public int secondsToBeProduced;
    public DateTime createdTime;

    public UnpublishedVideo (string name, ThemeType[] themes, VideoQuality quality, int time, DateTime date)
    {
        videoName = name;
        videoThemes = themes;
        videoQuality = quality;
        secondsToBeProduced = time;
        createdTime = date;
    }
}
