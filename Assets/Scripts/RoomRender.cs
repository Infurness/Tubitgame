using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Customizations;
using ModestTree;
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
    [SerializeField] List<VideoQualityCustomizationItem> currentVQItems;
    [SerializeField] List<ThemeCustomizationItem> currentThemeItems;
    [SerializeField] private RoomLayout DefaultRoomLayout;
    [SerializeField] private RoomLayout RoomLayout;
    private Camera renderCamera;
    [SerializeField] private float zoomInValue, ZoomOutValue;
    [SerializeField] private Vector3 zoomInPosition, zoomOutPosition;

    private void Awake()
    {
        renderCamera=Camera.main;
        currentVQItems = new List<VideoQualityCustomizationItem>();
        currentThemeItems = new List<ThemeCustomizationItem>();
        signalBus.Subscribe<RoomZoomStateChangedSignal>((signal =>
        {
            if (signal.ZoomIn)
            {
                ZoomInRoomRender();
            }
            else
            {
                ZoomOutRoomRender();
            }
        }));
    }

    void ZoomInRoomRender()
    {
        renderCamera.orthographicSize = zoomInValue;
        transform.localPosition = zoomInPosition;
        print("ZOOM IN");

    }

    void ZoomOutRoomRender()
    {
        renderCamera.orthographicSize = ZoomOutValue;
        transform.localPosition = zoomOutPosition;

        print("ZOOM OUT");

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
        ZoomInRoomRender();
    }

    private void OnEnable()
    {
        RoomLayout = new RoomLayout(PlayerInventory.GetRoomLayout());
        if (RoomLayout.FloorLayoutSlots.IsEmpty() && RoomLayout.WallLayoutSlots.IsEmpty())
        {
            currentVQItems = PlayerInventory.VideoQualityRoomItems;
            currentThemeItems = PlayerInventory.RoomThemeEffectItems;
            RoomLayout = new RoomLayout(DefaultRoomLayout);
            print("Using Default Layout");
        }
        else
        {
            currentVQItems = PlayerInventory.EquippedVideoQualityRoomItems;
            currentThemeItems = PlayerInventory.EquippedThemeEffectRoomItems;
           
        }
        print("Room Enabled");
            ProcessCurrentVCItems();
            ProcessCurrentThemeItems();
    }

    public void ProcessCurrentThemeItems()
    {
        var colliders = GetComponentsInChildren<PolygonCollider2D>();
        foreach (var collider in colliders)
        {
            Destroy(collider);
        }
        foreach (var wallSlot in RoomLayout.WallLayoutSlots)
        {
           var item= currentThemeItems.Find((it)=>it.name==wallSlot.ItemName);
            wallSlots[wallSlot.SlotID].Image.sprite =
                ((WallOrnament) item).wallOrnamentSprite;
            wallSlots[wallSlot.SlotID].Image.gameObject.SetActive(true);
            wallSlots[wallSlot.SlotID].Image.gameObject.AddComponent<PolygonCollider2D>();
        }

        foreach (var floorLayoutSlot in RoomLayout.FloorLayoutSlots)
        {
            var item= currentThemeItems.Find((it)=>it.name==floorLayoutSlot.ItemName);

            if (item!=null)
            {
                floorSlots[floorLayoutSlot.SlotID].Image.sprite =

                    ((FloorOrnament) item).floorOrnamentSprite;
                floorSlots[floorLayoutSlot.SlotID].Image.gameObject.SetActive(true);
               floorSlots[floorLayoutSlot.SlotID].Image.gameObject.AddComponent<PolygonCollider2D>();
                print("Floor ITem " + floorLayoutSlot.ItemName +" Found");


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
            roomObjectSlots[objectLayoutSlot.SlotID].Image.gameObject.SetActive(true);
            
            roomObjectSlots[objectLayoutSlot.SlotID].Image.gameObject.AddComponent<PolygonCollider2D>();
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
        print("Item Received "+wallOrnament.name);

        var slot = wallSlots.Find((item =>(item.WallOrnamentType == wallOrnament.WallOrnamentType)));
        
        if (slot==null)
        {
            print("Slot Not founded");

            return;
        }
        
        slot.Image.sprite = wallOrnament.wallOrnamentSprite;
        slot.Empty = false;
        slot.roomLayoutThemeSlot.ItemName = wallOrnament.name;
        slot.Image.gameObject.SetActive(true);
        RoomLayout.WallLayoutSlots[slot.roomLayoutThemeSlot.SlotID] = new RoomLayoutThemeSlot(wallOrnament.name, slot.roomLayoutThemeSlot.SlotID);

    }

    void AddFloorOrnament(FloorOrnament floorOrnament)
    {
        print("Item Received "+floorOrnament.name);
       var slot=floorSlots.Find((item =>(item.FloorOrnamentType == floorOrnament.floorOrnamentType)));
       if (slot==null)
       {
           print("Slot Not founded");
           return;
       }
       slot.Image.sprite = floorOrnament.floorOrnamentSprite;
       slot.Empty = false;
       slot.roomLayoutThemeSlot.ItemName = floorOrnament.name;
       slot.Image.gameObject.SetActive(true);

       RoomLayout.FloorLayoutSlots[slot.roomLayoutThemeSlot.SlotID] =new RoomLayoutThemeSlot(floorOrnament.name,slot.roomLayoutThemeSlot.SlotID);

    }

    void AddRoomObject(RoomObject roomObject)
    {
        var slot=roomObjectSlots.Find((item =>(item.RoomObjectType == roomObject.roomObjectType)));
        if (slot==null)
        {
            print("Slot Not founded");

            return;
        }
        slot.Image.sprite = roomObject.roomObjectSprite;
        slot.Empty = false;
        slot.roomLayoutThemeSlot.ItemName = roomObject.name;
        slot.Image.gameObject.SetActive(true);

        RoomLayout.ObjectsLayoutSlots[slot.roomLayoutThemeSlot.SlotID] = new RoomLayoutThemeSlot(roomObject.name,slot.roomLayoutThemeSlot.SlotID);
    }

    public  void OnSaveRoomLayout()
    {
       PlayerInventory.UpdateRoomData(RoomLayout,currentVQItems);
      gameObject.SetActive(false);
      gameObject.SetActive(true);
    }

    public void PopulateDataSlots()
    {
        DefaultRoomLayout.FloorLayoutSlots.Clear();
        DefaultRoomLayout.WallLayoutSlots.Clear();
        DefaultRoomLayout.ObjectsLayoutSlots.Clear();

        for (int i = 0; i < wallSlots.Count; i++)
        {
            wallSlots[i].roomLayoutThemeSlot.SlotID =  i;
            DefaultRoomLayout.WallLayoutSlots.Add(wallSlots[i].roomLayoutThemeSlot);
        }
        for (int i = 0; i < floorSlots.Count; i++)
        {
            floorSlots[i].roomLayoutThemeSlot.SlotID = i;
            DefaultRoomLayout.FloorLayoutSlots.Add(floorSlots[i].roomLayoutThemeSlot);
        }
        for (int i = 0; i < roomObjectSlots.Count; i++)
        {
            roomObjectSlots[i].roomLayoutThemeSlot.SlotID = i;
            DefaultRoomLayout.ObjectsLayoutSlots.Add(roomObjectSlots[i].roomLayoutThemeSlot);
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




