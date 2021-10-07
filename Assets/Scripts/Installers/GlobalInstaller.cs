using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
  public override void InstallBindings()
  {
    SignalBusInstaller.Install(Container);
    Container.DeclareSignal<OnLoginSuccessesSignal>();
    Container.DeclareSignal<OnLoginFailedSignal>();

    Container.Bind<IAuthenticator>().To<PlayFabAuthenticator>().AsCached();
  }
}
