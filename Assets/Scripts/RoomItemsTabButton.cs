using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomItemsTabButton : MonoBehaviour
{
    public static RoomItemsTabButton CurrentButton;
    private static UnityAction unSelectAllButtons;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite nonSelectedSprite;
    [SerializeField] private Color selectTextColor;
    private Button tabButton;
    private TMP_Text buttonText;
    
    
    private void Start()
    {
        tabButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();
        tabButton.onClick.AddListener(SelectButton);
        unSelectAllButtons += UnselectButton;
    }

   public void SelectButton()
   {
       unSelectAllButtons.Invoke();
       tabButton.image.sprite = selectedSprite;
       buttonText.color=selectTextColor;
   }

   public void UnselectButton()
   {
       tabButton.image.sprite = nonSelectedSprite;
       buttonText.color=Color.white;
   }
}
