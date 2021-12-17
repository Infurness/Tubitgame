using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectCell : MonoBehaviour
{
    [SerializeField] private TMP_Text effectText;

   

    public void SetText(string text)
    {
        effectText.text = text;
    }
}
