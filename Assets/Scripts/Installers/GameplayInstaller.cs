using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Zenject;
public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        base.InstallBindings();
        Container.DeclareSignal<SelectThemeSignal>();
    }
}
