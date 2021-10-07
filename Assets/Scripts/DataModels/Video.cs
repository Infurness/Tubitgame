using System.Collections.Generic;

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
}