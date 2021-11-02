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
    [SerializeField] PlayerInventoryAddressedData m_PlayerInventoryAddressedData;
    public HeadItem currentHead;
    public FaceItem currentFace;
    public TorsoItem currentTorso;
    public LegsItem currentLegs;
    public FeetItem currentFeet;
    public List<HeadItem> HeadItems;
    public List<FaceItem> FaceItems;
    public List<TorsoItem> TorsoItems;
    public List<LegsItem> LegsItems;
    public List<FeetItem> FeetItems;
    void Start()
    {
     //  signalBus.Subscribe<OnPlayerInventoryFetchedSignal>(OnPlayerInventoryFetched);
     LoadAddressedData();
    }

     async void LoadAddressedData()
    {
        var assets= Addressables.LoadAssetsAsync<CustomizationItem>("default", null);
        await assets.Task;
        if (assets.Status == AsyncOperationStatus.Succeeded)
        {
            var items= (List<CustomizationItem>) assets.Result;
            
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

            currentHead = HeadItems.Find((hi) => hi.name == m_PlayerInventoryAddressedData.currentHead);
            currentFace = FaceItems.Find(item => item.name == m_PlayerInventoryAddressedData.currentFace);
            currentTorso = TorsoItems.Find(item => item.name == m_PlayerInventoryAddressedData.currentTorso);
            currentFeet = FeetItems.Find(item => item.name == m_PlayerInventoryAddressedData.currentFeet);
            currentLegs = LegsItems.Find(item => item.name == m_PlayerInventoryAddressedData.currentLegs);

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
        m_PlayerInventoryAddressedData = playerInventoryFetchedSignal.PlayerInventoryAddressedData;
        
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
        if (m_PlayerInventoryAddressedData.headItemsNames.Contains(headItem.name))
            return;
        m_PlayerInventoryAddressedData.headItemsNames.Add(headItem.name);
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
        
    }
    public void AddFaceItem(FaceItem faceItem)
    {
        if (m_PlayerInventoryAddressedData.faceItemsNames.Contains(faceItem.name))
            return;
        m_PlayerInventoryAddressedData.faceItemsNames.Add(faceItem.name);
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
        
    }
    public void AddTorsoItem(TorsoItem torsoItem)
    {
        if (m_PlayerInventoryAddressedData.torsoItemsNames.Contains(torsoItem.name))
            return;
        m_PlayerInventoryAddressedData.torsoItemsNames.Add(torsoItem.name);
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
        
    }
    public void AddLegsItem(LegsItem legsItem)
    {
        if (m_PlayerInventoryAddressedData.legsItemsNames.Contains(legsItem.name))
            return;
        m_PlayerInventoryAddressedData.legsItemsNames.Add(legsItem.name);
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
        
    }
    public void AddFeetItem(FeetItem feetItem)
    {
        if (m_PlayerInventoryAddressedData.feetItemsNames.Contains(feetItem.name))
            return;
        m_PlayerInventoryAddressedData.feetItemsNames.Add(feetItem.name);
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
        
    }

    public void EquipHead(HeadItem headItem)
    {
        m_PlayerInventoryAddressedData.currentHead = headItem.name;
        signalBus.Fire<OnHeadEquippedSignal>(new OnHeadEquippedSignal()
        {
            HeadItem = headItem
        });
        signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
        {
            CustomizationItems = new List<CustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
        });
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);


    }
    public void EquipFace(FaceItem faceItem)
    {
        m_PlayerInventoryAddressedData.currentFace = faceItem.name;
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
        signalBus.Fire<OnFaceEquippedSignal>(new OnFaceEquippedSignal()
        {
            FaceItem = faceItem
        });
        signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
        {
            CustomizationItems = new List<CustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
        });
    }
    public void EquipTorso(TorsoItem torsoItem)
    {
        m_PlayerInventoryAddressedData.currentTorso = torsoItem.name;
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
      signalBus.Fire(new OnTorsoEquippedSignal()
      {
          TorsoItem = torsoItem
      });
      signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
      {
          CustomizationItems = new List<CustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
      });
    }
    public void EquipLegs(LegsItem legsItem)
    {
        m_PlayerInventoryAddressedData.currentLegs = legsItem.name;
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
            signalBus.Fire<OnLegsEquippedSignal>(new OnLegsEquippedSignal()
            {
                LegsItem = legsItem
            });
            signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
            {
                CustomizationItems = new List<CustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
            });
    }
    public void EquipFeet(FeetItem feetItem)
    {
        m_PlayerInventoryAddressedData.currentFeet = feetItem.name;
        playerDataManager.UpdatePlayerInventoryData(m_PlayerInventoryAddressedData);
       signalBus.Fire<OnFeetEquippedSignal>(new OnFeetEquippedSignal()
       {
           FeetItem = feetItem
       });
       signalBus.Fire(new OnPlayerEquippedItemChangedSignal()
       {
           CustomizationItems = new List<CustomizationItem>(){currentFace,currentFeet,currentHead,currentLegs,currentTorso}
       });
    }


    // Update is called once per frame
    void Update()
    {
        
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

        public string currentHead;
        public string currentFace;
        public string currentTorso;
        public string currentLegs;
        public string currentFeet;
        
      public  PlayerInventoryAddressedData()
        {
            headItemsNames = new List<string>();
            legsItemsNames = new List<string>();
            faceItemsNames = new List<string>();
            feetItemsNames = new List<string>();
            torsoItemsNames = new List<string>();
        }
        
    }