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
        signalBus.Subscribe<RemoteAssetsCheckSignal>(GetPlayerInventory);

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
            case "10HC":
                AddHardCurrency(10, confirmAction);
                break;
            case "50HC":
                AddHardCurrency(50, confirmAction);
                break;

        }
    }

   private void GetPlayerInventory()
    {
          GetUserDataRequest dataRequest = new GetUserDataRequest();
        dataRequest.Keys = new List<string>() { "Inventory","Avatar"};
        PlayFabClientAPI.GetUserData(dataRequest, (result =>
        {
            PlayerInventoryAddressedData inventorydata;
            CharacterAvatarAddressedData avatarData;
            UserDataRecord datarecord;
            if (result.Data.TryGetValue("Inventory",out datarecord))
            {
                
                 inventorydata = JsonConvert.DeserializeObject<PlayerInventoryAddressedData>(datarecord.Value);
                 if (result.Data.TryGetValue("Avatar",out datarecord))
                 {
                     avatarData=JsonConvert.DeserializeObject<CharacterAvatarAddressedData>(datarecord.Value);
                     signalBus.Fire(new OnPlayerInventoryFetchedSignal()
                     {
                         PlayerInventoryAddressedData = inventorydata,
                         CharacterAvatarAddressedData = avatarData
                     });
                 }
                 else
                 {
                     signalBus.Fire(new OnPlayerInventoryFetchedSignal()
                     {
                         PlayerInventoryAddressedData = inventorydata,
                         CharacterAvatarAddressedData = new CharacterAvatarAddressedData()
                     });
                 }
                 
               
            }
            else
            {
                signalBus.Fire<OnPlayerInventoryFetchedSignal>(new OnPlayerInventoryFetchedSignal()
                {
                    PlayerInventoryAddressedData = new PlayerInventoryAddressedData()
                });
            }



        }), (error => { print("Cant Retrieve Inventorey data"); }));
    }

    private void GetUserData()
    {
        
        GetUserDataRequest dataRequest = new GetUserDataRequest();
        dataRequest.Keys = new List<string>() { };
        PlayFabClientAPI.GetUserData(dataRequest, (result =>
        {
            UserDataRecord datarecord;

            foreach (var pair in result.Data.ToList())
            {
                print("Data pair " + pair.Key + " " + pair.Value.Value);
            }

            if (result.Data.TryGetValue("Videos", out datarecord))
            {

                var videosJson = datarecord.Value;
                playerData.videos = JsonConvert.DeserializeObject<List<Video>>(videosJson);

            }
            if (result.Data.TryGetValue ("UnpublishedVideos", out datarecord))
            {
                var unpublishedVideosJson = datarecord.Value;
                playerData.unpublishedVideos = JsonConvert.DeserializeObject<List<UnpublishedVideo>> (unpublishedVideosJson);
            }
            if (result.Data.TryGetValue ("XpData", out datarecord))
            {

                var xpDataJson = datarecord.Value;
                playerData.xpData = JsonConvert.DeserializeObject<ExperienceData> (xpDataJson);

            }
            else
            {
                playerData.xpData = new ExperienceData
                {
                    experiencePoints = 0,
                    softCurrencyThresholdCounter = 0,
                    subscribersThresholdCounter = 0,
                    viewsThresholdCounter = 0
                };
            }

            if (result.Data.TryGetValue("PlayerName", out datarecord))
            {
                playerData.playerName = JsonConvert.DeserializeObject<string>(datarecord.Value);
            }

            if (result.Data.TryGetValue("Subscribers", out datarecord))
            {
                playerData.subscribers = JsonConvert.DeserializeObject<ulong>(datarecord.Value);
            }
            else
            {
                playerData.subscribers = 0;
            }

            if (result.Data.TryGetValue("SoftCurrency", out datarecord))
            {
                playerData.softCurrency = JsonConvert.DeserializeObject<ulong>(datarecord.Value);
            }
            else
            {
                playerData.softCurrency = 0;
            }


            if (result.Data.TryGetValue ("HardCurrency", out datarecord))
            {
                playerData.hardCurrency = JsonConvert.DeserializeObject<ulong> (datarecord.Value);
            }
            else
            {
                playerData.hardCurrency = 0;
            }

        }), (error => { print("Cant Retrieve User data"); }));
    }

    private void UpdateUserDatabase(string[] keys, object[] data, Action onsuccess = null, Action onFailed = null,UserDataPermission permission=UserDataPermission.Private)
    {

        var dataRequest = new UpdateUserDataRequest();
        dataRequest.Data = new Dictionary<string, string>();

        for (int i = 0; i < keys.Length; i++)
        {
            var dataJson = JsonConvert.SerializeObject(data[i],new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
            dataRequest.Data.Add(keys[i], dataJson);
        }

        dataRequest.Permission = permission;
        PlayFabClientAPI.UpdateUserData(dataRequest, (result =>
            {
                print(keys[0] + "Updated");
                onsuccess?.Invoke();

            }),
            (error =>
            {
                print("Cant update " + keys[0]);
                onFailed?.Invoke();
            }));

    }

    public string GetPlayerName()
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

        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName (request, OnDisplayNameSetSuccess, OnDisplayNameSetError);
    }
    void OnDisplayNameSetSuccess (UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log ($"The player's display name is now: {result.DisplayName}");
    }
    void OnDisplayNameSetError (PlayFabError error)
    {
        Debug.LogError (error.GenerateErrorReport ());
    }

    public void AddVideo(Video _video)
    {
        var videos = playerData.videos;
        videos.Add(_video);
        UpdateUserDatabase(new[] {"Videos"}, new[] {videos}, (() => { playerData.videos = videos; }));
    }

    public void SetUnpublishedVideo(UnpublishedVideo unpublishedvideo)
    {
        var unpublishedvideos = playerData.unpublishedVideos;
        unpublishedvideos.Add (unpublishedvideo);
        UpdateUserDatabase (new[] { "UnpublishedVideos" }, new[] { unpublishedvideos }, (() => { playerData.unpublishedVideos = unpublishedvideos; }));
    }
    public void DeleteUnpublishVideo(string name)
    {
        var unpublishedvideos = playerData.unpublishedVideos;
        int index = 0;
        foreach(UnpublishedVideo video in unpublishedvideos)
        {
            if (video.videoName == name)
                break;
            index++;
        }        
        unpublishedvideos.RemoveAt(index);
        UpdateUserDatabase (new[] { "UnpublishedVideos" }, new[] { unpublishedvideos }, (() => { playerData.unpublishedVideos = unpublishedvideos; }));
    }
    public Video GetVideoByName(string _name)
    {
        foreach (Video video in playerData.videos)
        {
            if (video.name == _name)
            {
                return video;
            }
        }

        Debug.LogError($"Video named -{_name}- does not exist");
        return null;
    }

    public int GetNumberOfVideoByThemes(ThemeType[] _themeTypes)
    {
        int videoCounter = 0;
        foreach (Video video in playerData.videos)
        {
            bool sameThemes = true;
            if (video.themes.Length == _themeTypes.Length)
            {
                for (int i = 0; i < video.themes.Length; i++)
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

        foreach (UnpublishedVideo video in playerData.unpublishedVideos)
        {
            bool sameThemes = true;
            if (video.videoThemes.Length == _themeTypes.Length)
            {
                for (int i = 0; i < video.videoThemes.Length; i++)
                {
                    if (video.videoThemes[i] != _themeTypes[i])
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
    public List<UnpublishedVideo> GetUnpublishedVideos ()
    {
        return playerData.unpublishedVideos;
    }
    public ulong RecollectVideoMoney (string videoName)
    {
        Video video = GetVideoByName (videoName);
        var videoMoney = video.videoSoftCurrency;
        video.collectedCurrencies += video.videoSoftCurrency;
        playerData.softCurrency += videoMoney;
        UpdateUserDatabase(new[] {"SoftCurrency","Videos"},new object[]{ playerData.softCurrency,playerData.videos});
        signalBus.Fire<AddSoftCurrencyForExperienceSignal> (new AddSoftCurrencyForExperienceSignal () { softCurrency = video.videoSoftCurrency });//Add the soft currency gained for experience points calculation
        video.videoSoftCurrency = 0;
        Debug.LogError ("collectedMoney");
        return videoMoney;
    }
    public int GetPlayerTotalVideos ()
    {
        return playerData.videos.Count;
    }

    public string GetLastVideoName()
    {
        return playerData.videos[playerData.videos.Count - 1].name;
    }

    public float GetQuality()
    {
        return playerData.quality;
    }

    public void UpdatePlayerQuality(float newQuality)
    {
        UpdateUserDatabase(new[] {"PlayerQuality"}, new object[] {newQuality},
            (() => { playerData.quality = newQuality; }));
    }

    public ulong GetSubscribers()
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
    public void AddSoftCurrency ( ulong softCurrency)
    {
        playerData.softCurrency += softCurrency;
        UpdateUserDatabase (new[] {"SoftCurrency"}, new object[] { playerData.softCurrency });
    }
    public ulong GetHardCurrency ()
    {
        return playerData.hardCurrency;

    }
    public void UpdatePlayerData(ulong subscribersCount,List<Video> videos)
    {
        ulong lastSubs = playerData.subscribers;
        UpdateUserDatabase(new[] {"Subscribers","Videos"},new object[]
            {
            subscribersCount,
            videos
        }, (() =>
        {
            playerData.subscribers = subscribersCount;
            playerData.videos = videos;
        }));
        LeaderboardManager.Instance.UpdateLeaderboard ("SubscribersCount", (int)subscribersCount);
        signalBus.Fire (new ChangePlayerSubsSignal() { previousSubs= lastSubs, subs =subscribersCount});

    }

    public void AddHardCurrency(int amount, Action confirmPurchase = null)
    {
        var hc = playerData.hardCurrency;
        hc += (ulong) amount;


        UpdateUserDatabase(new[] {"HardCurrency"}, new object[] {hc}, (() =>
        {
            playerData.hardCurrency = hc;
            print("Added HC");
            confirmPurchase?.Invoke();
        }));
    }

    public void ConsumeHardCurrency(ulong amount, Action onRedeemed)
    {
        if (amount > playerData.hardCurrency)
        {
            return;
        }

        var hc = playerData.hardCurrency;

        hc -= amount;

        UpdateUserDatabase(new[] {"HardCurrency"}, new object[hc], (() =>
        {
            playerData.hardCurrency = hc;
            onRedeemed?.Invoke();
        }));

    }

    public void ConsumeSoftCurrency(ulong amount, Action onRedeemed)
    {
        if (amount > playerData.softCurrency)
        {
            return;
        }

        var sc = playerData.hardCurrency;

        sc -= amount;

        UpdateUserDatabase(new[] {"SoftCurrency"}, new object[sc], (() =>
        {
            playerData.softCurrency = sc;
            onRedeemed?.Invoke();
        }));
    }
    public void AddExperiencePoints (ulong experience)
    {
        playerData.xpData.experiencePoints += experience;
        UpdateXpData ();
    }
    public void CheatResetXp ()
    {
        playerData.xpData.experiencePoints = 0;
        UpdateXpData ();
    }
    public ulong GetExperiencePoints ()
    {
        return playerData.xpData.experiencePoints;
    }
    public int GetSubsThreshold ()
    {
        return playerData.xpData.subscribersThresholdCounter;
    }
    public void SetSubsThreshold (int value)
    {
        playerData.xpData.subscribersThresholdCounter = value;
        UpdateXpData ();
    }
    public int GetViewsThreshold ()
    {
        return playerData.xpData.viewsThresholdCounter;
    }
    public void SetViewsThreshold (int value)
    {
        playerData.xpData.viewsThresholdCounter = value;
        UpdateXpData ();
    }
    public int GetSoftCurrencyThreshold ()
    {
        return playerData.xpData.softCurrencyThresholdCounter;
    }
    public void SetSoftCurrencyThreshold (int value)
    {
        playerData.xpData.softCurrencyThresholdCounter = value;
        UpdateXpData ();
    }
    void UpdateXpData ()
    {
        UpdateUserDatabase (new[] { "XpData" }, new object[]
        {
            playerData.xpData
        });
    }
    public void UpdateEnergyData ( EnergyData energyData)
    {
       UpdateUserDatabase (new[] { "Energy" }, new object[]
       {
            energyData
       });
    }
    public void UpdateEnergyInventoryData (EnergyInventoryData energyInventoryData)
    {
       UpdateUserDatabase (new[] { "EnergyItems" }, new object[]
       {
            energyInventoryData
       });
    }
    public void UpdatePlayerInventoryData(PlayerInventoryAddressedData playerInventoryAddressedData)
    {
        UpdateUserDatabase(new string[] {"Inventory"}, new object[] {playerInventoryAddressedData});
    }

    public void UpdateCharacterAvatar(CharacterAvatarAddressedData addressedAvatar)
    {
        UpdateUserDatabase(new string[] {"Avatar"}, new object[] {addressedAvatar},permission:UserDataPermission.Public);

    }

    public void GetLevelUpRewards (LevelUpSignal signal) //Subscribed to signal in YouTubeVideomanager
    {
        playerData.softCurrency += (ulong)signal.reward.softCurrency;
        UpdateUserDatabase (new[] { "SoftCurrency", "Videos" }, new object[] { playerData.softCurrency, playerData.videos });
        AddHardCurrency (signal.reward.hardCurrency);
    }

}
