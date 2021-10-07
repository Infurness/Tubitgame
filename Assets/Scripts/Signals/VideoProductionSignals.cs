using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SelectThemeSignal
{
    public string ThemeName;
    public short ThemeId;
}

public class StartRecordingSignal
{
    public float RecordingTime;
}

public class StartPublishSignal
{
}

public class PublishVideoSignal
{
    public string videoName;
    public Theme[] videoThemes;
}