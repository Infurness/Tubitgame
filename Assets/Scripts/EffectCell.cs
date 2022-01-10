using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectCell : MonoBehaviour
{
    [SerializeField] private TMP_Text effectText;
    [SerializeField] private Color colorRed;
    private static bool redColor=true;
    

    public void SetText(string text)
    {
        effectText.text = text;
        effectText.color =redColor? colorRed: Color.white;
        redColor = !redColor;
    }
}
