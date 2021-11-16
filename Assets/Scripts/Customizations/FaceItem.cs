using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "FaceItem", menuName = "Customizations/FaceItem", order = 1)]
    public class FaceItem : ThemeCustomizationItem
    {
        public FaceItemType FaceItemType;
        public Sprite faceSprite;
        
    }

    public enum FaceItemType
    {
        Glasses,

        Piercings,

        Bold_Makeup_Applications,

        Masks
    }
}