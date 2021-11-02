using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "FeetItem", menuName = "Customizations/FeetItem", order = 4)]
    public class FeetItem : CustomizationItem
    {
        public FeetItemType FeetItemType;
        public Sprite feetSprite;

    }
    public enum FeetItemType
    {
        Sneakers,
        Sandals,
        Boots,
        Ankle_Bracelets
    }
}


