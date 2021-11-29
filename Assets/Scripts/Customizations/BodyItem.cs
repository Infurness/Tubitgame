using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "BodyItem", menuName = "Customizations/BodyItem")]
    public class BodyItem : ThemeCustomizationItem
    {
        public GenderItemType GenderItemType;
        public int BodyIndex;
    }

    public enum GenderItemType
    {
        Male,Female
    }
    
}