using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Customizations
{
    [CreateAssetMenu(fileName = "WallOrnament", menuName = "Customizations/WallOrnament")]
    public class WallOrnament : ThemeCustomizationItem
    {
        public ItemSlotType ConditionalSlot;
    }


}