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
    public HeadItem currentHead, defaultHead;
    public FaceItem currentFace, defaultFace;
    public TorsoItem currentTorso, defaultTorso;
    public LegsItem currentLegs, defaultLegs;
    public FeetItem currentFeet, defaultFeet;
    public List<HeadItem> HeadItems;
    public List<FaceItem> FaceItems;
    public List<TorsoItem> TorsoItems;
    public List<LegsItem> LegsItems;
    public List<FeetItem> FeetItems;
    public List<ThemeCustomizationItem> RoomItems;
    public List<ThemeCustomizationItem> CurrentRoomItems;
    public List<RealEstateCustomizationItem> RealEstateItems;
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
            
            foreach (var item in items)
            {
                switch (item)
                {
                    case HeadItem headItem:
                        HeadItems.Add(headItem);
                        break;
                    case FaceItem faceItem:
                        FaceItems.Add(faceItem);
                        break;
                    case TorsoItem torsoItem:
                        TorsoItems.Add(torsoItem);
                        break;
                    case LegsItem legsItem:
                        LegsItems.Add(legsItem);
                        break;
                    case FeetItem feetItem:
                        FeetItems.Add(feetItem);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            currentHead = HeadItems.Find((hi) => hi.name == playerInventoryAddressedData.currentHead);
            currentFace = FaceItems.Find(item => item.name == playerInventoryAddressedData.currentFace);
            currentTorso = TorsoItems.Find(item => item.name == playerInventoryAddressedData.currentTorso);
            currentFeet = FeetItems.Find(item => item.name == playerInventoryAddressedData.currentFeet);
            currentLegs = LegsItems.Find(item => item.name == playerInventoryAddressedData.currentLegs);

            // FaceItems=(List<FaceItem>)heads.Result;
        }
        else
        {
            print("Failed to load Faces ");
        }

    }
    void OnPlayerInventoryFetched(OnPlayerInventoryFetchedSignal playerInventoryFetchedSignal)
    {
        LoadAddressedData();
        playerInventoryAddressedData = playerInventoryFetchedSignal.PlayerInventoryAddressedData;
        
        signalBus.Fire(new OnHeadEquippedSignal()
        {
            HeadItem = currentHead
        });
        
        signalBus.Fire(new OnFaceEquippedSignal()
        {
            FaceItem = currentFace
        });
        signalBus.Fire(new OnTorsoEquippedSignal()
        {
            TorsoItem = currentTorso
        });
        signalBus.Fire(new OnLegsEquippedSignal()
        { 
            LegsItem= currentLegs
        });
        signalBus.Fire(new OnFeetEquippedSignal()
        {
            FeetItem= currentFeet
        });
    }
    public void AddHeadItem(HeadItem headItem)
    {
        if (playerInventoryAddressedData.headItemsNames.Contains(headItem.name))
            return;
        playerInventoryAddressedData.headItemsNames.Add(headItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }
    public void AddFaceItem(FaceItem faceItem)
    {
        if (playerInventoryAddressedData.faceItemsNames.Contains(faceItem.name))
            return;
        playerInventoryAddressedData.faceItemsNames.Add(faceItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }
    public void AddTorsoItem(TorsoItem torsoItem)
    {
        if (playerInventoryAddressedData.torsoItemsNames.Contains(torsoItem.name))
            return;
        playerInventoryAddressedData.torsoItemsNames.Add(torsoItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }
    public void AddLegsItem(LegsItem legsItem)
    {
        if (playerInventoryAddressedData.legsItemsNames.Contains(legsItem.name))
            return;
        playerInventoryAddressedData.legsItemsNames.Add(legsItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }
    public void AddFeetItem(FeetItem feetItem)
    {
        if (playerInventoryAddressedData.feetItemsNames.Contains(feetItem.name))
            return;
        playerInventoryAddressedData.feetItemsNames.Add(feetItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }

    public void EquipHead(HeadItem headItem)
    {
        playerInventoryAddressedData.currentHead = headItem.name;
        signalBus.Fire<OnHeadEquippedSignal>(new OnHeadEquippedSignal()
        {
            HeadItem = headItem
        });
        signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
        {
            CustomizationItems = new List<ThemeCustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
        });
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);


    }
    public void EquipFace(FaceItem faceItem)
    {
        playerInventoryAddressedData.currentFace = faceItem.name;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        signalBus.Fire<OnFaceEquippedSignal>(new OnFaceEquippedSignal()
        {
            FaceItem = faceItem
        });
        signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
        {
            CustomizationItems = new List<ThemeCustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
        });
    }
    public void EquipTorso(TorsoItem torsoItem)
    {
        playerInventoryAddressedData.currentTorso = torsoItem.name;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
      signalBus.Fire(new OnTorsoEquippedSignal()
      {
          TorsoItem = torsoItem
      });
      signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
      {
          CustomizationItems = new List<ThemeCustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
      });
    }
    public void EquipLegs(LegsItem legsItem)
    {
        playerInventoryAddressedData.currentLegs = legsItem.name;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
            signalBus.Fire<OnLegsEquippedSignal>(new OnLegsEquippedSignal()
            {
                LegsItem = legsItem
            });
            signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
            {
                CustomizationItems = new List<ThemeCustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
            });
    }
    public void EquipFeet(FeetItem feetItem)
    {
        playerInventoryAddressedData.currentFeet = feetItem.name;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
       signalBus.Fire<OnFeetEquippedSignal>(new OnFeetEquippedSignal()
       {
           FeetItem = feetItem
       });
       signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
       {
           CustomizationItems = new List<ThemeCustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
       });
    }

    public void EquipRoomItem(ThemeCustomizationItem themeCustomizationItem)
    {
        signalBus.Fire(new OnPlayerRoomThemeItemEquippedSignal()
        {
            ThemeCustomizationItem = themeCustomizationItem
        });
        signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
        {
            CustomizationItems = new List<ThemeCustomizationItem>(RoomItems){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
        });
    }

  
}
[System.Serializable]
    public class PlayerInventoryAddressedData
    {
        public List<string> headItemsNames;
        public  List<string> faceItemsNames; 
        public  List<string> torsoItemsNames;
        public List<string> legsItemsNames;
        public  List<string> feetItemsNames;
        public List<string> roomItemsNames;
        public string currentHead;
        public string currentFace;
        public string currentTorso;
        public string currentLegs;
        public string currentFeet;
        public List<string> equippedRoomItems;
      public  PlayerInventoryAddressedData()
        {
            headItemsNames = new List<string>();
            legsItemsNames = new List<string>();
            faceItemsNames = new List<string>();
            feetItemsNames = new List<string>();
            torsoItemsNames = new List<string>();
        }
        
    }