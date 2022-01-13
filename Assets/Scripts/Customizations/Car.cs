using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "Car", menuName = "Customizations/Car")]
    public class Car : ScriptableObject
    {
        public Sprite carSprite;
        public int price;
        public PriceType priceType;
    }

  
}