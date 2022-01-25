using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "FeetItem", menuName = "Customizations/FeetItem", order = 4)]
    public class FeetItem : ThemeCustomizationItem
    {
        public Sprite IdleFeet;
        public FeetItemType FeetItemType;
        public GenderItemType GenderItemType;
        public Sprite[] FeetVariants;

    }
    public enum FeetItemType
    {
        Sneakers,
        Sandals,
        Boots,
        Bracelets
    }
}


