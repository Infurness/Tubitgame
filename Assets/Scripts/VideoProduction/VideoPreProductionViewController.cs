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
    [Inject] ThemesManager themesManager;

    [SerializeField] private GameObject preProductionPanel;
    [SerializeField] private TMP_Text selectedTheme;
    [SerializeField] private Button recordButton;

    [SerializeField] private GameObject themeButtonsHolder;
    [SerializeField] private GameObject themeButtonPrefab;

    private List<ThemeType> selectedThemes = new List<ThemeType> ();

    void Start()
    {
        _signalBus.Subscribe<OpenThemeSelectionSignal> (OpenThisMenu);
        recordButton.onClick.AddListener(OnStartRecordingPressed);
        
        SetUpThemeButtons ();  
    }
    public void OpenThisMenu ()
    {
        preProductionPanel.SetActive (true);
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

    void SetUpThemeButtons ()
    {
        foreach(ThemeType themeType in themesManager.GetThemes())
        {
            CreateThemeButton (themeType);
        }
    }
    void CreateThemeButton (ThemeType _themeType)
    {
        GameObject button = Instantiate (themeButtonPrefab, themeButtonsHolder.transform);
        button.GetComponent<ButtonThemePreProductionView> ().themeType = _themeType;
        button.GetComponent<Button> ().onClick.AddListener (() => { OnSelectTheme (button); });
    }
    
    void Update()
    {
        
    }
}
