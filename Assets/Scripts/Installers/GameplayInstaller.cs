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
        Container.DeclareSignal<EnergyValueSignal> ();
        Container.DeclareSignal<AddEnergySignal> ();
        Container.DeclareSignal<ShowVideosStatsSignal> ();
        Container.DeclareSignal<GetMoneyFromVideoSignal> ();
        Container.DeclareSignal<OpenVideoManagerSignal> ();
        Container.DeclareSignal<Recieve3BestLeaderboard> ();
        Container.DeclareSignal<RecievePlayerLeaderboardPosition> ();
        Container.DeclareSignal<OnVideosStatsUpdatedSignal> ();
        Container.DeclareSignal<OpenThemeSelectorPopUpSignal> ();
        Container.DeclareSignal<ThemeHeldSignal> ();
        Container.DeclareSignal<ConfirmThemesSignal> ();
        Container.DeclareSignal<CancelVideoRecordingSignal> ();
        Container.DeclareSignal<ChangeUsernameSignal> ();
        Container.DeclareSignal<OpenVideoCreationSignal> ();
        Container.DeclareSignal<CloseVideoCreationSignal> ();
        Container.DeclareSignal<ChangePlayerSubsSignal> ();
        Container.DeclareSignal<UpdateThemesGraphSignal> ();
        Container.DeclareSignal<CloseSettingPanelSignal> ();

        //Dependencies
        Container.Bind<PlayerData> ().AsSingle();
        Container.Bind<YouTubeVideoManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<AlgorithmManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<ThemesManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<EnergyManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<PlayfabLeaderboard> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<PlayerDataManager>().FromInstance(PlayerDataManager.Instance);


    }
}
