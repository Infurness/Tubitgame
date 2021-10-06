using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Zenject;
public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        // Signals Declaration 
        Container.DeclareSignal<SelectThemeSignal>();
        Container.DeclareSignal<StartRecordingSignal>();
        Container.DeclareSignal<PublishNewVideoSignal>();
       
        //Fields 

        Container.Bind<PlayerDataManger>().FromComponentInHierarchy().AsSingle();



    }
}
