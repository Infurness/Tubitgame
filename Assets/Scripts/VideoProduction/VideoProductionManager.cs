using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VideoProductionManager : MonoBehaviour
{
    [Inject] SignalBus _signalBus;

    
    void Start()
    { 
        _signalBus.Subscribe<SelectThemeSignal>(signal => print("Theme : " + signal.ThemeName + " is Selected"));
    }

  
    void Update()
    {
        
    }
}
