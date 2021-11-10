using System;
using System.Collections.Generic;
using System.Linq;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class PlayerInventory : MonoBehaviour
{
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
    void Start()
    {
     //  signalBus.Subscribe<OnPlayerInventoryFetchedSignal>(OnPlayerInventoryFetched);
     LoadAddressedData();
    }

     async void LoadAddressedData()
    {
        var assets= Addressables.LoadAssetsAsync<ThemeCustomizationItem>("default", null);
        await assets.Task;
        if (assets.Status == AsyncOperationStatus.Succeeded)
        {
            var items= (List<ThemeCustomizationItem>) assets.Result;

            foreach (var characterItemName in playerInventoryAddressedData.characterItemsNames)
            {
                characterItems.Add(items.Find((item => item.name == characterItemName)));
            }

            foreach (var equippedCharacterItemName in playerInventoryAddressedData.equippedCharacterItemsNames)
            {
                equippedCharacterItems.Add(characterItems.Find((item => item.name==equippedCharacterItemName)));
            }

        }
        else
        {
            print("Failed to load Assets ");
        }

    }
    void OnPlayerInventoryFetched(OnPlayerInventoryFetchedSignal playerInventoryFetchedSignal)
    {
        playerInventoryAddressedData = playerInventoryFetchedSignal.PlayerInventoryAddressedData;

        LoadAddressedData();
        
       
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

    public void UpdateRoomData(RoomLayout roomLayout,List<VideoQualityCustomizationItem> vCItems)
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
        return playerInventoryAddressedData.RoomLayout;
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