using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class VideoProductionViewController : MonoBehaviour
{
    [Inject] private SignalBus m_SignalBus;

    [SerializeField] private TMP_Text selectedTheme;
    [SerializeField] private GameObject preProductionPanel, productionPanel, postProduction;

    public void OnSelectTheme(string themeName)
    {
        m_SignalBus.Fire<SelectThemeSignal>(new SelectThemeSignal()
        {
            ThemeName = themeName
        });
    }
    
    

   
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
