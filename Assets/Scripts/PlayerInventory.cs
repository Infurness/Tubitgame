using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class PlayerInventory : MonoBehaviour
{
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private SignalBus signalBus;
    private PlayerInventoryData playerInventoryData;
    void Start()
    {
        signalBus.Subscribe<OnPlayerInventoryFetchedSignal>(OnPlayerInventoryFetched);
      
    }

    void OnPlayerInventoryFetched(OnPlayerInventoryFetchedSignal playerInventoryFetchedSignal)
    {
        playerInventoryData = playerInventoryFetchedSignal.PlayerInventoryData;
        signalBus.Fire(new OnHeadEquippedSignal()
        {
            HeadItem = playerInventoryData.currentHead
        });
        
        signalBus.Fire(new OnFaceEquippedSignal()
        {
            FaceItem = playerInventoryData.currentFace
        });
        signalBus.Fire(new OnTorsoEquippedSignal()
        {
            TorsoItem = playerInventoryData.currentTorso
        });
        signalBus.Fire(new OnLegsEquippedSignal()
        { 
            LegsItem= playerInventoryData.currentLegs
        });
        signalBus.Fire(new OnFeetEquippedSignal()
        {
            FeetItem= playerInventoryData.currentFeet
        });
    }
    public void AddHeadItem(HeadItem headItem)
    {
        if (playerInventoryData.headItemsNames.Contains(headItem.name))
            return;
        playerInventoryData.headItemsNames.Add(headItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
        
    }
    public void AddFaceItem(FaceItem faceItem)
    {
        if (playerInventoryData.faceItemsNames.Contains(faceItem.name))
            return;
        playerInventoryData.faceItemsNames.Add(faceItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
        
    }
    public void AddTorsoItem(TorsoItem torsoItem)
    {
        if (playerInventoryData.torsoItemsNames.Contains(torsoItem.name))
            return;
        playerInventoryData.torsoItemsNames.Add(torsoItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
        
    }
    public void AddLegsItem(LegsItem legsItem)
    {
        if (playerInventoryData.legsItemsNames.Contains(legsItem.name))
            return;
        playerInventoryData.legsItemsNames.Add(legsItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
        
    }
    public void AddFeetItem(FeetItem feetItem)
    {
        if (playerInventoryData.feetItemsNames.Contains(feetItem.name))
            return;
        playerInventoryData.feetItemsNames.Add(feetItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
        
    }

    public void EquipHead(HeadItem headItem)
    {
        playerInventoryData.currentHead = headItem;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
        signalBus.Fire<OnHeadEquippedSignal>(new OnHeadEquippedSignal()
        {
            HeadItem = headItem
        });
        signalBus.Fire<OnPlayerEquippedItemChangedSignal>(new OnPlayerEquippedItemChangedSignal()
        {
            PlayerInventoryData = this.playerInventoryData
        });

    }
    public void EquipFace(FaceItem faceItem)
    {
        playerInventoryData.currentFace = faceItem;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
        signalBus.Fire<OnFaceEquippedSignal>(new OnFaceEquippedSignal()
        {
            FaceItem = faceItem
        });
        signalBus.Fire<OnPlayerEquippedItemChangedSignal>(new OnPlayerEquippedItemChangedSignal()
        {
            PlayerInventoryData = this.playerInventoryData
        });
    }
    public void EquipTorso(TorsoItem torsoItem)
    {
        playerInventoryData.currentTorso = torsoItem;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
      signalBus.Fire<OnTorsoEquippedSignal>(new OnTorsoEquippedSignal()
      {
          TorsoItem = torsoItem
      });
      signalBus.Fire<OnPlayerEquippedItemChangedSignal>(new OnPlayerEquippedItemChangedSignal()
      {
          PlayerInventoryData = this.playerInventoryData
      });
    }
    public void EquipLegs(LegsItem legsItem)
    {
        playerInventoryData.currentLegs = legsItem;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
            signalBus.Fire<OnLegsEquippedSignal>(new OnLegsEquippedSignal()
            {
                LegsItem = legsItem
            });
            signalBus.Fire<OnPlayerEquippedItemChangedSignal>(new OnPlayerEquippedItemChangedSignal()
            {
                PlayerInventoryData = this.playerInventoryData
            });
    }
    public void EquipFeet(FeetItem feetItem)
    {
        playerInventoryData.currentFeet = feetItem;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryData);
       signalBus.Fire<OnFeetEquippedSignal>(new OnFeetEquippedSignal()
       {
           FeetItem = feetItem
       });
       signalBus.Fire<OnPlayerEquippedItemChangedSignal>(new OnPlayerEquippedItemChangedSignal()
       {
           PlayerInventoryData = this.playerInventoryData
       });
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
    public class PlayerInventoryData
    {
        public List<string> headItemsNames;
        public  List<string> faceItemsNames; 
        public  List<string> torsoItemsNames;
        public List<string> legsItemsNames;
        public  List<string> feetItemsNames;

        public HeadItem currentHead;
        public FaceItem currentFace;
        public TorsoItem currentTorso;
        public LegsItem currentLegs;
        public FeetItem currentFeet;
        
      public  PlayerInventoryData()
        {
            headItemsNames = new List<string>();
            legsItemsNames = new List<string>();
            faceItemsNames = new List<string>();
            feetItemsNames = new List<string>();
            torsoItemsNames = new List<string>();
        }
        
    }