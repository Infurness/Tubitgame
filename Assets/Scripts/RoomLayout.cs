using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlayFab.Json;
using UnityEngine;

[System.Serializable]
public class RoomLayout
{
    public List<RoomLayoutSerializedSlot> WallLayoutSlots;


    public List<RoomLayoutSerializedSlot> FloorLayoutSlots;

    public List<RoomLayoutSerializedSlot> ObjectsLayoutSlots;

    public RoomLayoutSerializedSlot ComputerSlot;
    public RoomLayoutSerializedSlot CameraSlot;
    public RoomLayoutSerializedSlot MicrophoeSlot;
    public RoomLayoutSerializedSlot GreenScreenSlot;



    public RoomLayout(RoomLayout roomLayout)
 {
     WallLayoutSlots = new List<RoomLayoutSerializedSlot>(roomLayout.WallLayoutSlots);
     FloorLayoutSlots = new List<RoomLayoutSerializedSlot>(roomLayout.FloorLayoutSlots);
     ObjectsLayoutSlots = new List<RoomLayoutSerializedSlot>(roomLayout.ObjectsLayoutSlots);
     ComputerSlot = roomLayout.ComputerSlot;
     CameraSlot = roomLayout.CameraSlot;
     MicrophoeSlot = roomLayout.MicrophoeSlot;
     GreenScreenSlot = roomLayout.GreenScreenSlot;

 }

    public RoomLayout()
    {
        WallLayoutSlots = new List<RoomLayoutSerializedSlot>();
        FloorLayoutSlots = new List<RoomLayoutSerializedSlot>();
        ObjectsLayoutSlots = new List<RoomLayoutSerializedSlot>();
        ComputerSlot = new RoomLayoutSerializedSlot();
        CameraSlot =new RoomLayoutSerializedSlot();
        MicrophoeSlot =new RoomLayoutSerializedSlot();
        GreenScreenSlot =new RoomLayoutSerializedSlot();

    }
}
[System.Serializable]
public struct RoomLayoutSerializedSlot
{
    public RoomLayoutSerializedSlot(string itemName, int slotID,Vector3 position,Vector3 scale,int rotaionId)
    {
        ItemName = itemName;
        SlotID = slotID;
        Scale = scale;
        Position = position;
        RotaionId = rotaionId;
    } 
    public string ItemName;
    public int SlotID; 
    public Vector3 Position;
    public Vector3 Scale;
    public int RotaionId;


}