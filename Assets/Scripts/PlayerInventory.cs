using System;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class PlayerInventory : MonoBehaviour
{
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private SignalBus signalBus;
    [SerializeField] PlayerInventoryAddressedData playerInventoryAddressedData;
    public List<ThemeCustomizationItem> equippedCharacterItems;
    public List<ThemeCustomizationItem> defaultCharacterItems;
    public List<ThemeCustomizationItem> characterItems;
    public List<ThemeCustomizationItem> roomThemeEffectItems;
    public List<ThemeCustomizationItem> equippedThemeEffectRoomItems;
    public List<VideoQualityCustomizationItem> videoQualityRoomItems;
    public List<VideoQualityCustomizationItem> equippedVideoQualityRoomItems;
    public List<RealEstateCustomizationItem> realEstateItems;

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
        var totalThemeEquippedItems = new List<ThemeCustomizationItem>(equippedCharacterItems);
        totalThemeEquippedItems.AddRange(equippedThemeEffectRoomItems);
        signalBus.Fire(new OnPlayerEquippedThemeItemChangedSignal()
        {
            CustomizationItems =totalThemeEquippedItems
        });
//        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);


    }
   
    public void EquipThemeEffectRoomItem(ThemeCustomizationItem themeCustomizationItem)
    {
        signalBus.Fire(new OnPlayerRoomThemeItemEquippedSignal()
        {
            ThemeCustomizationItem = themeCustomizationItem
        });
        signalBus.Fire(new OnPlayerEquippedThemeItemChangedSignal()
        {
            CustomizationItems = new List<ThemeCustomizationItem>(roomThemeEffectItems)
        });
    }

    public void TestThemeEffectRoomITem(ThemeCustomizationItem themeCustomizationItem)
    {
        signalBus.Fire(new TestRoomThemeItemSignal(){ThemeCustomizationItem = themeCustomizationItem});
    }

    public void TestVideoQualityRoomItem(VideoQualityCustomizationItem videoQualityCustomizationItem)
    {
        signalBus.Fire(new TestRoomVideoQualityITemSignal(){VideoQualityCustomizationItem = videoQualityCustomizationItem});
    }

    public void EquipVideoQualityRoomItem(VideoQualityCustomizationItem videoQualityCustomizationItem)
    {
        
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
        public List<string> equippedRoomItemsNames;
        public List<string> equippedVideoQualityItemsNames;
      public  PlayerInventoryAddressedData()
        {
            characterItemsNames = new List<string>();
            roomItemsNames = new List<string>();
            videoQualityItemsNames = new List<string>();
            equippedCharacterItemsNames = new List<string>();
            equippedRoomItemsNames = new List<string>();
            equippedVideoQualityItemsNames = new List<string>();

        }
        
    }