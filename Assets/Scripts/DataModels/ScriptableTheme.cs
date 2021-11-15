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
                    ThemeData theme;
                    theme.themeType = themeType;
                    
                    theme.themeAlgorithm0to6WeekDays = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));
                    theme.themeAlgorithm6to12WeekDays = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));
                    theme.themeAlgorithm12to18WeekDays = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));
                    theme.themeAlgorithm18to24WeekDays = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));
                                                        
                    theme.themeAlgorithm0to6WeekEnd = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));
                    theme.themeAlgorithm6to12WeekEnd = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));
                    theme.themeAlgorithm12to18WeekEnd = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));
                    theme.themeAlgorithm18to24WeekEnd = AnimationCurve.EaseInOut (0, UnityEngine.Random.Range (0f, 2f), 1, UnityEngine.Random.Range (0f, 2f));

                    theme.graphLineColor = new Color (UnityEngine.Random.Range (0f, 1f), UnityEngine.Random.Range (0f, 1f), UnityEngine.Random.Range (0f, 1f), 1);

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
public enum ThemeType
{
    FilmsAndAnimation,
    AutosAndVehicles,
    Music,
    PetsAndAnimals,
    Sports,
    TravelAndEvents,
    Gaming,
    PeopleAndBlogs,
    Comedy,
    Entertainment,
    NewsAndPolitics,
    HowToAndStyle,
    Education,
    ScienceAndTechnology
}
[Serializable]
public struct ThemeData
{
    public ThemeType themeType;

    public AnimationCurve themeAlgorithm0to6WeekDays;
    public AnimationCurve themeAlgorithm6to12WeekDays;
    public AnimationCurve themeAlgorithm12to18WeekDays;
    public AnimationCurve themeAlgorithm18to24WeekDays;

    public AnimationCurve themeAlgorithm0to6WeekEnd;
    public AnimationCurve themeAlgorithm6to12WeekEnd;
    public AnimationCurve themeAlgorithm12to18WeekEnd;
    public AnimationCurve themeAlgorithm18to24WeekEnd;

    public Color graphLineColor;
}