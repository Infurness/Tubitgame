using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class Video
{
    public string name;
    public ThemeType[] themes = new ThemeType[3];
    public float quality;
    public ulong views;
    public ulong likes;
    public ulong comments;
    public ulong newSubscribers;
    public int money;
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