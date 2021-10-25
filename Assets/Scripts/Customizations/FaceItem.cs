using UnityEngine;

namespace Customizations
{
    [CreateAssetMenu(fileName = "FaceItem", menuName = "Customizations/FaceItem", order = 1)]
    public class FaceItem : ScriptableObject
    {
        public Sprite faceSprite;
    }
}