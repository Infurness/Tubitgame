using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelectThemeSignal
{
    public ThemeType themeType;
    public short ThemeId;
}

public class StartRecordingSignal //Deprecated, this is done in VideoManager_VC - StartRecordingVideo()
{
    public ThemeType[] recordedThemes;
    public string videoName;
}

public class StartPublishSignal //Not used
{
}

public class PublishVideoSignal
{
    public string videoName;
    public ThemeType[] videoThemes;
}
public class EndPublishVideoSignal
{
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
public class UpdateSoftCurrency
{

}

public class OpenVideoManagerSignal
{
}
public class OpenVideoCreationSignal
{
}
public class CloseVideoCreationSignal
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

public class CancelVideoRecordingSignal
{
    public string name;
}

public class UpdateThemesGraphSignal
{

}

public class OpenSettingPanelSignal
{

}

public class OpenDeleteAccountSignal
{

}