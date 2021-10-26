
using UnityEngine;

namespace Customizations
{
    public class CharacterItem : ScriptableObject
    {
        public string itemName;
        public Sprite logoSprite;
        public ThemeType[] affectedTheme;
        [Range(0.1f, 1)] public float themePopularityFactor=0.1f;
        public Rareness rareness;
        
        
        
    }
    
    
}