using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemesManager : MonoBehaviour
{

    [SerializeField] private ScriptableTheme themesData;
    // Start is called before the first frame update

    public ThemeType[] GetThemes ()
    {
        return (ThemeType[])Enum.GetValues (typeof (ThemeType));
    }

    public float GetThemePopularity (ThemeType _themeType, DateTime _dayHour)
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
    public AnimationCurve GetThemeAlgorithm (ThemeData _themeData, DateTime date)
    {
        AnimationCurve themeCurve;
        if (date.DayOfWeek.GetHashCode () < 5)//BeforeWeekend
        {
            switch (date.Hour)
            {
                case int h when h < 6:
                    themeCurve = _themeData.themeAlgorithm0to6WeekDays;
                    break;
                case int h when h < 12:
                    themeCurve = _themeData.themeAlgorithm6to12WeekDays;
                    break;
                case int h when h < 18:
                    themeCurve = _themeData.themeAlgorithm12to18WeekDays;
                    break;
                default:
                    themeCurve = _themeData.themeAlgorithm18to24WeekDays;
                    break;
            }
        }
        else //Weekend
        {
            switch (date.Hour)
            {
                case int h when h < 6:
                    themeCurve = _themeData.themeAlgorithm0to6WeekEnd;
                    break;
                case int h when h < 12:
                    themeCurve = _themeData.themeAlgorithm6to12WeekEnd;
                    break;
                case int h when h < 18:
                    themeCurve = _themeData.themeAlgorithm12to18WeekEnd;
                    break;
                default:
                    themeCurve = _themeData.themeAlgorithm18to24WeekEnd;
                    break;
            }
        }
        return themeCurve;
    }
    private float ThemePopularityBasedOnTime (ThemeData _themeData, DateTime date) //Dummy: this may be changed in the future
    {
        AnimationCurve themeCurve = GetThemeAlgorithm(_themeData, date);

        if (themeCurve.length==0)
        {
            Debug.LogError ($"No popularity set for theme {Enum.GetName (_themeData.themeType.GetType (), _themeData.themeType)}");
            return 0;
        }

        float hourInGraph =date.Hour * themeCurve[themeCurve.length-1].time / 24; //Since last number in time would be the hour 24, hourInGraph gets the value for the expected hour in the graph, no matter if the max time value is 1 or 37

        float themPopularity = themeCurve.Evaluate(hourInGraph);
        Debug.Log ($"Theme popularity is: {themPopularity}");
        return themPopularity;
    }
    //public AnimationCurve[] GetThemesPopuarityData ()
    //{
    //    List<AnimationCurve> curves = new List<AnimationCurve>();
    //    foreach(ThemeData data in themesData.themesData)
    //    {
    //        curves.Add (data.themeAlgorithm);
    //    }
    //    return curves.ToArray ();
    //}
    public ThemeData[] GetThemesData ()
    {
        return themesData.themesData.ToArray ();
    }
    public void UpdateThemesData (ScriptableTheme newThemesData)
    {
        themesData.themesData = newThemesData.themesData;
    }
}
