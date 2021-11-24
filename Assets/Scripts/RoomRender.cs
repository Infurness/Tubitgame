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
            wallSlots[wallSlot.SlotID].Image.gameObject.transform.localPosition = wallSlot.Position;
         //   wallSlots[wallSlot.SlotID].Image.gameObject.transform.localScale = wallSlot.Scale;
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
                floorSlots[floorLayoutSlot.SlotID].Image.gameObject.transform.localPosition = floorLayoutSlot.Position;
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
            roomObjectSlots[objectLayoutSlot.SlotID].Image.transform.localPosition = objectLayoutSlot.Position;
            roomObjectSlots[objectLayoutSlot.SlotID].Image.gameObject.AddComponent<PolygonCollider2D>();
        }
    }

    void SetRoomLayoutPositions()
    {
        RoomLayout.ComputerSlot.Position= computerSlot.transform.localPosition;
        RoomLayout.CameraSlot.Position = cameraSlot.transform.localPosition;
        RoomLayout.MicrophoeSlot.Position = microphoneSlot.transform.localPosition;
        RoomLayout.GreenScreenSlot.Position = greenScreenSlot.transform.localPosition;

        for (int i = 0; i < RoomLayout.WallLayoutSlots.Count; i++)
        {
            RoomLayout.WallLayoutSlots[i]=new RoomLayoutSerializedSlot( RoomLayout.WallLayoutSlots[i].ItemName, RoomLayout.WallLayoutSlots[i].SlotID,
                wallSlots[ RoomLayout.WallLayoutSlots[i].SlotID].Image.transform.localPosition,Vector3.one, 0);

        }
      
        for (int i = 0; i < RoomLayout.FloorLayoutSlots.Count; i++)
        {
            RoomLayout.FloorLayoutSlots[i]=new RoomLayoutSerializedSlot( RoomLayout.FloorLayoutSlots[i].ItemName, RoomLayout.FloorLayoutSlots[i].SlotID,
                floorSlots[ RoomLayout.FloorLayoutSlots[i].SlotID].Image.transform.localPosition,Vector3.one, 0);

        }
        for (int i = 0; i < RoomLayout.ObjectsLayoutSlots.Count; i++)
        {
            RoomLayout.ObjectsLayoutSlots[i]=new RoomLayoutSerializedSlot( RoomLayout.ObjectsLayoutSlots[i].ItemName, RoomLayout.ObjectsLayoutSlots[i].SlotID,
                roomObjectSlots[ RoomLayout.WallLayoutSlots[i].SlotID].Image.transform.localPosition,Vector3.one, 0);

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
                    computerSlot.transform.localPosition = RoomLayout.ComputerSlot.Position;
                    
                    break;
                case VideoQualityItemType.Camera:
                    cameraSlot.gameObject.SetActive(true);
                    cameraSlot.sprite = vqItem.itemSprite;
                    cameraSlot.transform.localPosition = RoomLayout.ComputerSlot.Position;
                    break;
                case VideoQualityItemType.Microphone:
                    microphoneSlot.gameObject.SetActive(true);
                    microphoneSlot.sprite = vqItem.itemSprite;
                    microphoneSlot.transform.localPosition = RoomLayout.MicrophoeSlot.Position;
                  
                    break;
                case VideoQualityItemType.GreenScreen:
                    greenScreenSlot.gameObject.SetActive(true);
                    greenScreenSlot.sprite = vqItem.itemSprite;
                    greenScreenSlot.transform.localPosition = RoomLayout.GreenScreenSlot.Position;
                
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
        slot.roomLayoutSerializedSlot.ItemName = wallOrnament.name;
        slot.Image.gameObject.SetActive(true);
        var trans = slot.Image.gameObject.transform;
        RoomLayout.WallLayoutSlots[slot.roomLayoutSerializedSlot.SlotID] = new RoomLayoutSerializedSlot(wallOrnament.name, slot.roomLayoutSerializedSlot.SlotID,transform.position,trans.lossyScale,0);

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
       slot.roomLayoutSerializedSlot.ItemName = floorOrnament.name;
       slot.Image.gameObject.SetActive(true);
       var trans = slot.Image.gameObject.transform;
       RoomLayout.FloorLayoutSlots[slot.roomLayoutSerializedSlot.SlotID] =
           new RoomLayoutSerializedSlot(floorOrnament.name, slot.roomLayoutSerializedSlot.SlotID, trans.position,
               trans.lossyScale,0);

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
        slot.roomLayoutSerializedSlot.ItemName = roomObject.name;
        slot.Image.gameObject.SetActive(true);
        var trans = slot.Image.gameObject.transform;

        RoomLayout.ObjectsLayoutSlots[slot.roomLayoutSerializedSlot.SlotID] = new RoomLayoutSerializedSlot(roomObject.name,slot.roomLayoutSerializedSlot.SlotID,trans.position,trans.lossyScale,0);
    }

    public  void OnSaveRoomLayout()
    {

        SetRoomLayoutPositions();
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
            RoomLayoutSerializedSlot serializedSlot = new RoomLayoutSerializedSlot();
            serializedSlot.ItemName = wallSlots[i].WallOrnamentType.ToString();
            serializedSlot.SlotID =  i;
            var imgTransform = wallSlots[i].Image.transform;
            serializedSlot.Position = imgTransform.localPosition;
            serializedSlot.Scale = imgTransform.lossyScale;
            DefaultRoomLayout.WallLayoutSlots.Add(serializedSlot);
            
        }
        for (int i = 0; i < floorSlots.Count; i++)
        {
            RoomLayoutSerializedSlot serializedSlot = new RoomLayoutSerializedSlot();
            serializedSlot.ItemName = floorSlots[i].FloorOrnamentType.ToString();
            serializedSlot.SlotID =  i;
            var imgTransform = floorSlots[i].Image.transform;
            serializedSlot.Position = imgTransform.localPosition;
            serializedSlot.Scale = imgTransform.lossyScale;
            DefaultRoomLayout.FloorLayoutSlots.Add(serializedSlot);
        }
        for (int i = 0; i < roomObjectSlots.Count; i++)
        {
            RoomLayoutSerializedSlot serializedSlot = new RoomLayoutSerializedSlot();
            serializedSlot.ItemName = roomObjectSlots[i].RoomObjectType.ToString();
            serializedSlot.SlotID =  i;
            var imgTransform = roomObjectSlots[i].Image.transform;
            serializedSlot.Position = imgTransform.localPosition;
            serializedSlot.Scale = imgTransform.lossyScale;
            DefaultRoomLayout.ObjectsLayoutSlots.Add(serializedSlot);
        }

        RoomLayoutSerializedSlot pcSlot = new RoomLayoutSerializedSlot();
        pcSlot.Position = computerSlot.transform.localPosition;
        pcSlot.Scale = cameraSlot.transform.lossyScale;
        pcSlot.RotaionId = 0;
        DefaultRoomLayout.ComputerSlot = pcSlot;

        RoomLayoutSerializedSlot camSlot = new RoomLayoutSerializedSlot();

        camSlot.Position = cameraSlot.transform.localPosition;
        camSlot.Scale = cameraSlot.transform.lossyScale;
        camSlot.RotaionId = 0;
        DefaultRoomLayout.CameraSlot = camSlot;

        RoomLayoutSerializedSlot micSlot = new RoomLayoutSerializedSlot();

        micSlot.Position = microphoneSlot.transform.localPosition;
        micSlot.Scale = microphoneSlot.transform.lossyScale;
        micSlot.RotaionId = 0;
        DefaultRoomLayout.MicrophoeSlot = micSlot;
        DefaultRoomLayout.GreenScreenSlot= new RoomLayoutSerializedSlot(" 0",0,greenScreenSlot.transform.localPosition,
            greenScreenSlot.transform.lossyScale,0);
        
    }
}

[System.Serializable]
public class WallSlotData
{
    public SpriteRenderer Image;
    public WallOrnamentType WallOrnamentType;
    public bool Empty=true;
    public RoomLayoutSerializedSlot roomLayoutSerializedSlot;


}
[System.Serializable]
public class FloorSlotData
{
    public SpriteRenderer Image;
    public FloorOrnamentType FloorOrnamentType;
    public bool Empty=true;
    public RoomLayoutSerializedSlot roomLayoutSerializedSlot;



}
[System.Serializable]
public class ObjectSlotData
{
    public SpriteRenderer Image;
    public RoomObjectType RoomObjectType;
    public bool Empty=true;
    public RoomLayoutSerializedSlot roomLayoutSerializedSlot;

}




