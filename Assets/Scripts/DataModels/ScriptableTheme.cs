using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/ThemesInfo", order = 1)]
public class ScriptableTheme : ScriptableObject
{
    public List<ThemeData> themesData;
#if UNITY_EDITOR
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
    public void RandomizeCurves()
    {
        for(int i=0; i<themesData.Count;i++)
        {
            ThemeData theme = themesData[i];
            theme.themeAlgorithm0to6WeekDays = RandomAnimCurve();
            theme.themeAlgorithm0to6WeekEnd = RandomAnimCurve();
            theme.themeAlgorithm12to18WeekDays = RandomAnimCurve();
            theme.themeAlgorithm12to18WeekEnd = RandomAnimCurve();
            theme.themeAlgorithm18to24WeekDays = RandomAnimCurve();
            theme.themeAlgorithm18to24WeekEnd = RandomAnimCurve();
            theme.themeAlgorithm6to12WeekDays = RandomAnimCurve();
            theme.themeAlgorithm6to12WeekEnd = RandomAnimCurve();
            themesData[i] = theme;
        }
    }
    AnimationCurve RandomAnimCurve()
    {
        AnimationCurve animCurve = new AnimationCurve();
        int numberOfKeys = UnityEngine.Random.Range(5, 10);
        float[] timeForEachKey = new float[numberOfKeys];
        float timeSections = 1f / numberOfKeys;
        for(int i=0;i<timeForEachKey.Length;i++)
        {
            if (i == 0)
                timeForEachKey[i] = 0;
            else if (i == timeForEachKey.Length - 1)
                timeForEachKey[i] = 1;
            else
                timeForEachKey[i] = UnityEngine.Random.Range(timeSections * i, timeSections * (i + 1));
        }
        float[] valueForEachKey = new float[numberOfKeys];
        for (int i = 0; i < timeForEachKey.Length; i++)
        {
            valueForEachKey[i] = UnityEngine.Random.Range(0f, 2f);
        }

        for (int i = 0; i < numberOfKeys; i++)
        {
            animCurve.AddKey(timeForEachKey[i], valueForEachKey[i]);
            UnityEditor.AnimationUtility.SetKeyRightTangentMode(animCurve, i, UnityEditor.AnimationUtility.TangentMode.Linear);
            UnityEditor.AnimationUtility.SetKeyLeftTangentMode(animCurve, i, UnityEditor.AnimationUtility.TangentMode.Linear);
        }

        return animCurve;
    }
    bool CheckIfThemeExistsInList (ThemeType _themeType)
    {
        return themesData.Exists (x => x.themeType == _themeType);
    }
#endif
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