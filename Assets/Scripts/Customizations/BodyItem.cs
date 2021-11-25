using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "BodyItem", menuName = "Customizations/BodyItem", order = 0)]
    public class BodyItem : ThemeCustomizationItem
    {
        public GenderItemType GenderItemType;

    }

    public enum GenderItemType
    {
        Male,Female,Unisex
    }
}