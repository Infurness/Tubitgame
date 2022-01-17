using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "Car", menuName = "Customizations/Car")]
    public class Car : ScriptableObject
    {
        public Sprite carSprite;
        public bool Owned=false;
        public Rareness rareness;
        public uint HCPrice;
        public uint SCPrice;
        public PriceType priceType;
    }

  
}