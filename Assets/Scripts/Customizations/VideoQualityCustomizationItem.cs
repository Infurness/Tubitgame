using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "VideoQualityItem", menuName = "Customizations/VideoQualityItem")]

    public class VideoQualityCustomizationItem : ScriptableObject
    {
        public Sprite itemSprite;
        public VideoQualityItemType videoQualityItemType;
        [Range(0.1f,0.3f)]
        public float videoQualityBonus;
        public Rareness rareness=Rareness.Common;
        [TextArea]
        public string descriptionText;

        [TextArea] public string newStatsText;
        
        public PriceType PriceType;
        public short HCPrice;
        public short SCPrice;
    }

    public enum VideoQualityItemType
    {
        Computer,
        Camera,
        Microphone,
        GreenScreen
    }
}