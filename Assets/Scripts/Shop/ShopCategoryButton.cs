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

    private void Awake()
    {
        deSelcectAll += SetButtonUnSelected;
        SetButtonUnSelected();
    }

    void Start()
    {
        
    }

    public void SetButtonSelected()
    {
        deSelcectAll.Invoke();
        buttonText.color = highlightColor;
        iconImage.color = highlightColor;
        highlightImage.gameObject.SetActive(true);
    }

    public void SetButtonUnSelected()
    {
        buttonText.color = Color.gray;
        iconImage.color = Color.gray;
        highlightImage.gameObject.SetActive(false);

    }

}
