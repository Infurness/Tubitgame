using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Customizations
{
    [CreateAssetMenu(fileName = "RoomObject", menuName = "Customizations/RoomObject")]
    public class RoomObject : ThemeCustomizationItem
    {
        public ItemSlotType ConditionalSlot;
    }

  
}