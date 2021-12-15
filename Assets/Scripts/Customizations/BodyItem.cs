using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "BodyItem", menuName = "Customizations/BodyItem")]
    public class BodyItem : ThemeCustomizationItem
    {
        public GenderItemType GenderItemType;
        public int BodyIndex;
        public Vector3 headPosition;
        public Vector3 torsoPosition;
        public Vector3 legsPosition;
        public Vector3 feetPosition;
    }

    public enum GenderItemType
    {
        Male,Female
    }
    
}