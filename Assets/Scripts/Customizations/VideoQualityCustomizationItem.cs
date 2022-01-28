using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Customizations
{
    [CreateAssetMenu(fileName = "VideoQualityItem", menuName = "Customizations/VideoQualityItem")]

    public class VideoQualityCustomizationItem : ScriptableObject
    {
        public Sprite itemSprite;
        public AssetReference itemPrefab;
        public VideoQualityItemType videoQualityItemType;
        [Range(0.0f,0.3f)]
        public float videoQualityBonus;
        public Rareness rareness=Rareness.Common;
        [TextArea]
        public string descriptionText;

        [TextArea] public string newStatsText;
        public bool Owned=false;
        public PriceType PriceType;
        public uint HCPrice;
        public uint SCPrice;
        public ItemSlotType SlotType;
        
        public void OnEnable()
        {

            var percent = videoQualityBonus * 100;
            descriptionText ="Video Quality Bonus : %"+ (int)(percent);
        }
    }

    public enum VideoQualityItemType
    {
        Computer,
        Camera,
        Microphone,
        GreenScreen
    }
}