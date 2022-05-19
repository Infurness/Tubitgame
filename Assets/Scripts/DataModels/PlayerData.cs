﻿using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName = "No Data";
    public List<Video> videos = new List<Video>();
    public List<UnpublishedVideo> unpublishedVideos = new List<UnpublishedVideo>();
    public ulong subscribers;
    public float quality = 0.1f;
    public ulong softCurrency;
    public ulong hardCurrency;
    public bool noAds = false;
    public bool hasDoubleEnergy = false;
    public bool isResting=false;
    public ExperienceData xpData;
    public EnergyData? energyData;
    public string playerId;
}
[System.Serializable]
public struct ExperienceData
{
    public ulong experiencePoints;
    public int subscribersThresholdCounter;
    public int viewsThresholdCounter;
    public int softCurrencyThresholdCounter;
}