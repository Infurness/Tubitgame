using UnityEngine;

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
        Statues,
        CatTree
    }
}