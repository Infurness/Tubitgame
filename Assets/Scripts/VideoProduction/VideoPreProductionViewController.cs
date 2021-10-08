using System;
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

    private List<ThemeType> selectedThemes = new List<ThemeType> ();

    void Start()
    {
        recordButton.onClick.AddListener(OnStartRecordingPressed);

    }
    public void OnSelectTheme(GameObject button)
    {
        ThemeType _themeType = button.GetComponent<ButtonThemePreProductionView> ().themeType;
        _signalBus.TryFire<SelectThemeSignal>(new SelectThemeSignal()
        {
            themeType = _themeType
        });
        selectedTheme.text = $"Theme: {Enum.GetName(_themeType.GetType(), _themeType)}";
        selectedThemes.Add (_themeType);
        recordButton.interactable = true;
    }
    public void OnStartRecordingPressed()
    {
        preProductionPanel.SetActive(false);
        _signalBus.Fire<StartRecordingSignal>(new StartRecordingSignal()
        {
            recordingTime = 3f,
            recordedThemes = selectedThemes.ToArray()
        });
        selectedThemes.Clear ();
    }

    
    void Update()
    {
        
    }
}
