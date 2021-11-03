using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "RoomObject", menuName = "Customizations/RoomObject")]
    public class RoomObject : ThemeCustomizationItem
    {
        public Sprite roomObjectSprite;
        public RoomObjectType roomObjectType;
    }

    public enum RoomObjectType
    {
        Ball,
        VideoGameConsole,
        FlowerVase,
        ClothingRack
    }
}