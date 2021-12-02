
using TMPro;
using UnityEngine;

namespace Customizations
{
    public class ThemeCustomizationItem : ScriptableObject
    {
        public Sprite sprite;
        public CustomizationThemeEffect[] affectedTheme;
        public Rareness rareness = Rareness.Common;
        [TextArea] public string descriptionText;

        [TextArea] public string newStatsText;
        public PriceType PriceType;
        public short HCPrice;
        public short SCPrice;
    }




    [System.Serializable]
    public class CustomizationThemeEffect
    {
        public ThemeType ThemeType;
        [Range(0f, 1)] public float themePopularityFactor = 0.1f;

    }

    public enum PriceType
    {
        Free,
        SC,
        HC,
        Exchangeable
    }
}