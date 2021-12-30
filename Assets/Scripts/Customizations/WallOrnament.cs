using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Customizations
{
    [CreateAssetMenu(fileName = "WallOrnament", menuName = "Customizations/WallOrnament")]
    public class WallOrnament : ThemeCustomizationItem
    {
        public AssetReference itemPrefab;
        public WallOrnamentType WallOrnamentType;
    }

 public   enum WallOrnamentType
    {
        Paintings,
        TV,
        Windows,

        Blackboard,

        Bookshelf
    }
}