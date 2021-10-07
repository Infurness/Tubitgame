using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/*
 * This MonoBehaviour should  handle player data during online session and offline one
 * it should handle reading and updating data to DB
 */
public class PlayerDataManager : MonoBehaviour
{
    [Inject] private PlayerData m_PlayerData;
    
    public void AddVideo (Video _video)
    {
        m_PlayerData.videos.Add (_video);
    }

    public Video GetVideoByName (string _name)
    {
        foreach(Video video in m_PlayerData.videos)
        {
            if(video.name == _name)
            {
                return video;
            }
        }
        Debug.LogError ($"Video named -{_name}- does not exist");
        return null;
    }

    public float GetQuality ()
    {
        return m_PlayerData.quality;
    }
    public ulong GetSubscribers ()
    {
        return m_PlayerData.subscribers;
    }
}
