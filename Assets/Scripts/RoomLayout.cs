using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlayFab.Json;
using UnityEngine;

[System.Serializable]
public class RoomLayout
{
    public List<RoomLayoutThemeSlot> WallLayoutSlots;


    public List<RoomLayoutThemeSlot> FloorLayoutSlots;

    public List<RoomLayoutThemeSlot> ObjectsLayoutSlots;


 public RoomLayout(RoomLayout roomLayout)
 {
     WallLayoutSlots = new List<RoomLayoutThemeSlot>(roomLayout.WallLayoutSlots);
     FloorLayoutSlots = new List<RoomLayoutThemeSlot>(roomLayout.FloorLayoutSlots);
     ObjectsLayoutSlots = new List<RoomLayoutThemeSlot>(roomLayout.ObjectsLayoutSlots);
 }

 public RoomLayout()
 {
     WallLayoutSlots = new List<RoomLayoutThemeSlot>();


     FloorLayoutSlots = new List<RoomLayoutThemeSlot>();

     ObjectsLayoutSlots = new List<RoomLayoutThemeSlot>();
 }
}
[System.Serializable]
public struct RoomLayoutThemeSlot
{
    public RoomLayoutThemeSlot(string itemName, int slotID)
    {
        ItemName = itemName;
        SlotID = slotID;
    }

    public string ItemName;
   public int SlotID;

   
}