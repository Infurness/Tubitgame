﻿using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "FaceItem", menuName = "Customizations/FaceItem", order = 1)]
    public class HairItem : ThemeCustomizationItem
    {
        public HairItemType hairItemType;
        public GenderItemType GenderItemType;
        public Sprite[] HairVariants;
    }

    public enum HairItemType
    {
        Black,

        Blonde,

        Red,
        
    }
}