using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string ID;
    public List<Video> videos = new List<Video>();
    public ulong subscribers;
    public float quality;
}