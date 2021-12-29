using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Customizations
{

    [CreateAssetMenu(fileName = "HeadItem", menuName = "Customizations/HeadItem", order = 0)]
    public class HeadItem : ThemeCustomizationItem
    {
        public HeadItemType HeadItemType;
        public GenderItemType GenderItemType;
        public Sprite[] HeadVariants;
    }

    public enum HeadItemType
    {
      
    }
}