using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemesManager : MonoBehaviour
{
    [SerializeField] private List<Theme> availableThemes = new List<Theme>();
    [SerializeField] private ScriptableTheme themesData;
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

    //public float GetThemePopularity (ThemeType _themeType, int _dayHour)
    //{
    //    foreach(Theme theme in availableThemes)
    //    {
    //        if(theme.themeType == _themeType)
    //        {
    //            return ThemePopularityBasedOnTime (theme, _dayHour);
    //        }
    //    }
    //    Debug.LogError ($"No theme: {Enum.GetName (_themeType.GetType (), _themeType)}, is available");
    //    return 0;
    //}

    public float GetThemePopularity (ThemeType _themeType, int _dayHour)
    {
        foreach (ThemeData theme in themesData.themesData)
        {
            if (theme.themeType == _themeType)
            {
                return ThemePopularityBasedOnTime (theme, _dayHour);
            }
        }
        Debug.LogError ($"No theme: {Enum.GetName (_themeType.GetType (), _themeType)}, is available");
        return 0;
    }

    //private float ThemePopularityBasedOnTime (Theme _theme, int _dayHour) //Dummy: this may be changed in the future
    //{
    //    int hourIndex = _dayHour; 
    //    if (_theme.popularityEachHour == null || _theme.popularityEachHour.Length == 0)
    //    {
    //        Debug.LogError ($"No popularity set for theme {Enum.GetName (_theme.themeType.GetType (), _theme.themeType)}");
    //        return 0;
    //    }
              
    //    if (_dayHour > _theme.popularityEachHour.Length)
    //    {
    //        hourIndex = (_dayHour % _theme.popularityEachHour.Length) - 1;
    //        if (hourIndex < 0)
    //            hourIndex = 0;
    //    }

    //    float themPopularity = _theme.popularityEachHour[hourIndex];
    //    Debug.Log ($"Theme popularity is: {themPopularity}");
    //    return themPopularity;
    //}
    private float ThemePopularityBasedOnTime (ThemeData _themeData, int _dayHour) //Dummy: this may be changed in the future
    {
        
        if (_themeData.themeAlgorithm.length==0)
        {
            Debug.LogError ($"No popularity set for theme {Enum.GetName (_themeData.themeType.GetType (), _themeData.themeType)}");
            return 0;
        }

        AnimationCurve algorithm = _themeData.themeAlgorithm;
        float hourInGraph =_dayHour * algorithm[algorithm.length-1].time / 24; //Since last number in time would be the hour 24, hourInGraph gets the value for the expected hour in the graph, no matter if the max time value is 1 or 37

        float themPopularity = algorithm.Evaluate(hourInGraph);
        Debug.Log ($"Theme popularity is: {themPopularity}");
        return themPopularity;
    }
}
