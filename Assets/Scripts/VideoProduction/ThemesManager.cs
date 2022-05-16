using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using Zenject;

[Serializable]
class ImagePerTheme
{
    public ThemeType themeType;
    public Sprite themeImage;
}
public class ThemesManager : MonoBehaviour
{

    [SerializeField] private ScriptableTheme themesData;
    [SerializeField] private ImagePerTheme[] imagesPerTheme;
    [Header("Push notifications")]
    [Range(1.5f,2f)]
    [SerializeField] private float popularityLimit = 1.8f;
    [SerializeField] private float minutesToForecastPopularity = 30;
    private List<CustomizationThemeEffect> bounsEffect;
    [Inject] private SignalBus signBus;
    [Inject] private IPushNotificationsManager pushNotification;

    private void Start()
    {
        bounsEffect = new List<CustomizationThemeEffect>();
        foreach (var themeData in themesData.themesData)     
        {
            bounsEffect.Add(new CustomizationThemeEffect(){ThemeType = themeData.themeType,themePopularityFactor = 0});
        }
        signBus.Subscribe<OnPlayerEquippedThemeItemChangedSignal>(OnPlayerEquipmentsChanged);

        StartCoroutine(CheckNextPopularTheme());
    }

    private IEnumerator CheckNextPopularTheme()
    {
        var time = GameClock.Instance.Now.AddMinutes(minutesToForecastPopularity);
        foreach (ThemeData theme in themesData.themesData)
        {
            var popularity = ThemePopularityBasedOnTime (theme, time);
            if (popularity > popularityLimit)
            {
                var title = "Popular Theme";
                var subtitle = "Login to create a video.";
                var text = new string[] {$"{theme.themeType} is rising through the chart!", 
                                        $"People can't stop watching {theme.themeType} at the moment!",
                                        $"People are crazy about {theme.themeType}. Take your chance, now!"};
                var fireTimeSeconds = minutesToForecastPopularity * 60;
                var id = 3;
       
                pushNotification.ScheduleNotification(title, subtitle, text[UnityEngine.Random.Range(0, text.Length)], fireTimeSeconds, id);
                break;
            }
        }
        
        yield return new WaitForSeconds(minutesToForecastPopularity * 60);
        StartCoroutine(CheckNextPopularTheme());
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
                        bounsEffect[(int) ThemeType.AutosAndVehicles].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Music:
                        bounsEffect[(int) ThemeType.Music].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.PetsAndAnimals:
                        bounsEffect[(int) ThemeType.PetsAndAnimals].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Sports:
                        bounsEffect[(int) ThemeType.Sports].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.TravelAndEvents:
                        bounsEffect[(int) ThemeType.TravelAndEvents].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Gaming:
                        bounsEffect[(int) ThemeType.Gaming].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.PeopleAndBlogs:
                        bounsEffect[(int) ThemeType.PeopleAndBlogs].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Comedy:
                        bounsEffect[(int) ThemeType.Comedy].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Entertainment:
                        bounsEffect[(int) ThemeType.Entertainment].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.NewsAndPolitics:
                        bounsEffect[(int) ThemeType.NewsAndPolitics].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.HowToAndStyle:
                        bounsEffect[(int) ThemeType.HowToAndStyle].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.Education:
                        bounsEffect[(int) ThemeType.Education].themePopularityFactor += themeEffect.themePopularityFactor;
                        break;
                    case ThemeType.ScienceAndTechnology:
                        bounsEffect[(int) ThemeType.ScienceAndTechnology].themePopularityFactor += themeEffect.themePopularityFactor;
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

    public float GetThemePopularity (ThemeType _themeType, DateTime _dayHour)
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
        return themPopularity;
    }
    public ThemeData[] GetThemesData ()
    {
        return themesData.themesData.ToArray ();
    }
    public void UpdateThemesData (ScriptableTheme newThemesData)
    {
        themesData.themesData = newThemesData.themesData;
    }
    public Sprite GetThemeSprite(ThemeType themeType)
    {
        return imagesPerTheme.FirstOrDefault(pair => pair.themeType == themeType).themeImage;
    }
}
