using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class Video
{
    public string name;
    public ThemeType[] themes = new ThemeType[3];
    public float quality;
    public ulong views,maxViews;
    public bool isViral;
    public ulong likes,maxLikes;
    public ulong comments,maxComments;
    public ulong newSubscribers,maxNewSubscribers;
    public int money,maxMoney;
    public int lifeTimeHours;
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
