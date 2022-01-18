using UnityEngine;

namespace Customizations
{
    public enum RealEstateHouse { BasicHouse, NiceApartment, HugeHouse};
    [CreateAssetMenu(fileName = "RealEstate", menuName = "Customizations/RealEstateItem")]
    public class RealEstateCustomizationItem : ScriptableObject
    {
        public RealEstateHouse realEstateHouse;
        public string itemName;
        public Sprite streetViewSprite;
        public Sprite houseCloseSpite;
        public Vector3 garagePosition;
        public Rareness rareness=Rareness.Common;
        public uint roomSlots;
        public uint garageSlots;
        public bool Owned=false;

        public PriceType PriceType;
        public uint HCPrice;
        public uint SCPrice;
    }
}