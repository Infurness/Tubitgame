
using System;
using System.Linq;
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
        public ItemSlotType SlotType;

        public void OnEnable()
        {

            string themeEffectText="";
            foreach (var effect in affectedTheme)
            {
                var factor = effect.themePopularityFactor * 100;
                var themeText = string
                    .Concat(effect.ThemeType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString()))
                    .TrimStart(' ');
                themeEffectText += "\n" +   themeText + " : " + (int) factor + "%";
            }

            descriptionText = themeEffectText.ToUpper();
        }
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