using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "RealEstate", menuName = "Customizations/RealEstateItem")]
    public class RealEstateCustomizationItem : ScriptableObject
    {
        public string itemName;
        public Sprite itemSprite;
        public Rareness rareness=Rareness.Common;
        public uint roomSlots;
        public uint garageSlots;
        public bool Owned=false;

        public PriceType PriceType;
        public uint HCPrice;
        public uint SCPrice;
    }
}