using System;
using Customizations;
using TMPro;
using UniRx.Triggers;
using  UniRx;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Zenject;

public class Shop_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private Shop shop;
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private PlayerInventory playerInventory;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject itemsPanel,offersPanel,realEstatePanel,currenciesPanel;
    [SerializeField] private Button offersButton, clothingButton, furnitureButton, equipmentsButton,currenciesButton, realEstateButton;
    [SerializeField] private GameObject itemsScrollView, offersButtonsScrollView, offerRedeemPanel;
    [SerializeField] private ShopItemSlot shopItemButton;
    [SerializeField] private Sprite[] rarenessSprites;
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private Image rarenessImage, iconImage,coinImage;
    [SerializeField] private TMP_Text descriptionText, statsText, nameText, rarenessText,priceText;
    [SerializeField] private Sprite hcCoin, scCoin;
    [SerializeField] private Button buyButton;
    void ClearItemsPanel()
    {
        
        for (int i = 0; i < itemsScrollView.transform.childCount; i++)
        {
            Destroy(itemsScrollView.transform.GetChild(i).gameObject);
        }
    }
    
    void Start()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(false);
        offersButton.onClick.AddListener(OpenOffersPanel);
        clothingButton.onClick.AddListener(OpenClothingPanel);
        furnitureButton.onClick.AddListener(OpenFurniturePanel);
        equipmentsButton.onClick.AddListener(OpenEquipmentsPanel);
        realEstateButton.onClick.AddListener(OpenRealEstatePanel);
        currenciesButton.onClick.AddListener(OpenCurrenciesPanel);
        shopPanel.OnEnableAsObservable().Subscribe((unit =>
        {
            offersButton.onClick.Invoke();
            buyPanel.gameObject.SetActive(false);
        }));
    }

    Sprite GetRarenessSpriteByIndex(Rareness rareness)
    {
        return rarenessSprites[(int) rareness];
    }

    public void OpenCurrenciesPanel()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);
        currenciesPanel.gameObject.SetActive(true);

    }
    void OpenOffersPanel()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(true);
        realEstatePanel.gameObject.SetActive(false);
        currenciesPanel.gameObject.SetActive(false);

        
        

    }

    void OpenClothingPanel()
    {
        ClearItemsPanel();
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);
        currenciesPanel.gameObject.SetActive(false);


        var clothes = shop.Clothes;

        foreach (var item in clothes)
        {
            if (!item.Owned)
            {


                var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
                switch (item.PriceType)
                {
                    case PriceType.Free:
                        Destroy(shopButton.gameObject);

                        break;
                    case PriceType.SC:
                        shopButton.SetSCBuyButton(item.SCPrice, GetRarenessSpriteByIndex(item.rareness), item.sprite,
                            (() => BuyClothItem(item, PriceType.SC)));

                        break;
                    case PriceType.HC:
                        shopButton.SetHCBuyButton(item.HCPrice, GetRarenessSpriteByIndex(item.rareness), item.sprite,
                            () => BuyClothItem(item, PriceType.HC));
                        break;

                    case PriceType.Exchangeable:
                        shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                            GetRarenessSpriteByIndex(item.rareness), item.sprite,
                            (() => BuyClothItem(item, PriceType.SC)),
                            (() => BuyClothItem(item, PriceType.HC)));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }

   

    void OpenFurniturePanel()
    {
        ClearItemsPanel();
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);
        currenciesPanel.gameObject.SetActive(false);
    
        var furniture = shop.Furniture;

        foreach (var item in furniture)
        {
            if (!item.Owned)
            {


                var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
                switch (item.PriceType)
                {
                    case PriceType.Free:
                        Destroy(shopButton.gameObject);

                        break;
                        return;
                    case PriceType.SC:
                        shopButton.SetSCBuyButton(item.SCPrice, GetRarenessSpriteByIndex(item.rareness), item.sprite,
                            (() => BuyRoomItem(item, PriceType.SC)));
                        break;
                    case PriceType.HC:
                        shopButton.SetHCBuyButton(item.HCPrice, GetRarenessSpriteByIndex(item.rareness), item.sprite,
                            () => BuyRoomItem(item, PriceType.HC));

                        break;
                    case PriceType.Exchangeable:
                        shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                            GetRarenessSpriteByIndex(item.rareness), item.sprite,
                            (() => BuyRoomItem(item, PriceType.SC)),
                            (() => BuyRoomItem(item, PriceType.HC)));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }

    void OpenEquipmentsPanel()
    {
        ClearItemsPanel();
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);
        currenciesPanel.gameObject.SetActive(false);

        var equipments = shop.Equipments;
      ;
      foreach (var item in equipments)
      {
          if (!item.Owned)
          {
              var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);

              switch (item.PriceType)
              {
                  case PriceType.Free:
                      Destroy(shopButton.gameObject);

                      break;
                  case PriceType.SC:
                      shopButton.SetSCBuyButton(item.SCPrice, GetRarenessSpriteByIndex(item.rareness), item.itemSprite,
                          (() => BuyVCItem(item, PriceType.SC)));

                      break;
                  case PriceType.HC:
                      shopButton.SetHCBuyButton(item.HCPrice, GetRarenessSpriteByIndex(item.rareness), item.itemSprite,
                          () => BuyVCItem(item, PriceType.HC));

                      break;
                  case PriceType.Exchangeable:
                      shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                          GetRarenessSpriteByIndex(item.rareness), item.itemSprite,
                          (() => BuyVCItem(item, PriceType.SC)),
                          (() => BuyVCItem(item, PriceType.HC)));

                      break;
                  default:
                      throw new ArgumentOutOfRangeException();
              }
          }
      }

    }

    void OpenRealEstatePanel()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(false);
       realEstatePanel.gameObject.SetActive(true);
    }
    
    public void BuyHCBundle(string id)
    {
        
        _signalBus.Fire<OnPurchaseProductSignal>(new OnPurchaseProductSignal()
        {
            productID = id
        });
    }

    public void BuyNoAds()
    {
        _signalBus.Fire<OnPurchaseProductSignal>(new OnPurchaseProductSignal()
        {
            productID = "noads"
        });
    }
    void BuyClothItem(ThemeCustomizationItem item,PriceType priceType)
    {
        SetBuyPanelData(item.name, item.rareness, item.descriptionText, item.newStatsText, item.sprite);
        buyButton.onClick.RemoveAllListeners();

        switch (priceType)
        {
         
            case PriceType.SC:
                priceText.text = item.SCPrice.ToString();
                coinImage.sprite = scCoin;

                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeSoftCurrency((ulong)item.SCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddCharacterItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenClothingPanel();

                    });
                }));

              
                break;
            case PriceType.HC:
                priceText.text = item.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong)item.HCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddCharacterItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenClothingPanel();

                    });
                }));
               
                break;
     
        }
    }
    void BuyRoomItem(ThemeCustomizationItem item,PriceType priceType)
    {
        SetBuyPanelData(item.name, item.rareness, item.descriptionText, item.newStatsText, item.sprite);
         buyButton.onClick.RemoveAllListeners();
        switch (priceType)
        {
         
            case PriceType.SC:
                priceText.text = item.SCPrice.ToString();
                coinImage.sprite = scCoin;

                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeSoftCurrency((ulong)item.SCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddRoomItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenFurniturePanel();

                    });
                }));

             
                break;
            case PriceType.HC:
                priceText.text = item.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong)item.HCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddRoomItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenFurniturePanel();


                    });
                }));

               
                break;
     
        }
    }
    void BuyVCItem(VideoQualityCustomizationItem item,PriceType priceType)
    {
        SetBuyPanelData(item.name, item.rareness, item.descriptionText, item.newStatsText, item.itemSprite);
        buyButton.onClick.RemoveAllListeners();
        switch (priceType)
        {
         
            case PriceType.SC:
                priceText.text = item.SCPrice.ToString();
                coinImage.sprite = scCoin;
                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeSoftCurrency((ulong)item.SCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddVCItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenEquipmentsPanel();

                    }); 
                }));

            
                break;
            case PriceType.HC:
                priceText.text = item.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener(() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong)item.HCPrice, () =>
                    {
                        item.Owned = true;
                        playerInventory.AddVCItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenEquipmentsPanel();


                    });
                });

               
                break;
     
        }
    }

    void SetBuyPanelData(string itemName,Rareness rareness, string description, string stats,Sprite icon )
    {
        buyPanel.gameObject.SetActive(true);
        nameText.text = itemName;
        rarenessText.text = rareness.ToString();
        descriptionText.text = description;
        statsText.text = stats;
        rarenessImage.sprite = GetRarenessSpriteByIndex(rareness);
        iconImage.sprite = icon;
        
    }

}
