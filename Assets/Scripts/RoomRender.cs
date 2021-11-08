using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Customizations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoomRender : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private Image computer;
    [SerializeField] private Image camera;
    [SerializeField] private Image microphone;
    [SerializeField] private List<WallSlotData> wallSlots;
    [SerializeField] private List<FloorSlotData> floorSlots;
    [SerializeField] private List<ObjectSlotData> roomObjectSlots;

    [SerializeField] private List<Image> floorItems;
    [SerializeField] private Canvas roomCanvas;
    private Dictionary<string, VideoQualityCustomizationItem> testedVQItems;
    private Dictionary<string, ThemeCustomizationItem> testedThemeItems;


    void Start()
    {
        testedVQItems = new Dictionary<string, VideoQualityCustomizationItem>();
        testedThemeItems = new Dictionary<string, ThemeCustomizationItem>();
        signalBus.Subscribe<TestRoomThemeItemSignal>(OnEquipRoomThemeItemFired);
        signalBus.Subscribe<TestRoomVideoQualityITemSignal>(OnTestVideoQualityItem);
    }

    public void OnTestVideoQualityItem(TestRoomVideoQualityITemSignal testRoomVideoQualityITemSignal)
    {
        var item = testRoomVideoQualityITemSignal.VideoQualityCustomizationItem;
        switch (item.videoQualityItemType)
        {
            case VideoQualityItemType.Computer:
                computer.gameObject.SetActive(true);
                computer.sprite = item.itemSprite;
                testedVQItems.Remove("computer");
                testedVQItems.Add("computer",item);
                break;
            case VideoQualityItemType.Camera:
                camera.gameObject.SetActive(true);
                camera.sprite = item.itemSprite;
                testedVQItems.Remove("camera");
                testedVQItems.Add("camera",item);
                break;
            case VideoQualityItemType.Microphone:
                microphone.gameObject.SetActive(true);
                microphone.sprite = item.itemSprite;
                testedVQItems.Remove("mic");
                testedVQItems.Add("mic",item);
                break;
            case VideoQualityItemType.GreenScreen:
                testedVQItems.Remove("greenscreen");
                testedVQItems.Add("greenscreen",item);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnEquipRoomThemeItemFired(TestRoomThemeItemSignal testRoomThemeItem)
    {
        switch (testRoomThemeItem.ThemeCustomizationItem)
        {
            case WallOrnament wallOrnament : AddWallOrnament(wallOrnament); break;
            case FloorOrnament floorOrnament: AddFloorOrnament(floorOrnament); break;
            case RoomObject roomObject : AddRoomObject(roomObject); break;
            
        }
    }

    void AddWallOrnament(WallOrnament wallOrnament)
    {
        var slot = wallSlots.Find((item =>(item.WallOrnamentType == wallOrnament.WallOrnamentType)&&item.Empty));
        if (slot==null)
        {
            return;
        }
        slot.Image.sprite = wallOrnament.wallOrnamentSprite;
        slot.Empty = false;
        

    }

    void AddFloorOrnament(FloorOrnament floorOrnament)
    {
       var slot=floorSlots.Find((item =>(item.FloorOrnamentType == floorOrnament.floorOrnamentType)&&item.Empty));
       if (slot==null)
       {
           return;
       }
       slot.Image.sprite = floorOrnament.floorOrnamentSprite;
       slot.Empty = false;
    }

    void AddRoomObject(RoomObject roomObject)
    {
        var slot=roomObjectSlots.Find((item =>(item.RoomObjectType == roomObject.roomObjectType)&&item.Empty));
        if (slot==null)
        {
            return;
        }
        slot.Image.sprite = roomObject.roomObjectSprite;
        slot.Empty = false;
    }

    public void SaveRoomLayout()
    {
        var items = testedVQItems.ToList();
        signalBus.Fire(new OnPlayerRoomVideoQualityItemsEquippedSignal(){});
    }
 
}

[System.Serializable]
public class WallSlotData
{
    public Image Image;
    public WallOrnamentType WallOrnamentType;
    public bool Empty;
}
[System.Serializable]
public class FloorSlotData
{
    public Image Image;
    public FloorOrnamentType FloorOrnamentType;
    public bool Empty;
}
[System.Serializable]
public class ObjectSlotData
{
    public Image Image;
    public RoomObjectType RoomObjectType;
    public bool Empty=true;
}
