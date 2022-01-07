using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlayFab.Json;
using UnityEngine;

[System.Serializable]
public class RoomLayout
{
    public List<RoomLayoutSerializedSlot> roomSlots;


    public RoomLayout(RoomLayout roomLayout)
    {

        roomSlots = new List<RoomLayoutSerializedSlot>(roomLayout.roomSlots);
    }

    public RoomLayout()
    {
        roomSlots = new List<RoomLayoutSerializedSlot>();

    }
}
[System.Serializable]
public struct RoomLayoutSerializedSlot
{
    public RoomLayoutSerializedSlot(string itemName, int slotID,Vector3 position,Vector3 scale,int rotaionId)
    {
        ItemName = itemName;
        Scale = scale;
        Position = position;
        RotaionId = rotaionId;
        ParentType = RoomParentType.Floor;
        customizationType = CustomizationType.Theme;
    } 
    public string ItemName;
    public Vector3 Position;
    public Vector3 Scale;
    public int RotaionId;
    public RoomParentType ParentType;
    public CustomizationType customizationType;
}


public enum RoomParentType
{
    Floor,
    RightWall,
    LeftWall,
    Desk,
    Chair,
    
}

public enum CustomizationType
{
    Theme,
    VideoQuality
}