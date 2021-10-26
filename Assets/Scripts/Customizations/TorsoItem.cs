using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "TorsoItem", menuName = "Customizations/TorsoItem", order = 2)]
    public class TorsoItem : CharacterItem
    {
        public TorsoItemType TorsoItemType;

        public Sprite torsoSprite;
    }

    public enum TorsoItemType
    {
        Tshirts,
        Jackets
    }
    
}
