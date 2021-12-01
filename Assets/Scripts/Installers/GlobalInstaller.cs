using System.Collections;
using System.Collections.Generic;
using Google;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
    
        // Signals
        Container.DeclareSignal<OnPlayFabLoginSuccessesSignal>();
        Container.DeclareSignal<OnLoginFailedSignal>();
        Container.DeclareSignal<OnGoogleSignInFailed>();
        Container.DeclareSignal<OnGoogleSignInSuccessSignal>();
        Container.DeclareSignal<OnFacebookLoginFailedSignal>();
        Container.DeclareSignal<OnFacebookLoginSuccessSignal>();
        Container.DeclareSignal<OnAppleLoginSuccessSignal>();
        Container.DeclareSignal<OnAppleLoginFailedSignal>();
        Container.DeclareSignal<OnPurchaseProductSignal>();
        Container.DeclareSignal<ProcessPurchaseSignal>();
        Container.DeclareSignal<ConfirmPendingPurchaseSignal>();

        Container.DeclareSignal<SaveRoomLayoutSignal>();
        Container.DeclareSignal<DiscardRoomLayoutSignal>();
        Container.DeclareSignal<AddSoftCurrencyForExperienceSignal> ();
        Container.DeclareSignal<ChangePlayerSubsSignal> ();
        Container.DeclareSignal<AssetsLoadedSignal>();
        Container.DeclareSignal<RemoteAssetsCheckSignal>();
  
        //Customizations signals 
        Container.DeclareSignal<OnPlayerEquippedThemeItemChangedSignal>();
        Container.DeclareSignal<TestRoomThemeItemSignal>();
        Container.DeclareSignal<TestRoomVideoQualityITemSignal>();
        Container.DeclareSignal<OnPlayerInventoryFetchedSignal>();
        Container.DeclareSignal<OnPlayerInventoryFetchedSignal>();

        //Advertisement signals
        Container.DeclareSignal<AdsInitializedSignal> ();
        Container.DeclareSignal<RewardAdLoadedSignal> ();
        Container.DeclareSignal<StartShowingAdSignal> ();
        Container.DeclareSignal<GrantRewardSignal> ();
        Container.DeclareSignal<NotGrantedRewardSignal> (); 
        Container.DeclareSignal<FinishedAdVisualitationRewardSignal> ();
        Container.DeclareSignal<OpenSoftCurrencyAdSignal> ();
        Container.DeclareSignal<OpenDoubleViewsAdSignal> ();

        //Dependencies
        Container.Bind<PlayerInventory>().FromInstance(PlayerInventory.Instance);

        Container.Bind<PlayerDataManager>().FromInstance(PlayerDataManager.Instance);
        Container.Bind<IAuthenticator>().To<PlayFabAuthenticator>().AsSingle();
    }
}
