using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public List<Video> videos = new List<Video>();
    public ulong subscribers;
    public float quality = 1f;
    public ulong softCurrency;
    public ulong hardCurrency;
}