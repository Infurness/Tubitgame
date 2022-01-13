using System;
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

    //per day quantities
    public double lastBonusViews;
    public ulong bonusViews;
    public double lastBonusSubscribers;
    public ulong bonusSubscribers;
    public double lastBonusLikes;
    public ulong bonusLikes;
    public double lastBonusComments;
    public ulong bonusComments;
    public float bonusLifeTimeHours;
    public DateTime finishMinningDateTime;
    public int daysSinceLastUpdate;
    public bool isBonusStatsCompleted = false;
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
    public int viewsBonus;
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
