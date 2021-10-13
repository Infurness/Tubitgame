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
    public int GetNumberOfVideoByThemes (ThemeType[] _themeTypes)
    {
        int videoCounter = 0;
        foreach (Video video in m_PlayerData.videos)
        {
            bool sameThemes = true;
            if(video.themes.Length == _themeTypes.Length)
            {
                for(int i =0; i<video.themes.Length;i++)
                {
                    if (video.themes[i] != _themeTypes[i])
                    {
                        sameThemes = false;
                        break;
                    }    
                }
                if (sameThemes)
                    videoCounter++;
            }
        }
        return videoCounter;
    }
    public int RecollectVideoMoney (string _name)
    {
        Video video = GetVideoByName (_name);
        int videoMoney = video.money;
        video.money = 0;
        return videoMoney;
    }
    public float GetPlayerTotalVideos ()
    {
        return m_PlayerData.videos.Count;
    }
    public string GetLastVideoName ()
    {
        return m_PlayerData.videos[m_PlayerData.videos.Count-1].name;
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
