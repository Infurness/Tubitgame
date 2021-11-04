using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    private Button itemButton;
    [SerializeField] Image itemLogo;
    public string Type;

    [SerializeField] private Image equippedLabel;

    public void SetButtonLogo(Sprite sprite)
    {
        itemLogo.sprite = sprite;
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
