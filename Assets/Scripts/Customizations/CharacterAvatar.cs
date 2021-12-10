using System.Collections.Generic;

namespace Customizations
{
    [System.Serializable]
    public class CharacterAvatar
    {
        
        public BodyItem bodyItem;
        public HeadItem headItem;
        public HairItem hairItem;
        public TorsoItem torsoItem;
        public LegsItem legsItem;
        public FeetItem feetItem;

        public List<ThemeCustomizationItem> GetThemesEffectItems()
        {
            return new List<ThemeCustomizationItem>() {bodyItem,headItem, torsoItem, hairItem, legsItem, feetItem};
        }

        public CharacterAvatar(CharacterAvatar characterAvatar)
        {
            bodyItem = characterAvatar.bodyItem;
            headItem = characterAvatar.headItem;
            hairItem = characterAvatar.hairItem;
            torsoItem = characterAvatar.torsoItem;
            legsItem = characterAvatar.legsItem;
            feetItem = characterAvatar.feetItem;
        }
    }
}