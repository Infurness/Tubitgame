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
    Pizza,
    Package,
    Shelf,
    Books,
    Painting,
    FlowerPot,
    Bin,
    Tv,
    Ball,
    Mic,
    Computer,
    Camera,
    GreenScreen,
    Lamps,
    CatTree,

}