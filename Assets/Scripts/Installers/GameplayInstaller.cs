using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
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
        Container.DeclareSignal<UpdateSoftCurrency> ();
        Container.DeclareSignal<OpenVideoManagerSignal> ();
        Container.DeclareSignal<Recieve3BestLeaderboard> ();
        Container.DeclareSignal<RecieveTop10Leaderboard> ();
        Container.DeclareSignal<RecievePlayerLeaderboardPosition> ();
        Container.DeclareSignal<OnVideosStatsUpdatedSignal> ();
        Container.DeclareSignal<OpenThemeSelectorPopUpSignal> ();
        Container.DeclareSignal<ThemeHeldSignal> ();
        Container.DeclareSignal<ConfirmThemesSignal> ();
        Container.DeclareSignal<CancelVideoRecordingSignal> ();
        Container.DeclareSignal<ChangeUsernameSignal> ();
        Container.DeclareSignal<OpenVideoCreationSignal> ();
        Container.DeclareSignal<CloseVideoCreationSignal> ();
        Container.DeclareSignal<UpdateThemesGraphSignal> ();
        Container.DeclareSignal<OpenSettingPanelSignal> ();
        Container.DeclareSignal<OpenDeleteAccountSignal> ();
        Container.DeclareSignal<OpenLeaderboardsSignal> ();
        Container.DeclareSignal<AddSubsForExperienceSignal> ();
        Container.DeclareSignal<AddViewsForExperienceSignal> ();
        Container.DeclareSignal<LevelUpSignal> ();
        Container.DeclareSignal<UpdateRankSignal> ();

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
