using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class ThemeGraphSelectorButton_VC : MonoBehaviour
{
    SignalBus signalBus;

    private ThemeType theme;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject selectedBG;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectColor;

    public void SetUpReferences(SignalBus bus, ThemeType themeType)
    {
        signalBus = bus;
        theme = themeType;
        SetUp();
    }
    void SetUp()
    {
        signalBus.Subscribe<SelectThemeInGraphSignal>(SelectTheme);
    }
    void SelectTheme(SelectThemeInGraphSignal signal)
    {
        bool selected = signal.themeType == theme;
        if (selected && selectedBG.activeSelf)
        {
            signalBus.Fire(new ResetLineColorInGraphSignal());
            selected = !selected;
        }
        else if(selected && !selectedBG.activeSelf)
        {
            signalBus.Fire<SetLineColorInGraphSignal>(new SetLineColorInGraphSignal {themeType = signal.themeType});
        }

        selectedBG.SetActive(selected);
        if (selected)
            text.color = selectedColor;
        else
            text.color = unselectColor;
    }
}
