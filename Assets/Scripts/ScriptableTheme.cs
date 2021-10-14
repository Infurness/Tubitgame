using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/ThemesInfo", order = 1)]
public class ScriptableTheme : ScriptableObject
{
    public List<ThemeData> themesData;

    public void UpdateAvailableThemes ()
    {
        if (themesData.Count != Enum.GetValues (typeof (ThemeType)).Length)
        {
            foreach (int i in Enum.GetValues (typeof (ThemeType)))
            {
                ThemeType themeType = (ThemeType)i;
                if (!CheckIfThemeExistsInList (themeType))
                {
                    ThemeData theme = new ThemeData ();
                    theme.themeType = themeType;
                    themesData.Add (theme);
                }
            }
        }
    }
    bool CheckIfThemeExistsInList (ThemeType _themeType)
    {
        return themesData.Exists (x => x.themeType == _themeType);
    }
}
[Serializable]
public struct ThemeData
{
    public ThemeType themeType;
    public AnimationCurve themeAlgorithm;
}