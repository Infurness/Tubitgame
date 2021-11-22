using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private SignalBus signalBus;
    [SerializeField] private PlayerInventoryAddressedData playerInventoryAddressedData;
    [SerializeField] private List<ThemeCustomizationItem> equippedCharacterItems;
    [SerializeField] private List<ThemeCustomizationItem> defaultCharacterItems;
    [SerializeField] private List<ThemeCustomizationItem> characterItems;
    [SerializeField] private List<ThemeCustomizationItem> roomThemeEffectItems;
    [SerializeField] private List<ThemeCustomizationItem> equippedThemeEffectRoomItems;
    [SerializeField] private List<VideoQualityCustomizationItem> videoQualityRoomItems;
    [SerializeField] private List<VideoQualityCustomizationItem> equippedVideoQualityRoomItems;
    [SerializeField] private List<RealEstateCustomizationItem> realEstateItems;
    
    public  List<ThemeCustomizationItem> EquippedCharacterItems => equippedCharacterItems.ToList();

    public  List<ThemeCustomizationItem> DefaultCharacterItems => defaultCharacterItems.ToList();

    public List<ThemeCustomizationItem> CharacterItems => characterItems.ToList();

    public List<ThemeCustomizationItem> RoomThemeEffectItems => roomThemeEffectItems.ToList();

    public List<ThemeCustomizationItem> EquippedThemeEffectRoomItems => equippedThemeEffectRoomItems.ToList();

    public List<VideoQualityCustomizationItem> VideoQualityRoomItems => videoQualityRoomItems.ToList();

    public List<VideoQualityCustomizationItem> EquippedVideoQualityRoomItems => equippedVideoQualityRoomItems.ToList();

    public List<RealEstateCustomizationItem> RealEstateItems => realEstateItems.ToList();

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }

    void Start()
    {
      signalBus.Subscribe<OnPlayerInventoryFetchedSignal>(OnPlayerInventoryFetched);
      
      playerDataManager = PlayerDataManager.Instance;

    }

     async Task LoadThemeEffectAddressedAssets()
    {
        
        var themeEffectAssets= Addressables.LoadAssetsAsync<ThemeCustomizationItem>("roomtheme", null);
        await themeEffectAssets.Task;
        if (themeEffectAssets.Status == AsyncOperationStatus.Succeeded)
        {
            var themeEffectItems= (List<ThemeCustomizationItem>) themeEffectAssets.Result;
            roomThemeEffectItems = themeEffectItems;
            // foreach (var characterItemName in playerInventoryAddressedData.characterItemsNames)
            // {
            //     characterItems.Add(themeEffectItems.Find((item => item.name == characterItemName)));      //todo uncomment when shop implemented
            // }
            var roomLayout = playerInventoryAddressedData.RoomLayout;
            foreach (var floorItem in roomLayout.FloorLayoutSlots)
            { 
                equippedThemeEffectRoomItems.Add(themeEffectItems.Find((it) => it.name == floorItem.ItemName));
      
            }
            foreach (var wallItem in roomLayout.WallLayoutSlots)
            { 
                equippedThemeEffectRoomItems.Add(themeEffectItems.Find((it) => it.name == wallItem.ItemName));
            }
            foreach (var objectItem in roomLayout.ObjectsLayoutSlots)
            { 
                equippedThemeEffectRoomItems.Add(themeEffectItems.Find((it) => it.name == objectItem.ItemName));
            }
           
            

        }
        else
        {
            print("Failed to load Assets ");
        }
        
     

    }

     async Task LoadVideoQualityAddressedAssets()
     {
         var videoQualityItems= Addressables.LoadAssetsAsync<VideoQualityCustomizationItem>("default", null);
         await videoQualityItems.Task;
         if (videoQualityItems.Status == AsyncOperationStatus.Succeeded)
         {
             var vcItems= (List<VideoQualityCustomizationItem>) videoQualityItems.Result;

             videoQualityRoomItems = vcItems;

             // foreach (var videoQualityItemName in playerInventoryAddressedData.videoQualityItemsNames)
             // {
             //     videoQualityRoomItems.Add(vcItems.Find((item => item.name==videoQualityItemName)));     //todo uncomment when shop implemented
             // }
             
             

             foreach (var qualityItemsName in playerInventoryAddressedData.equippedVideoQualityItemsNames)
             {
                 equippedVideoQualityRoomItems.Add(videoQualityRoomItems.Find((item => item.name==qualityItemsName)));
             }

         }
         else
         {
             print("Failed to load Assets ");
         }
     }
      async  void  OnPlayerInventoryFetched(OnPlayerInventoryFetchedSignal playerInventoryFetchedSignal)
    {
        playerInventoryAddressedData = playerInventoryFetchedSignal.PlayerInventoryAddressedData;

      await  LoadThemeEffectAddressedAssets();
      await  LoadVideoQualityAddressedAssets();

      signalBus.Fire<AssetsLoadedSignal>();

    }
    public void AddCharacterItem(ThemeCustomizationItem themeCustomizationItem)
    {
        if (playerInventoryAddressedData.characterItemsNames.Contains(themeCustomizationItem.name))
            return;
        playerInventoryAddressedData.characterItemsNames.Add(themeCustomizationItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }
   

    public void EquipCharacterItem(ThemeCustomizationItem themeCustomizationItem)
    {
       var oldItem =characterItems.Find((item => item.GetType() == themeCustomizationItem.GetType()));

       playerInventoryAddressedData.equippedCharacterItemsNames.Remove(oldItem.name);
       characterItems.Remove(oldItem);
        signalBus.Fire(new OnCharacterItemEquippedSignal()
        {
            ThemeCustomizationItem = themeCustomizationItem
        });
        var totalThemeEquippedItems = new List<ThemeCustomizationItem>(EquippedCharacterItems);
        totalThemeEquippedItems.AddRange(EquippedThemeEffectRoomItems);
        signalBus.Fire(new OnPlayerEquippedThemeItemChangedSignal()
        {
            CustomizationItems =totalThemeEquippedItems
        });
//        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);


    }

    public  void UpdateRoomData(RoomLayout roomLayout,List<VideoQualityCustomizationItem> vCItems)
    {
        
        playerInventoryAddressedData.RoomLayout = roomLayout;
        equippedThemeEffectRoomItems.Clear();
        foreach (var floorItem in roomLayout.FloorLayoutSlots)
        { 
           equippedThemeEffectRoomItems.Add(roomThemeEffectItems.Find((it) => it.name == floorItem.ItemName));
      
        }
        foreach (var wallItem in roomLayout.WallLayoutSlots)
        { 
            equippedThemeEffectRoomItems.Add(roomThemeEffectItems.Find((it) => it.name == wallItem.ItemName));
        }
        foreach (var objectItem in roomLayout.ObjectsLayoutSlots)
        { 
            equippedThemeEffectRoomItems.Add(roomThemeEffectItems.Find((it) => it.name == objectItem.ItemName));
        }
        equippedVideoQualityRoomItems.Clear();
        equippedVideoQualityRoomItems = vCItems;
        playerInventoryAddressedData.equippedVideoQualityItemsNames.Clear();
        equippedVideoQualityRoomItems.ForEach((vc) =>
            playerInventoryAddressedData.equippedVideoQualityItemsNames.Add(vc.name));
        var allThemItems = new List<ThemeCustomizationItem>();
        allThemItems.AddRange(EquippedCharacterItems);
        allThemItems.AddRange(EquippedThemeEffectRoomItems);
        signalBus.Fire(new OnPlayerEquippedThemeItemChangedSignal()
        {
            CustomizationItems = allThemItems
        });
        playerDataManager.UpdatePlayerQuality(equippedVideoQualityRoomItems.Sum((item => item.videoQualityBonus)));
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);

    }

    

    public void TestThemeEffectRoomITem(ThemeCustomizationItem themeCustomizationItem)
    {
        signalBus.Fire(new TestRoomThemeItemSignal(){ThemeCustomizationItem = themeCustomizationItem});
    }

    public void TestVideoQualityRoomItem(VideoQualityCustomizationItem videoQualityCustomizationItem)
    {
        signalBus.Fire(new TestRoomVideoQualityITemSignal(){VideoQualityCustomizationItem = videoQualityCustomizationItem});
    }

    public RoomLayout GetRoomLayout()
    {
        return  new RoomLayout( playerInventoryAddressedData.RoomLayout);
    }

    public T GetEquippedItem<T>() where T: ThemeCustomizationItem
    {
        foreach (var characterItem in equippedCharacterItems)
        {
            if (characterItem.GetType() == typeof(T))
            {
                return (T) characterItem;

            }
        }

        return null;
    }
  
}
[System.Serializable]
    public class PlayerInventoryAddressedData
    {
      
        public  List<string> characterItemsNames;
        public List<string> roomItemsNames;
        public List<string> videoQualityItemsNames;
        public List<string> equippedCharacterItemsNames;
        public List<string> equippedVideoQualityItemsNames;
        public RoomLayout RoomLayout;
      public  PlayerInventoryAddressedData()
        {
            characterItemsNames = new List<string>();
            roomItemsNames = new List<string>();
            videoQualityItemsNames = new List<string>();
            equippedCharacterItemsNames = new List<string>();
            equippedVideoQualityItemsNames = new List<string>();
             RoomLayout = new RoomLayout();

        }
        
    }