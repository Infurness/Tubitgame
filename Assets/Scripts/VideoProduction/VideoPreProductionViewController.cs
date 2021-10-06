using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VideoPreProductionViewController : MonoBehaviour
{
    [Inject] SignalBus _signalBus;
    [SerializeField] private GameObject preProductionPanel;
    [SerializeField] private TMP_Text selectedTheme;
    [SerializeField] private Button recordButton;

    void Start()
    {
        recordButton.onClick.AddListener(OnStartRecordingPressed);

    }
    public void OnSelectTheme(string themeName)
    {
        _signalBus.TryFire<SelectThemeSignal>(new SelectThemeSignal()
        {
            ThemeName = themeName
            
            
        });
        selectedTheme.text = "Theme : " + themeName;
        recordButton.interactable = true;
    }
    public void OnStartRecordingPressed()
    {
        preProductionPanel.SetActive(false);
        _signalBus.Fire<StartRecordingSignal>(new StartRecordingSignal()
        {
            RecordingTime = 10f
        });
     
    }

    
    void Update()
    {
        
    }
}
