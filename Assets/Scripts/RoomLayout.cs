using System.Collections.Generic;

[System.Serializable]
public class RoomLayout
{
   public List<RoomLayoutThemeSlot> WallLayoutSlots=new List<RoomLayoutThemeSlot>();
   public List<RoomLayoutThemeSlot> FloorLayoutSlots=new List<RoomLayoutThemeSlot>();
   public List<RoomLayoutThemeSlot> ObjectsLayoutSlots=new List<RoomLayoutThemeSlot>();

}
[System.Serializable]
public class RoomLayoutThemeSlot
{
   public string ItemName;
   public int SlotID;
}