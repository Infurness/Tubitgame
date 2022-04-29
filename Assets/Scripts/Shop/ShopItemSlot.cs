using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text HcPriceText,SCPriceText;
    [SerializeField] private Image  iconImage;
    [SerializeField] private Button HCBuyButton, SCBuyButton;
    [SerializeField] private TMP_Text itemNameText;
    void Start()
    {
        
    }

    public void SetHCBuyButton(uint price,string itemName,Sprite iconSprite,UnityAction buyAction)
    {
        // rarenessImage.sprite = rarenessSprite;
        iconImage.sprite = iconSprite;
        HcPriceText.text = price.ToString();
        HCBuyButton.gameObject.SetActive(true);
        SCBuyButton.gameObject.SetActive(false);
        itemNameText.text = string.Concat(itemName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        HCBuyButton.onClick.AddListener(buyAction);
    }
    public void SetSCBuyButton(uint price,string itemName,Sprite iconSprite,UnityAction buyAction)
    {
        // rarenessImage.sprite = rarenessSprite;
        iconImage.sprite = iconSprite;
        SCPriceText.text = price.ToString();
        itemNameText.text = string.Concat(itemName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        HCBuyButton.gameObject.SetActive(false);
        SCBuyButton.gameObject.SetActive(true);
        SCBuyButton.onClick.AddListener(buyAction);
    }
    public void SetBuyByBothButtons(uint HCprice,uint SCPrice,string itemName,Sprite iconSprite,UnityAction buySCAction,UnityAction buyHCAction)
    {
        // rarenessImage.sprite = rarenessSprite;
        iconImage.sprite = iconSprite;
        HcPriceText.text = HCprice.ToString();
        SCPriceText.text = SCPrice.ToString();
        
        HCBuyButton.gameObject.SetActive(true);
        SCBuyButton.gameObject.SetActive(true);
        itemNameText.text = string.Concat(itemName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

        HCBuyButton.onClick.AddListener(buyHCAction);
        SCBuyButton.onClick.AddListener(buySCAction);
    }
    void Update()
    {
        
    }
}
