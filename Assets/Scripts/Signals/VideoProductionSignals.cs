using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class OpenThemeSelectionSignal
{

}
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

public class OpenVideoManager
{

public class OnVideosStatsUpdatedSignal
{
    
}