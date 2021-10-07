using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string ID;
    public List<Video> videos;
    public ulong subscribers;
    public float quality;
}