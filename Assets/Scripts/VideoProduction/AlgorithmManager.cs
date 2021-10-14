using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AlgorithmManager : MonoBehaviour
{

    public ulong GetVideoViews (int _base, ulong _subscribers, float[] _themes, float _videoQuality)
    {
        float themesPopularity = 0;
        foreach (float theme in _themes)
        {
            themesPopularity += theme;
        }
        ulong viewers = (ulong)(((ulong)_base + _subscribers) + (((ulong)_base + _subscribers) * themesPopularity * _videoQuality));
        return viewers;
    }

    public ulong GetVideoLikes (ulong _views, float _videoQuality)
    {
        return (ulong)(_views * (_videoQuality * 0.05f));
    }
    public ulong GetVideoComments (ulong _views)
    {
        return (ulong)(_views * 0.04f);
    }
    public ulong GetVideoSubscribers (ulong _views, float _videoQuality)
    {
        return (ulong)(_views * ((_videoQuality * 0.2f) * 0.01f));
    }
    public int GetVideoMoney ()
    {
        return 5;
    }
}