using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "WallOrnament", menuName = "Customizations/WallOrnament")]
    public class WallOrnament : ThemeCustomizationItem
    {
        public Sprite wallOrnamentSprite;
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