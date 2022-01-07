using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Customizations;
using ModestTree;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Zenject;

public class RoomRender : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlayerInventory PlayerInventory;

    [SerializeField] List<VideoQualityCustomizationItem> currentVQItems;
    [SerializeField] List<ThemeCustomizationItem> currentThemeItems;
    [SerializeField] private RoomLayout DefaultRoomLayout;
    [SerializeField] private RoomLayout RoomLayout;
    [SerializeField] private GameObject floorObject, rightWall, leftWall,desk,chair,seatedCharacter;
    private void Awake()
    {
        currentVQItems = new List<VideoQualityCustomizationItem>();
        currentThemeItems = new List<ThemeCustomizationItem>();
       
    }

    void Start()
    {
        
        signalBus.Subscribe<TestRoomThemeItemSignal>(OnTestRoomThemeItem);
        signalBus.Subscribe<TestRoomVideoQualityITemSignal>(OnTestVideoQualityItem);


        signalBus.Subscribe<SaveRoomLayoutSignal>((signal => { OnSaveRoomLayout(); }));
        signalBus.Subscribe<DiscardRoomLayoutSignal>((signal =>
        {
            RoomLayout = new RoomLayout(PlayerInventory.GetRoomLayout());
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }));
    }

    private void OnEnable()
    {
        RoomLayout = new RoomLayout(PlayerInventory.GetRoomLayout());
        if (RoomLayout.roomSlots.Count==0)
        {
            RoomLayout = DefaultRoomLayout;
        }
        PopulateRoomLayout();
        print("Room Enabled");
        
    }

    async void  PopulateRoomLayout()
    {
        foreach (var roomSlot in RoomLayout.roomSlots)
        {
            AsyncOperationHandle<GameObject> loadOp;

            switch (roomSlot.customizationType)
            {
                case CustomizationType.Theme:
                     var themItem = PlayerInventory.RoomThemeEffectItems.Find((item => roomSlot.ItemName == item.name));
                      loadOp = Addressables.InstantiateAsync(themItem.itemPrefab);
                     await loadOp.Task;
                    break;
                case CustomizationType.VideoQuality:
                     var vcItem = PlayerInventory.VideoQualityRoomItems.Find((item => roomSlot.ItemName == item.name));
                      loadOp = Addressables.InstantiateAsync(vcItem.itemPrefab);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var go = loadOp.Result;
            switch (roomSlot.ParentType)
            {
                case RoomParentType.Floor:
                    go.transform.parent = floorObject.transform;
                    break;
                case RoomParentType.RightWall:
                    go.transform.parent = rightWall.transform;
                    break;
                case RoomParentType.LeftWall:
                    go.transform.parent = leftWall.transform;
                    break;
                case RoomParentType.Desk:
                    go.transform.parent = desk.transform;
                    break;
                case RoomParentType.Chair:
                    go.transform.parent = chair.transform;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            go.transform.localPosition = roomSlot.Position;
            go.transform.localScale = roomSlot.Scale;

            var objectdata = go.GetComponent<RoomObjectData>();
            objectdata.assetName = roomSlot.ItemName;
   
        }
    }

   
    async void OnTestVideoQualityItem(TestRoomVideoQualityITemSignal testRoomVideoQualityITemSignal)
    {
        var item = testRoomVideoQualityITemSignal.VideoQualityCustomizationItem;
        var loadOp = Addressables.InstantiateAsync(item.itemPrefab);
        await loadOp.Task;
        var go = loadOp.Result;
        var objecdata = go.GetComponent<RoomObjectData>();
        switch (objecdata.parentType)
        {
            case RoomParentType.Floor:
                go.transform.parent = floorObject.transform;
                break;
            case RoomParentType.RightWall:
                go.transform.parent = rightWall.transform;
                break;
            case RoomParentType.LeftWall:
                go.transform.parent = leftWall.transform;
                break;
            case RoomParentType.Desk:
                go.transform.parent = desk.transform;
                break;
            case RoomParentType.Chair:
                go.transform.parent = chair.transform;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        objecdata.assetName = item.name;
    }

    async void OnTestRoomThemeItem(TestRoomThemeItemSignal testRoomThemeItem)
    {
        var item = testRoomThemeItem.ThemeCustomizationItem;
        var loadOp = Addressables.InstantiateAsync(item.itemPrefab);
        await loadOp.Task;
        var go = loadOp.Result;
        var objecdata = go.GetComponent<RoomObjectData>();
        switch (objecdata.parentType)
        {
            case RoomParentType.Floor:
                go.transform.parent = floorObject.transform;
                break;
            case RoomParentType.RightWall:
                go.transform.parent = rightWall.transform;
                break;
            case RoomParentType.LeftWall:
                go.transform.parent = leftWall.transform;
                break;
            case RoomParentType.Desk:
                go.transform.parent = desk.transform;
                break;
            case RoomParentType.Chair:
                go.transform.parent = chair.transform;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        objecdata.assetName = item.name;
    }

    
    

    public  void OnSaveRoomLayout()
    {
        SaveLayout();
       PlayerInventory.UpdateRoomData(RoomLayout,currentVQItems);
      gameObject.SetActive(false);
      gameObject.SetActive(true);
    }

    void SaveLayout()
    {
        RoomLayout newLayout = new RoomLayout();

        var roomObjectsData = FindObjectsOfType<RoomObjectData>();
        if (roomObjectsData.Length==0)
        {
            return;
        }
        foreach (var ob in roomObjectsData)
        {
            var newslot = new RoomLayoutSerializedSlot();
            newslot.ItemName = ob.assetName;
            newslot.customizationType = ob.customizationType;
            newslot.ParentType = ob.parentType;
            newslot.Position = ob.transform.localPosition;
            newslot.Scale = ob.transform.localScale;
            newLayout.roomSlots.Add(newslot);
        }

        RoomLayout = newLayout;
    }
    public void PopulateDataSlots()
    {
       
    }
}





