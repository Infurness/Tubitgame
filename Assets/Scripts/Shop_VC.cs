using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
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
    [SerializeField] private GameObject itemsPanel,offersPanel,realEstatePanel;
    [SerializeField] private Button offersButton, clothingButton, furnitureButton, equipmentsButton, realEstateButton;
    [SerializeField] private GameObject itemsScrollView, offersButtonsScrollView, offerRedeemPanel;
    [SerializeField] private ShopItemSlot shopItemButton;
    [SerializeField] private Sprite[] rarenessSprites;

    void Start()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(false);
        offersButton.onClick.AddListener(OpenOffersPanel);
        clothingButton.onClick.AddListener(OpenClothingPanel);
        furnitureButton.onClick.AddListener(OpenFurniturePanel);
        equipmentsButton.onClick.AddListener(OpenEquipmentsPanel);
        realEstateButton.onClick.AddListener(OpenRealEstatePanel);
        shopPanel.OnEnableAsObservable().Subscribe((unit => offersButton.onClick.Invoke()));
    }

    Sprite GetRarenessSpriteByIndex(Rareness rareness)
    {
        return rarenessSprites[(int) rareness];
    }
    void OpenOffersPanel()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(true);
        realEstatePanel.gameObject.SetActive(false);
        
        

    }

    void OpenClothingPanel()
    {
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);

        var clothes = shop.Clothes;

        foreach (var item in clothes)
        {
          var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
          switch (item.PriceType)
          {
              case PriceType.Free:
                  break;
              case PriceType.SC:
                  shopButton.SetSCBuyButton(item.SCPrice,GetRarenessSpriteByIndex(item.rareness),item.sprite,(() => BuyClothItem(item,PriceType.SC)));
                  
                  break;
              case PriceType.HC:
                  shopButton.SetHCBuyButton(item.HCPrice,GetRarenessSpriteByIndex(item.rareness),item.sprite,()=>BuyClothItem(item,PriceType.HC));
                  break;
              case PriceType.Exchangeable:
                  shopButton.SetBuyByBothButtons(item.HCPrice,item.SCPrice,GetRarenessSpriteByIndex(item.rareness),item.sprite,(() => BuyClothItem(item,PriceType.SC)),
                      (() => BuyClothItem(item,PriceType.HC)));
                  break;
              default:
                  throw new ArgumentOutOfRangeException();
          }
        }

    }

   

    void OpenFurniturePanel()
    {
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);
        var furniture = shop.Furniture;

        foreach (var item in furniture)
        {
            var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
            switch (item.PriceType)
            {
                case PriceType.Free:
                    break;
                case PriceType.SC:
                    shopButton.SetSCBuyButton(item.SCPrice,GetRarenessSpriteByIndex(item.rareness),item.sprite,(() => BuyRoomItem(item,PriceType.SC)));
                  
                    break;
                case PriceType.HC:
                    shopButton.SetHCBuyButton(item.HCPrice,GetRarenessSpriteByIndex(item.rareness),item.sprite,()=>BuyRoomItem(item,PriceType.HC));
                    break;
                case PriceType.Exchangeable:
                    shopButton.SetBuyByBothButtons(item.HCPrice,item.SCPrice,GetRarenessSpriteByIndex(item.rareness),item.sprite,(() => BuyRoomItem(item,PriceType.SC)),
                        (() => BuyRoomItem(item,PriceType.HC)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    void OpenEquipmentsPanel()
    {
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);
        var equipments = shop.Equipments;

        foreach (var item in equipments)
        {
            var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
            switch (item.PriceType)
            {
                case PriceType.Free:
                    break;
                case PriceType.SC:
                    shopButton.SetSCBuyButton(item.SCPrice,GetRarenessSpriteByIndex(item.rareness),item.itemSprite,(() => BuyVCItem(item,PriceType.SC)));
                  
                    break;
                case PriceType.HC:
                    shopButton.SetHCBuyButton(item.HCPrice,GetRarenessSpriteByIndex(item.rareness),item.itemSprite,()=>BuyVCItem(item,PriceType.HC));
                    break;
                case PriceType.Exchangeable:
                    shopButton.SetBuyByBothButtons(item.HCPrice,item.SCPrice,GetRarenessSpriteByIndex(item.rareness),item.itemSprite,(() => BuyVCItem(item,PriceType.SC)),
                        (() => BuyVCItem(item,PriceType.HC)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    void OpenRealEstatePanel()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(false);
       realEstatePanel.gameObject.SetActive(true);
    }
    
    public void OnBuyProductPressed(string id)
    {
        
        _signalBus.Fire<OnPurchaseProductSignal>(new OnPurchaseProductSignal()
        {
            productID = id
        });
    }
    void BuyClothItem(ThemeCustomizationItem item,PriceType priceType)
    {
        switch (priceType)
        {
         
            case PriceType.SC:
                playerDataManager.ConsumeSoftCurrency(item.SCPrice, () =>
                {
                    playerInventory.AddCharacterItem(item);
                });
                break;
            case PriceType.HC:
                playerDataManager.ConsumeHardCurrency(item.SCPrice, () =>
                {
                    playerInventory.AddCharacterItem(item);
                });
                break;
     
        }
    }
    void BuyRoomItem(ThemeCustomizationItem item,PriceType priceType)
    {
        switch (priceType)
        {
         
            case PriceType.SC:
                playerDataManager.ConsumeSoftCurrency(item.SCPrice, () =>
                {
                    playerInventory.AddRoomItem(item);
                });
                break;
            case PriceType.HC:
                playerDataManager.ConsumeHardCurrency(item.SCPrice, () =>
                {
                    playerInventory.AddRoomItem(item);
                });
                break;
     
        }
    }
    void BuyVCItem(VideoQualityCustomizationItem item,PriceType priceType)
    {
        switch (priceType)
        {
         
            case PriceType.SC:
                playerDataManager.ConsumeSoftCurrency(item.SCPrice, () =>
                {
                    playerInventory.AddVCItem(item);
                });
                break;
            case PriceType.HC:
                playerDataManager.ConsumeHardCurrency(item.SCPrice, () =>
                {
                    playerInventory.AddVCItem(item);
                });
                break;
     
        }
    }

}
