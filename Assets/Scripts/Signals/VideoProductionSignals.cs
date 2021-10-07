using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class PublishNewVideoSignal
{
    public Video video;
}