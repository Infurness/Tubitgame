using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Zenject;
public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        
        Container.DeclareSignal<SelectThemeSignal>();
        Container.DeclareSignal<StartRecordingSignal>();
        Container.DeclareSignal<StartPublishSignal> ();


    }
}
