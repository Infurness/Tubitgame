using UnityEngine;

    public class RoomObjectData : MonoBehaviour
    {
        public ItemSlotType slotItemType;
        public CustomizationType customizationType;
        public string assetName;
    }
public enum CustomizationType
{
    ThemeItem,
    VideoQualityItem
}

public enum ItemSlotType
{
    Desk,
    Chair,
    Bed,
    Sofa,
    Carpet,
    FloorSlot1,
    FloorSlot2,
    FloorSlot3,
    PlayStation,
    WallSlot1,
    WallSlot2,
    WallSlot3,
    Books,
    Curtains,
    Windows,
    Mic,
    Computer,
    Camera,
    GreenScreen,
    Lamps,
    DesktopItem,
    Pillow,
    ClothHanger,
    none

}