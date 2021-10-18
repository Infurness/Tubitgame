using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;
using Newtonsoft.Json;
/*
 * This MonoBehaviour should  handle player data during online session and offline one
 * it should handle reading and updating data to DB
 */
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    private PlayerData playerData;
    [Inject] private SignalBus signalBus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>((signal =>
        {
            if (!signal.NewPlayer)
            {
                GetUserData();
            }

        }));
    }

    private void GetUserData()
    {
        playerData = new PlayerData();
        GetUserDataRequest dataRequest = new GetUserDataRequest();
        dataRequest.Keys = new List<string>() {};
        PlayFabClientAPI.GetUserData(dataRequest, (result =>
        {
            UserDataRecord datarecord;
            
            foreach (var pair in result.Data.ToList())
            {
                print("Data pair " +pair.Key+" "+ pair.Value.Value);
            }
            if (result.Data.TryGetValue("Videos", out datarecord))
            {
                
                var videosJson = datarecord.Value;
                playerData.videos = JsonConvert.DeserializeObject<List<Video>>(videosJson);

            }

            if (result.Data.TryGetValue("PlayerName",out datarecord))
            {
                playerData.playerName = JsonConvert.DeserializeObject<string>(datarecord.Value);
            }

          
        }), (error => { print("Cant Retrieve User data"); }));
    }

   

    private void UpdateUserDatabase(string key,object data)
    {
        var dataJson = JsonConvert.SerializeObject(data,Formatting.Indented);
        var dataRequest = new UpdateUserDataRequest();
        dataRequest.Data=new Dictionary<string, string>(){{key,dataJson}};
        PlayFabClientAPI.UpdateUserData(dataRequest, (result => { print(key+ "Updated"); }),
            (error => { print("Cant update "+key ); }));

    }
    public string GetPlayerName ()
    {
        return playerData.playerName;
    }

    public void SetPLayerName(string playerName)
    {
        playerData.playerName = playerName;
        UpdateUserDatabase("PlayerName",playerName);
    }

    public void AddVideo (Video _video)
    {
        playerData.videos.Add (_video);
        UpdateUserDatabase("Videos",playerData.videos);
    }

    public Video GetVideoByName (string _name)
    {
        foreach(Video video in playerData.videos)
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
        foreach (Video video in playerData.videos)
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
        return playerData.videos.Count;
    }
    public string GetLastVideoName ()
    {
        return playerData.videos[playerData.videos.Count-1].name;
    }
    public float GetQuality ()
    {
        return playerData.quality;
    }

    public void UpdatePlayerQuality(float newQuality)
    {
        playerData.quality = newQuality;
        UpdateUserDatabase("PlayerQuality",playerData.quality);
    }
    public ulong GetSubscribers ()
    {
        return playerData.subscribers;
    }

    public void UpdateSubscribers(ulong newCount)
    {
        playerData.subscribers += newCount;
        UpdateUserDatabase("Subscribers",playerData.subscribers);
        
    }
}
