using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AlgorithmManager : MonoBehaviour
{ 
    
    public int GetVideoViews (int _base, int _subscribers, float[] _themes, float _videoQuality)
    {
        float themesPopularity = 0;
        foreach(float theme in _themes)
        {
            themesPopularity += theme;
        }

        int viewers =(int)((_base + _subscribers) + ((_base + _subscribers) * themesPopularity * _videoQuality));
        return viewers;
    }

    public int GetVideoLikes (int _views, int _videoQuality)
    {
        return (int)(_views * (_videoQuality*0.05f));
    }
    public int GetVideoComments (int _views)
    {
        return (int)(_views * 0.04f);
    }
    public int GetVideoSubscribers (int _views, int _videoQuality)
    {
        return (int)(_views *((_videoQuality*0.2f)*0.01f));
    }
}