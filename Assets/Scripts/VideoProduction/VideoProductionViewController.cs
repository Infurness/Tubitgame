using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class VideoProductionViewController : MonoBehaviour
{
    [Inject] SignalBus _signalBus;

   

    [SerializeField] private TMP_Text selectedTheme;
    [SerializeField] private GameObject preProductionPanel, productionPanel, postProduction;
      
    public void OnSelectTheme(string themeName)
    {
        _signalBus.TryFire<SelectThemeSignal>(new SelectThemeSignal()
        {
            ThemeName = themeName
            
            
        });
        selectedTheme.text = "Theme : " + themeName;
    }

    public void OnStartRecordingPressed()
    {
        
    }

   
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
