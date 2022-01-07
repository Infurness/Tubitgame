
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Customizations
{
    public class ThemeCustomizationItem : ScriptableObject
    {
        public Sprite sprite;
        public AssetReference itemPrefab;
        public CustomizationThemeEffect[] affectedTheme;
        public Rareness rareness = Rareness.Common;
        [TextArea] public string descriptionText;

        [TextArea] public string newStatsText;
        public bool Owned=false;

        public PriceType PriceType;
        public uint HCPrice;
        public uint SCPrice;
        
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