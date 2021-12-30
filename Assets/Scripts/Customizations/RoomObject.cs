using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Customizations
{
    [CreateAssetMenu(fileName = "RoomObject", menuName = "Customizations/RoomObject")]
    public class RoomObject : ThemeCustomizationItem
    {
        public AssetReference itemPrefab;
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