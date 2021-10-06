using System.Collections.Generic;

[System.Serializable]
public class Video
{
    public string name;
    public List<Theme> themes=new List<Theme>(3);
    public float quality;
    public ulong views;
    public ulong likes;
    public ulong comments;

}