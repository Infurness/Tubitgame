using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ThemesManager : MonoBehaviour
{

    [SerializeField] private ScriptableTheme themesData;

    private List<CustomizationThemeEffect> bounsEffect;
    // Start is called before the first frame update
    [Inject] private SignalBus signBus;

    private void Start()
    {
        bounsEffect = new List<CustomizationThemeEffect>();
        foreach (var themeData in themesData.themesData)     
        {
            bounsEffect.Add(new CustomizationThemeEffect(){ThemeType = themeData.themeType,themePopularityFactor = 0});
        }
        signBus.Subscribe<OnPlayerEquippedThemeItemChangedSignal>(OnPlayerEquipmentsChanged);
        
    }

    void OnPlayerEquipmentsChanged(OnPlayerEquippedThemeItemChangedSignal playerEquippedThemeItemChangedSignal)
    {
        foreach (var themeEffect in bounsEffect)
        {
            themeEffect.themePopularityFactor = 0;
        }
        foreach (var item in playerEquippedThemeItemChangedSignal.CustomizationItems)
        {
            foreach (var themeEffect in item.affectedTheme)
            {
                switch (themeEffect.ThemeType)
                {
                    case ThemeType.FilmsAndAnimation:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.AutosAndVehicles:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Music:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.PetsAndAnimals:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Sports:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.TravelAndEvents:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Gaming:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.PeopleAndBlogs:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Comedy:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Entertainment:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.NewsAndPolitics:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.HowToAndStyle:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Education:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.ScienceAndTechnology:
                        bounsEffect[(int) ThemeType.FilmsAndAnimation].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public ThemeType[] GetThemes ()
    {
        return (ThemeType[])Enum.GetValues (typeof (ThemeType));
    }

    public float GetThemePopularity (ThemeType _themeType, int _dayHour)
    {
        foreach (ThemeData theme in themesData.themesData)
        {
            if (theme.themeType == _themeType)
            {
                return ThemePopularityBasedOnTime (theme, _dayHour)+bounsEffect[(int)_themeType].themePopularityFactor;
            }
        }
        Debug.LogError ($"No theme: {Enum.GetName (_themeType.GetType (), _themeType)}, is available");
        return 0;
    }

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
