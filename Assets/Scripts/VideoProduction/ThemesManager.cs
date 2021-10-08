using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemesManager : MonoBehaviour
{
    [SerializeField] private List<Theme> availableThemes = new List<Theme>();
    // Start is called before the first frame update
    void Start()
    {
        UpdateAvailableThemes ();
    }

    public ThemeType[] GetThemes ()
    {
        return (ThemeType[])Enum.GetValues (typeof (ThemeType));
    }

    public void UpdateAvailableThemes ()
    {
        if(availableThemes.Count != Enum.GetValues (typeof (ThemeType)).Length)
        {
            foreach (int i in Enum.GetValues (typeof (ThemeType)))
            {
                ThemeType themeType = (ThemeType)i;
                if (!CheckIfThemeExistsInList(themeType))
                {
                    Theme theme = new Theme ();
                    theme.themeType = themeType;
                    availableThemes.Add (theme);
                }               
            }
        }
    }
    bool CheckIfThemeExistsInList (ThemeType _themeType)
    {
        return availableThemes.Exists (x => x.themeType == _themeType);
    }

    public float GetThemePopularity (ThemeType _themeType, int _dayHour)
    {
        foreach(Theme theme in availableThemes)
        {
            if(theme.themeType == _themeType)
            {
                return ThemePopularityBasedOnTime (theme, _dayHour);
            }
        }
        Debug.LogError ($"No theme: {Enum.GetName (_themeType.GetType (), _themeType)}, is available");
        return 0;
    }

    private float ThemePopularityBasedOnTime (Theme _theme, int _dayHour) //Dummy: this may be changed in the future
    {
        int hourIndex = _dayHour; 
        if (_theme.popularityEachHour == null || _theme.popularityEachHour.Length == 0)
        {
            Debug.LogError ($"No popularity set for theme {Enum.GetName (_theme.themeType.GetType (), _theme.themeType)}");
            return 0;
        }
              
        if (_dayHour > _theme.popularityEachHour.Length)
        {
            hourIndex = (_dayHour % _theme.popularityEachHour.Length) - 1;
        }

        float themPopularity = _theme.popularityEachHour[hourIndex];
        Debug.Log ($"Theme popularity is: {themPopularity}");
        return themPopularity;
    }
}
