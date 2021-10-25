using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PopUps_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private GameObject popUpsBlockBackgroundPanel;

    [SerializeField] private GameObject themeSelectionPanel;
    [SerializeField] private Button themeSelectionPanelCloseButton;

    [SerializeField] private GameObject skipRecordPanel;
    [SerializeField] private Button skipRecordPanelCloseButton;

    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenThemeSelectorPopUpSignal> (OpenThemeSelector);        

        themeSelectionPanelCloseButton.onClick.AddListener (CloseThemeSelector);
        signalBus.Subscribe<ConfirmThemesSignal> (CloseThemeSelector);

        InitialState ();
    }
    void InitialState ()
    {
        CloseThemeSelector ();
    }
    void OpenThemeSelector ()
    {
        popUpsBlockBackgroundPanel.SetActive (true);
        themeSelectionPanel.SetActive (true);
    }
    void CloseThemeSelector ()
    {
        popUpsBlockBackgroundPanel.SetActive (false);
        themeSelectionPanel.SetActive (false);
    }
}
