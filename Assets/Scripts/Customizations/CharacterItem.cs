
using UnityEngine;

public class CharacterItem : ScriptableObject
{
    public ThemeType[] affectedTheme;
    [Range(0.1f,1)]
    public float themePopularityFactor;
    public Rareness rareness;
}
