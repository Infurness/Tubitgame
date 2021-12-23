using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {

       
        //Dependencies

        Container.Bind<PlayerData> ().AsSingle();
        Container.Bind<YouTubeVideoManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<AlgorithmManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<ThemesManager> ().FromComponentInHierarchy ().AsSingle ();

        Container.Bind<PlayerInventory>().FromInstance(PlayerInventory.Instance);
        
        Container.Bind<EnergyManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<PlayfabLeaderboard> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<PlayerDataManager>().FromInstance(PlayerDataManager.Instance);

        Container.Bind<ExperienceManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<GlobalAudioManager> ().FromComponentInHierarchy ().AsSingle ();
        Container.Bind<EnergyInventoryManager> ().FromComponentInHierarchy ().AsSingle ();
     //   Container.Bind<RoomInteractivityManager> ().FromComponentInHierarchy ().AsSingle (); 

        Container.Bind<CheatsManager> ().FromComponentInHierarchy ().AsSingle ();

        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();
        Container.Bind<AdsRewardsManager> ().FromComponentInHierarchy ().AsSingle ();
    }
}
