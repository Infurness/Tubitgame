using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelectThemeSignal
{
    public ThemeType themeType;
    public short ThemeId;
}

public class StartRecordingSignal
{
    public float recordingTime;
    public ThemeType[] recordedThemes;
}

public class StartPublishSignal
{
}

public class PublishVideoSignal
{
    public string videoName;
    public ThemeType[] videoThemes;
}
public class EndPublishVideoSignal
{
    public string videoName;
}

public class ShowVideosStatsSignal
{

}

public class EnergyValueSignal
{
    public float energy;
}
public class AddEnergySignal
{
    public float energyAddition;
}

public class GetMoneyFromVideoSignal
{
    public string videoName;
}

public class OpenVideoManagerSignal
{
}
public class OnVideosStatsUpdatedSignal
{
    
}

public class OpenThemeSelectorPopUpSignal
{

}

public class ThemeHeldSignal
{
    public GameObject themeBox;
    public ThemeType themeType;
    public string buttonText;
}
public class ConfirmThemesSignal
{
    public Dictionary<int, ThemeType> selectedThemesSlots;
}