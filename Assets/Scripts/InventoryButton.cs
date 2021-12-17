using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    private Button itemButton;
    [SerializeField] Image itemLogo;
    public string Type;

    [SerializeField] private TMP_Text LabelText;

    public void SetButtonSprites(Sprite logoSprite,Sprite rarenessSprite)
    {
        itemLogo.sprite = logoSprite;
        itemButton.image.sprite = rarenessSprite;
        itemLogo.preserveAspect = true;
    }
    void Awake()
    {
        itemButton = GetComponent<Button>();
    }

    public void SetButtonAction(UnityAction buttonAction)
    {
        itemButton.onClick.AddListener(buttonAction);
    }
    
   
}
