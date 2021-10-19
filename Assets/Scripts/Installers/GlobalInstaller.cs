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
    
    //Dependencies

    Container.Bind<PlayerDataManager>().FromInstance(PlayerDataManager.Instance).AsSingle();
    Container.Bind<IAuthenticator>().To<PlayFabAuthenticator>().AsSingle();
  
  }
}
