using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ThemesManager : MonoBehaviour
{

    [SerializeField] private ScriptableTheme themesData;

    // Start is called before the first frame update
    [Inject] private SignalBus signBus;

    private void Start()
    {
        signBus.Subscribe<OnPlayerEquippedItemChangedSignal>(OnPlayerEquipmentsChanged);
    }

    void OnPlayerEquipmentsChanged(OnPlayerEquippedItemChangedSignal playerEquippedItemChangedSignal)
    {
        //todo Process all current equipment update themes popularity @Jorge
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
                return ThemePopularityBasedOnTime (theme, _dayHour);
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
