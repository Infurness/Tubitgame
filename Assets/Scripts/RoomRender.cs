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
    [Inject] private PlayerInventory PlayerInventory;
    [SerializeField] private SpriteRenderer computerSlot;
    [SerializeField] private SpriteRenderer cameraSlot;
    [SerializeField] private SpriteRenderer microphoneSlot;
    [SerializeField] private SpriteRenderer greenScreenSlot;
    [SerializeField] private List<WallSlotData> wallSlots;
    [SerializeField] private List<FloorSlotData> floorSlots;
    [SerializeField] private List<ObjectSlotData> roomObjectSlots;
    [SerializeField] private List<Image> floorItems;
    [SerializeField] private Canvas roomCanvas;
    [SerializeField]  List<VideoQualityCustomizationItem> currentVQItems;
    [SerializeField]  List<ThemeCustomizationItem> currentThemeItems;
    [SerializeField] private RoomLayout DefalutRoomLayout;
    [SerializeField] private RoomLayout RoomLayout;
    [SerializeField] private Camera renderCamera;
    [SerializeField] private float zoomInValue, ZoomOutValue;
    private void Awake()
    {
        currentVQItems = new List<VideoQualityCustomizationItem>();
        currentThemeItems = new List<ThemeCustomizationItem>();

    }

    public void ZoomInRoomRender()
    {
        renderCamera.orthographicSize = zoomInValue;
    }

    public void ZoomOutRoomRender()
    {
        renderCamera.orthographicSize = ZoomOutValue;

    }
    void Start()
    {
        signalBus.Subscribe<TestRoomThemeItemSignal>(OnTestRoomThemeItem);
        signalBus.Subscribe<TestRoomVideoQualityITemSignal>(OnTestVideoQualityItem);
        
        foreach (var wallSlot in wallSlots)
        {
            if (wallSlot.Empty)
            {
                wallSlot.Image.gameObject.SetActive(false);
            }
        }
        foreach (var floorSlot in floorSlots)    
        {
            if (floorSlot.Empty)
            {
                floorSlot.Image.gameObject.SetActive(false);
            }
        }

        foreach (var objectSlot in roomObjectSlots)
        {
            if (objectSlot.Empty)
            {
                objectSlot.Image.gameObject.SetActive(false);
            }
        }
        signalBus.Subscribe<SaveRoomLayoutSignal>((signal =>
        {
            OnSaveRoomLayout();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }));
        signalBus.Subscribe<DiscardRoomLayoutSignal>((signal =>
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        } ));
        ZoomInRoomRender();
    }

    private void OnEnable()
    {
        currentVQItems=PlayerInventory.EquippedVideoQualityRoomItems;
        currentThemeItems = PlayerInventory.EquippedThemeEffectRoomItems;
        RoomLayout = PlayerInventory.GetRoomLayout();
        if (RoomLayout.FloorLayoutSlots.Count==0)
        {
            RoomLayout = DefalutRoomLayout;
        }
        print("Room Enabled");
            ProcessCurrentVCItems();
            ProcessCurrentThemeItems();
    }

    public void ProcessCurrentThemeItems()
    {
        foreach (var wallSlot in RoomLayout.WallLayoutSlots)
        {
           var item= currentThemeItems.Find((it)=>it.name==wallSlot.ItemName);
            wallSlots[wallSlot.SlotID].Image.sprite =
                ((WallOrnament) item).wallOrnamentSprite;
        }

        foreach (var floorLayoutSlot in RoomLayout.FloorLayoutSlots)
        {
            var item= currentThemeItems.Find((it)=>it.name==floorLayoutSlot.ItemName);

            if (item!=null)
            {
                floorSlots[floorLayoutSlot.SlotID].Image.sprite =

                    ((FloorOrnament) item).floorOrnamentSprite;
            }
            else
            {
                print("Floor ITem " + floorLayoutSlot.ItemName +"Not Found");
            }
           
        }

        foreach (var objectLayoutSlot in RoomLayout.ObjectsLayoutSlots)
        {
            var item= currentThemeItems.Find((it)=>it.name==objectLayoutSlot.ItemName);

            roomObjectSlots[objectLayoutSlot.SlotID].Image.sprite =
                ((RoomObject) item).roomObjectSprite;
        }
    }

    public void ProcessCurrentVCItems()
    {
        foreach (var vqItem in currentVQItems)
        {
            
            switch (vqItem.videoQualityItemType)
            {
                case VideoQualityItemType.Computer:
                    computerSlot.gameObject.SetActive(true);
                    computerSlot.sprite = vqItem.itemSprite;
                    
                    break;
                case VideoQualityItemType.Camera:
                    cameraSlot.gameObject.SetActive(true);
                    cameraSlot.sprite = vqItem.itemSprite;
                  
                    break;
                case VideoQualityItemType.Microphone:
                    microphoneSlot.gameObject.SetActive(true);
                    microphoneSlot.sprite = vqItem.itemSprite;
                  
                    break;
                case VideoQualityItemType.GreenScreen:
                    greenScreenSlot.gameObject.SetActive(true);
                    greenScreenSlot.sprite = vqItem.itemSprite;
                
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void OnTestVideoQualityItem(TestRoomVideoQualityITemSignal testRoomVideoQualityITemSignal)
    {
        var item = testRoomVideoQualityITemSignal.VideoQualityCustomizationItem;
        switch (item.videoQualityItemType)
        {
            case VideoQualityItemType.Computer:
                computerSlot.gameObject.SetActive(true);
                computerSlot.sprite = item.itemSprite;
                var computer=   currentVQItems.Find((vci) => vci.videoQualityItemType == VideoQualityItemType.Computer);
                if (computer!=null)
                {
                    currentVQItems.Remove(computer);
                }
                currentVQItems.Add(item);
                break;
            case VideoQualityItemType.Camera:
                cameraSlot.gameObject.SetActive(true);
                cameraSlot.sprite = item.itemSprite;
                var camera = currentVQItems.Find(vci => vci.videoQualityItemType == VideoQualityItemType.Camera);
                if (camera!=null)
                {
                    currentVQItems.Remove(camera);
                }
                currentVQItems.Add(item);
                break;
            case VideoQualityItemType.Microphone:
                microphoneSlot.gameObject.SetActive(true);
                microphoneSlot.sprite = item.itemSprite;
                var mic = currentVQItems.Find(vci => vci.videoQualityItemType == VideoQualityItemType.Microphone);
                if (mic!=null)
                {
                    currentVQItems.Remove(mic);
                }
                currentVQItems.Add(item);
                break;
            case VideoQualityItemType.GreenScreen:
                greenScreenSlot.gameObject.SetActive(true);
                greenScreenSlot.sprite = item.itemSprite;
                var greenScreen = currentVQItems.Find(vci => vci.videoQualityItemType == VideoQualityItemType.Microphone);
                if (greenScreen!=null)
                {
                    currentVQItems.Remove(greenScreen);
                }
                currentVQItems.Add(item);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnTestRoomThemeItem(TestRoomThemeItemSignal testRoomThemeItem)
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
        slot.roomLayoutThemeSlot.ItemName = wallOrnament.name;
        DefalutRoomLayout.WallLayoutSlots[slot.roomLayoutThemeSlot.SlotID].ItemName = wallOrnament.name;

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
       slot.roomLayoutThemeSlot.ItemName = floorOrnament.name;
       DefalutRoomLayout.FloorLayoutSlots[slot.roomLayoutThemeSlot.SlotID].ItemName = floorOrnament.name;

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
        slot.roomLayoutThemeSlot.ItemName = roomObject.name;
        DefalutRoomLayout.ObjectsLayoutSlots[slot.roomLayoutThemeSlot.SlotID].ItemName = roomObject.name;
    }

    public void OnSaveRoomLayout()
    {
      PlayerInventory.UpdateRoomData(RoomLayout,currentVQItems);
    }

    public void PopulateDataSlots()
    {
        DefalutRoomLayout.FloorLayoutSlots.Clear();
        DefalutRoomLayout.WallLayoutSlots.Clear();
        DefalutRoomLayout.ObjectsLayoutSlots.Clear();

        for (int i = 0; i < wallSlots.Count; i++)
        {
            wallSlots[i].roomLayoutThemeSlot.SlotID =  i;
            DefalutRoomLayout.WallLayoutSlots.Add(wallSlots[i].roomLayoutThemeSlot);
        }
        for (int i = 0; i < floorSlots.Count; i++)
        {
            floorSlots[i].roomLayoutThemeSlot.SlotID = i;
            DefalutRoomLayout.FloorLayoutSlots.Add(floorSlots[i].roomLayoutThemeSlot);
        }
        for (int i = 0; i < roomObjectSlots.Count; i++)
        {
            roomObjectSlots[i].roomLayoutThemeSlot.SlotID = i;
            DefalutRoomLayout.ObjectsLayoutSlots.Add(roomObjectSlots[i].roomLayoutThemeSlot);
        }
        
    }
}

[System.Serializable]
public class WallSlotData
{
    public SpriteRenderer Image;
    public WallOrnamentType WallOrnamentType;
    public bool Empty=true;
    public RoomLayoutThemeSlot roomLayoutThemeSlot;


}
[System.Serializable]
public class FloorSlotData
{
    public SpriteRenderer Image;
    public FloorOrnamentType FloorOrnamentType;
    public bool Empty=true;
    public RoomLayoutThemeSlot roomLayoutThemeSlot;



}
[System.Serializable]
public class ObjectSlotData
{
    public SpriteRenderer Image;
    public RoomObjectType RoomObjectType;
    public bool Empty=true;
    public RoomLayoutThemeSlot roomLayoutThemeSlot;

}




