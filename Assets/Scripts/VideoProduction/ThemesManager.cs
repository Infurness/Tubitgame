using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemesManager : MonoBehaviour
{
    [SerializeField] private List<Theme> availableThemes = new List<Theme>();
    // Start is called before the first frame update
    void Awake()
    {
        UpdateAvailableThemes ();
    }

    public ThemeType[] GetThemes ()
    {
        return (ThemeType[])Enum.GetValues (typeof (ThemeType));
    }
    void UpdateAvailableThemes ()
    {
        availableThemes.Clear ();
        foreach (int i in Enum.GetValues (typeof (ThemeType)))
        {
            Theme theme = new Theme ();
            theme.themeType = (ThemeType)i;
            availableThemes.Add (theme);
        }
    }

    public float GetThemePopularity (ThemeType _themeType)
    {
        foreach(Theme theme in availableThemes)
        {
            if(theme.themeType == _themeType)
            {
                return theme.popularity;
            }
        }
        Debug.LogError ($"No theme: {Enum.GetName (_themeType.GetType (), _themeType)}, is available");
        return 0;
    }
}
