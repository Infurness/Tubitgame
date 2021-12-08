using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text HcPriceText,SCPriceText;
    [SerializeField] private Image rarenessImage, iconImage;
    [SerializeField] private Button HCBuyButton, SCBuyButton;
    
    void Start()
    {
        
    }

    public void SetHCBuyButton(uint price,Sprite rarenessSprite,Sprite iconSprite,UnityAction buyAction)
    {
        rarenessImage.sprite = rarenessSprite;
        iconImage.sprite = iconSprite;
        HcPriceText.text = price.ToString();
        HCBuyButton.gameObject.SetActive(true);
        SCBuyButton.gameObject.SetActive(false);
        HCBuyButton.onClick.AddListener(buyAction);
    }
    public void SetSCBuyButton(uint price,Sprite rarenessSprite,Sprite iconSprite,UnityAction buyAction)
    {
        rarenessImage.sprite = rarenessSprite;
        iconImage.sprite = iconSprite;
        SCPriceText.text = price.ToString();
        HCBuyButton.gameObject.SetActive(false);
        SCBuyButton.gameObject.SetActive(true);
        SCBuyButton.onClick.AddListener(buyAction);
    }
    public void SetBuyByBothButtons(uint HCprice,uint SCPrice,Sprite rarenessSprite,Sprite iconSprite,UnityAction buySCAction,UnityAction buyHCAction)
    {
        rarenessImage.sprite = rarenessSprite;
        iconImage.sprite = iconSprite;
        HcPriceText.text = HCprice.ToString();
        SCPriceText.text = SCPrice.ToString();
        
        HCBuyButton.gameObject.SetActive(true);
        SCBuyButton.gameObject.SetActive(true);
        HCBuyButton.onClick.AddListener(buyHCAction);
        SCBuyButton.onClick.AddListener(buySCAction);
    }
    void Update()
    {
        
    }
}
