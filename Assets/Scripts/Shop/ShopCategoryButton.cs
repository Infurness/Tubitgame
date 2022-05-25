using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopCategoryButton : MonoBehaviour
{
     static ShopCategoryButton selectedButton;
     static UnityAction deSelcectAll;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Image highlightImage;
    [SerializeField] private Image bgImage;

    private void Awake()
    {
        deSelcectAll += SetButtonUnSelected;
        SetButtonUnSelected();
    }

    public void SetButtonSelected()
    {
        deSelcectAll.Invoke();
        buttonText.color = highlightColor;
        iconImage.color = highlightColor;
        highlightImage.gameObject.SetActive(true);
        bgImage.gameObject.SetActive(false);
    }

    public void SetButtonUnSelected()
    {
        buttonText.color = Color.gray;
        if(iconImage != null)
            iconImage.color = Color.gray;
        if(highlightImage != null)
            highlightImage.gameObject.SetActive(false);
        if(bgImage != null)
            bgImage.gameObject.SetActive(true);
  
    }

}
