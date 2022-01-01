using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConsumableInventoryButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private Button buyButton;

    public void SetButtonData(Sprite itemSprite,string _item_Name,int price,int _amount,UnityAction buttonAction )
    {
        iconImage.sprite = itemSprite;
        itemName.text = _item_Name;
        priceText.text = price.ToString();
        amount.text = "X" + _amount;
        buyButton.onClick.AddListener(buttonAction);

    }
}
