using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Customizations
{
    [CreateAssetMenu(fileName = "FloorOrnament", menuName = "Customizations/FloorOrnament", order = 0)]
    public class FloorOrnament : ThemeCustomizationItem
    {
        public ItemSlotType ConditionalSlot;

    }


}