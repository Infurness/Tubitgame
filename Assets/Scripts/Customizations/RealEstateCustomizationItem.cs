﻿using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "RealEstate", menuName = "Customizations/RealEstateItem")]
    public class RealEstateCustomizationItem : ScriptableObject
    {
        public string itemName;
        public Sprite streetViewSprite;
        public Sprite houseCloseSpite;
        public Rareness rareness=Rareness.Common;
        public uint roomSlots;
        public uint garageSlots;
        public bool Owned=false;

        public PriceType PriceType;
        public uint HCPrice;
        public uint SCPrice;
    }
}