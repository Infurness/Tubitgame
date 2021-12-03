using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "RealEstate", menuName = "Customizations/RealEstateItem")]
    public class RealEstateCustomizationItem : ScriptableObject
    {
        public Sprite itemSprite;
        public Rareness rareness=Rareness.Common;
        [TextArea]
        public string descriptionText;
        public bool Owned=false;

        public PriceType PriceType;
        public uint HCPrice;
        public uint SCPrice;
    }
}