using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonThemePreProductionView : MonoBehaviour
{
    public ThemeType themeType;
    [SerializeField] private TMP_Text buttonText;

    private void Start ()
    {
        buttonText.text = Enum.GetName (themeType.GetType (), themeType);
    }
}
