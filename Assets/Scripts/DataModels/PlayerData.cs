using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName = "No Data";
    public List<Video> videos = new List<Video>();
    public ulong subscribers;
    public float quality = 0.1f;
    public ulong softCurrency;
    public ulong hardCurrency;
    //Leveling System 
    public ulong experiencePoints;
    public int subscribersThresholdCounter;
    public int viewsThresholdCounter;
    public int softCurrencyThresholdCounter;
}