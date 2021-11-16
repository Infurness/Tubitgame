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
<<<<<<< HEAD
=======

>>>>>>> 2e36ce1daf816edb42e05b7e6e244a28b40d1495
    Container.DeclareSignal<OnPlayerInventoryFetchedSignal>();

    Container.DeclareSignal<AddSoftCurrencyForExperienceSignal> ();
    Container.DeclareSignal<ChangePlayerSubsSignal> ();
    
<<<<<<< HEAD
=======

>>>>>>> 2e36ce1daf816edb42e05b7e6e244a28b40d1495
    //Dependencies

    Container.Bind<PlayerDataManager>().FromInstance(PlayerDataManager.Instance);
    Container.Bind<IAuthenticator>().To<PlayFabAuthenticator>().AsSingle();
  
  }
}
