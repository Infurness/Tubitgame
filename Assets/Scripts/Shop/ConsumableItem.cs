using Customizations;
using UnityEngine;


    [CreateAssetMenu(fileName = "Consumables", menuName = "Consumables", order = 0)]
    public class ConsumableItem : ScriptableObject
    {
        public Sprite sprite;
        public string itemName;
        public PriceType priceType;
        public int price;
        public int amount;
        public ConsumableType type;
        public string specialID;
        public enum ConsumableType
        {
            SoftCurrency,
            Energy
        }
    }
