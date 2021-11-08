using System;
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
            Destroy(this);
        }
        playerData = new PlayerData ();
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
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += ((arg0, mode) =>
        {
            signalBus.Subscribe<ProcessPurchaseSignal>(ProcessSuccessesPurchases);

        });
    }



    void ProcessSuccessesPurchases(ProcessPurchaseSignal purchaseSignal)
    {
        print("Process Currencies");
        var confirmAction = new Action(() =>
        {
            signalBus.Fire<ConfirmPendingPurchaseSignal>(new ConfirmPendingPurchaseSignal()
            {
                product = purchaseSignal.product
            });
        });
        switch (purchaseSignal.product.definition.id)
        {
            case "10HC": AddHardCurrency(10,confirmAction);break;
            case "50HC": AddHardCurrency(50,confirmAction); break;

        }
    }

    private void GetUserData()
    {
        
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

            if (result.Data.TryGetValue("Subscribers",out datarecord))
            {
                playerData.subscribers = JsonConvert.DeserializeObject<ulong>(datarecord.Value);
            }
            else
            {
                playerData.subscribers = 0;
            }
            if (result.Data.TryGetValue("SoftCurrency",out datarecord))
            {
                playerData.softCurrency = JsonConvert.DeserializeObject<ulong>(datarecord.Value);
            }
            else
            {
                playerData.softCurrency= 0;
            }
            

          
        }), (error => { print("Cant Retrieve User data"); }));
    }

   

    private void UpdateUserDatabase(string[] keys,object[] data,Action onsuccess=null,Action onFailed=null)
    {
      
        var dataRequest = new UpdateUserDataRequest();
        dataRequest.Data=new Dictionary<string, string>();
        for (int i = 0; i < keys.Length; i++)
        {
            var dataJson = JsonConvert.SerializeObject(data[i],Formatting.Indented);
            dataRequest.Data.Add(keys[i],dataJson);
        }
       
        PlayFabClientAPI.UpdateUserData(dataRequest, (result =>
            {
                print(keys[0]+ "Updated") ;
                onsuccess?.Invoke();

            }),
            (error =>
            {
                print("Cant update "+keys[0] ); 
                onFailed?.Invoke();
            }));

    }
    
    public string GetPlayerName ()
    {
        if (playerData != null)
            return playerData.playerName;
        else
            Debug.LogError ("NO PLAYER DATA SET");
        return "Error: NoData";    
    }

    public void SetPLayerName(string playerName)
    {
        UpdateUserDatabase(new[] {"PlayerName"},new[] {playerName},(() =>
        {
            playerData.playerName = playerName;
 
        }));
    }

    public void AddVideo (Video _video)
    {
        var videos = playerData.videos;
        videos.Add(_video);
        UpdateUserDatabase(new[] {"Videos"},new[] {videos},(() =>
        {
            playerData.videos = videos;
        }));
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
    public ulong RecollectVideoMoney (string _name)
    {
        Video video = GetVideoByName (_name);
        var videoMoney = video.videoSoftCurrency;
        video.collectedCurrencies += video.videoSoftCurrency;
        playerData.softCurrency += videoMoney;
        UpdateUserDatabase(new[] {"SoftCurrency","Videos"},new object[]{ playerData.softCurrency,playerData.videos});
        video.videoSoftCurrency = 0;
        return videoMoney;
    }
    public int GetPlayerTotalVideos ()
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
        UpdateUserDatabase(new[] {"PlayerQuality"},new object[] {newQuality},(() =>
        {
            playerData.quality = newQuality;

        }));
    }
    public ulong GetSubscribers ()
    {
        if (playerData != null)
            return playerData.subscribers;
        else
            Debug.LogError ("NO PLAYER DATA SET");
        return 0;
       
    }

    public List<Video> GetVideos()
    {
        return playerData.videos;
    }

    public ulong GetSoftCurrency()
    {
        return playerData.softCurrency;

    }
    public void UpdatePlayerData(ulong subscribersCount,List<Video> videos)
    { 
        UpdateUserDatabase(new[] {"Subscribers","Videos"},new object[]
        {
            subscribersCount,
            videos
        },(() =>
        {
            playerData.subscribers = subscribersCount;
            playerData.videos = videos; 
        }));
        signalBus.Fire<ChangePlayerSubsSignal> (new ChangePlayerSubsSignal { subs=subscribersCount});
    }

     void AddHardCurrency(int amount,Action confirmPurchase=null)
    {
        var hc = playerData.hardCurrency;
        hc += (ulong) amount;
        
        
        UpdateUserDatabase(new[] {"HardCurrency"},new object []  {hc},(() =>
        {
            playerData.hardCurrency = hc;
            print("Added HC");
            confirmPurchase?.Invoke();
        }));
    }

    public void ConsumeHardCurrency(ulong amount,Action onRedeemed)
    {
        if (amount>playerData.hardCurrency)
        {
            return ;
        }
        var hc = playerData.hardCurrency;
        
        hc -= amount;
     
        UpdateUserDatabase(new[] {"HardCurrency"},new object[hc],(() =>
        {
            playerData.hardCurrency = hc;
            onRedeemed?.Invoke();
        }));

    }
    public void ConsumeSoftCurrency(ulong amount,Action onRedeemed)
    {
        if (amount>playerData.softCurrency)
        {
            return ;
        }
        var sc = playerData.hardCurrency;
        
        sc -= amount;
     
        UpdateUserDatabase(new[] {"SoftCurrency"},new object[sc],(() =>
        {
            playerData.softCurrency = sc;
            onRedeemed?.Invoke();
        }));
    }

}
