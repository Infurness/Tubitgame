using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Zenject;
public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        
        //Signals
        Container.DeclareSignal<SelectThemeSignal>();
        Container.DeclareSignal<StartRecordingSignal>();
        Container.DeclareSignal<StartPublishSignal>();
        Container.DeclareSignal<PublishVideoSignal>();
        Container.DeclareSignal<EndPublishVideoSignal> ();
        Container.DeclareSignal<OpenThemeSelectionSignal> ();
        Container.DeclareSignal<EnergyValueSignal> ();
        Container.DeclareSignal<AddEnergySignal> ();
        Container.DeclareSignal<ShowVideosStatsSignal> ();
        Container.DeclareSignal<GetMoneyFromVideoSignal> (); 

        //Dependencies
        Container.Bind<PlayerData> ().AsSingle();
        Container.Bind<PlayerDataManager> ().FromComponentInHierarchy().AsSingle ();
        Container.Bind<YouTubeVideoManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<AlgorithmManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<ThemesManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<EnergyManager> ().FromComponentInHierarchy ().AsSingle ();
    }
}
