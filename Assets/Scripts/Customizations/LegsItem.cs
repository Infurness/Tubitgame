﻿using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "LegsItem", menuName = "Customizations/LegsItem", order = 3)]
    public class LegsItem : CustomizationItem
    {
        public LegsItemType LegsType;
        public Sprite legSprite;
    
    }
    public enum LegsItemType
    {
        Pants,
        Shorts,
        Bathing_Suits
    }
    
}

