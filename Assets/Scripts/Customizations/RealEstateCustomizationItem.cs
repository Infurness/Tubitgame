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
        
        public PriceType PriceType;
        public short HCPrice;
        public short SCPrice;
    }
}