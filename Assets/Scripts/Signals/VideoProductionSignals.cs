using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelectThemeSignal
{
    public ThemeType themeType;
    public short ThemeId;
}

public class StartRecordingSignal //Dummy - Deprecated, this is done in VideoManager_VC - StartRecordingVideo()
{
    public ThemeType[] recordedThemes;
    public string videoName;
}

public class StartPublishSignal //Dummy - Not used
{
}

public class PublishVideoSignal
{
    public string videoName;
    public ThemeType[] videoThemes;
    public VideoQuality videoSelectedQuality;
    public float qualityValue;
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
public class UpdateSoftCurrencySignal
{

}
public class UpdateHardCurrencySignal
{

}
public class UpdateExperienceSignal
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

public class OpenLeaderboardsSignal
{

}

public class OpenEnergyInventorySignal
{

}

public class LevelUpSignal
{
    public int level;
    public RewardsData reward;
}

public class AddSubsForExperienceSignal
{
    public ulong subs;
}
public class AddViewsForExperienceSignal
{
    public ulong views;
}
public class AddSoftCurrencyForExperienceSignal
{
    public ulong softCurrency;
}

public class OpenLevelUpPanelSignal
{

}

public class OpenDefaultMessagePopUpSignal
{
    public string message;
}

public class OpenAdsDefaultPopUpSignal
{
    public string message;
}
public class CloseAdsDefaultPopUpSignal
{

}
public class ChangeBackButtonSignal
{
    public bool changeToHome;
}
public class UseEnergyItemSignal
{
    public string label;
    public float energy;
}
public class ChangeRestStateSignal
{

}