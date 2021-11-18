﻿using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "FloorOrnament", menuName = "Customizations/FloorOrnament", order = 0)]
    public class FloorOrnament : ThemeCustomizationItem
    {
        public Sprite floorOrnamentSprite;
        public FloorOrnamentType floorOrnamentType;
    }

    public enum FloorOrnamentType
    {
        Table,
        Chair,
        Sofa,
        Statues,
        Bed,
        Carpet,
        Bin,
        FlowerPot,
        Box0,
        Box1,
        Box2,
        Box3
    }
}