using System;
using System.Linq;
using System.Collections.Generic;
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
    [Inject] private EnergyInventoryManager energyInventoryManager;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject itemsPanel, offersPanel, realEstatePanel, currenciesPanel;

    [SerializeField] private Button offersButton,
        clothingButton,
        furnitureButton,
        equipmentsButton,
        currenciesButton,
        realEstateButton;

    [SerializeField] private GameObject itemsScrollView, offersButtonsScrollView, offerRedeemPanel;

    [SerializeField] private ShopItemSlot shopItemButton;

    //RealState
    [SerializeField] private GameObject realStateButton;
    [SerializeField] private GameObject realStateButtonsContainer;
    [SerializeField] private Image houseImage;
    [SerializeField] private TMP_Text roomSlots;
    [SerializeField] private TMP_Text garageSlots;
    [SerializeField] private Button houseBuyButton;
    [SerializeField] private TMP_Text housePrice;

    [SerializeField] private Sprite[] rarenessSprites;
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private Image rarenessImage, iconImage, coinImage;
    [SerializeField] private TMP_Text descriptionText, statsText, nameText, rarenessText, priceText;
    [SerializeField] private Sprite hcCoin, scCoin;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject softCurrencyPanel, energyPanel;
    [SerializeField] private GameObject consumablesPanel;
    [SerializeField] private ConsumableInventoryButton consumableItemButtonPrefab;
    
    
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
        _signalBus.Subscribe<OpenRealEstateShopSignal>(OpenRealEstateFromSignal);

        consumablesPanel.OnEnableAsObservable().Subscribe((unit =>
        {
            PopulateSoftCurrencyBundles();
            PopulateEnergyItems();
        }));

        _signalBus.Subscribe<BuyHouseSignal>(UpdateHouseDisplay);

        SetHouseDisplay(playerInventory.OwnedRealEstateItems[0]);
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
                        shopButton.SetSCBuyButton(item.SCPrice, GetRarenessSpriteByIndex(item.rareness),
                            item.itemSprite,
                            (() => BuyVCItem(item, PriceType.SC)));

                        break;
                    case PriceType.HC:
                        shopButton.SetHCBuyButton(item.HCPrice, GetRarenessSpriteByIndex(item.rareness),
                            item.itemSprite,
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

    void OpenRealEstateFromSignal(OpenRealEstateShopSignal signal)
    {
        OpenRealEstatePanel();
        realEstateButton.GetComponent<ShopCategoryButton>().SetButtonSelected();
        realEstateButton.gameObject.transform.parent.GetComponent<RectTransform>().anchoredPosition =
            new Vector3(0, 1000, 0);
        RealEstateCustomizationItem house = shop.Houses.Where((house) => house.name == signal.houseName).FirstOrDefault();
        SetHouseDisplay(house);
    }

    void OpenRealEstatePanel()
    {
        itemsPanel.gameObject.SetActive(false);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(true);
        if (realStateButtonsContainer.transform.childCount > 1)
            return;
        List<RealEstateCustomizationItem> houses = shop.Houses;

        foreach (RealEstateCustomizationItem item in houses)
        {
            GameObject shopButton = Instantiate(realStateButton, realStateButtonsContainer.transform);
            shopButton.GetComponentInChildren<TMP_Text>().text = item.name;            
            shopButton.GetComponentInChildren<Button>().onClick.AddListener(() => SetHouseDisplay(item));          
        }
    }

    void SetHouseDisplay(RealEstateCustomizationItem item)
    {
        bool ownedHouse = item.Owned || playerInventory.OwnedRealEstateItems.Find(house => house.itemName == item.itemName);

        houseImage.sprite = item.streetViewSprite;
        garageSlots.text = $"Garage slots: {item.garageSlots}";
        roomSlots.text = $"Room slots: {item.roomSlots}";
        if (ownedHouse)
            housePrice.text = "Owned";
        else
        {
            housePrice.text = $"{item.SCPrice}";
            houseBuyButton.onClick.AddListener(() => BuyRealEstateItem(item, item.PriceType));
        }
        houseBuyButton.interactable = !ownedHouse;
    }

    void UpdateHouseDisplay(BuyHouseSignal signal)
    {
        RealEstateCustomizationItem house = shop.Houses.Where((house) => house.name == signal.houseName).FirstOrDefault();
        SetHouseDisplay(house);
    }

    public void BuyHCBundle(string id)
    {

        _signalBus.Fire<OnPurchaseProductSignal>(new OnPurchaseProductSignal()
        {
            productID = id
        });
        _signalBus.Fire<BuyItemSoundSignal>();
    }

    public void BuyNoAds()
    {
        _signalBus.Fire<OnPurchaseProductSignal>(new OnPurchaseProductSignal()
        {
            productID = "noads"
        });
        _signalBus.Fire<BuyItemSoundSignal>();
    }

    void BuyClothItem(ThemeCustomizationItem item, PriceType priceType)
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
                    playerDataManager.ConsumeSoftCurrency((ulong) item.SCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddCharacterItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenClothingPanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                }));


                break;
            case PriceType.HC:
                priceText.text = item.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong) item.HCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddCharacterItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenClothingPanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                }));

                break;

        }
    }

    void BuyRoomItem(ThemeCustomizationItem item, PriceType priceType)
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
                    playerDataManager.ConsumeSoftCurrency((ulong) item.SCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddRoomItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenFurniturePanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                }));


                break;
            case PriceType.HC:
                priceText.text = item.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong) item.HCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddRoomItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenFurniturePanel();
                        _signalBus.Fire<BuyItemSoundSignal>();

                    });
                }));


                break;

        }
    }

    void BuyVCItem(VideoQualityCustomizationItem item, PriceType priceType)
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
                    playerDataManager.ConsumeSoftCurrency((ulong) item.SCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddVCItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenEquipmentsPanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                }));


                break;
            case PriceType.HC:
                priceText.text = item.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener(() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong) item.HCPrice, () =>
                    {
                        item.Owned = true;
                        playerInventory.AddVCItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenEquipmentsPanel();

                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                });


                break;

        }
    }

    void BuyRealEstateItem(RealEstateCustomizationItem item, PriceType priceType)
    {
        SetBuyPanelData(item.name, item.rareness, "", $"Room Slots: {item.roomSlots}\nGarage Slots: {item.garageSlots}",
            item.streetViewSprite);
        buyButton.onClick.RemoveAllListeners();
        switch (priceType)
        {
            case PriceType.SC:
                priceText.text = item.SCPrice.ToString();
                coinImage.sprite = scCoin;
                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeSoftCurrency((ulong) item.SCPrice, () =>
                    {
                        playerInventory.AddRealEstateItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenRealEstatePanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                }));
                break;

            case PriceType.HC:
                priceText.text = item.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener(() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong) item.HCPrice, () =>
                    {
                        playerInventory.AddRealEstateItem(item);
                        buyPanel.gameObject.SetActive(false);
                        OpenRealEstatePanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                });
                break;

        }
    }

    void SetBuyPanelData(string itemName, Rareness rareness, string description, string stats, Sprite icon)
    {
        buyPanel.gameObject.SetActive(true);
        nameText.text = itemName;
        rarenessText.text = rareness.ToString();
        descriptionText.text = description;
        statsText.text = stats;
        rarenessImage.sprite = GetRarenessSpriteByIndex(rareness);
        iconImage.sprite = icon;

    }



    public void PopulateSoftCurrencyBundles()
    {
        for (int i = 0; i < softCurrencyPanel.transform.childCount; i++)
        {
            Destroy(softCurrencyPanel.transform.GetChild(i).gameObject);
        }
        var items = shop.ConsumableItems.FindAll(it => it.type == ConsumableItem.ConsumableType.SoftCurrency);
        foreach (var item in items)
        {
            var bt = Instantiate(consumableItemButtonPrefab, softCurrencyPanel.transform);
            bt.SetButtonData(item.sprite,item.name,item.price,item.amount, () =>
            {
                playerDataManager.ConsumeHardCurrency((uint)item.price,(() =>
                {
                    playerDataManager.AddSoftCurrency((uint)item.amount);

                }));
            });
        }
    }

    public void PopulateEnergyItems()
    {
        for (int i = 0; i < energyPanel.transform.childCount; i++)
        {
            Destroy(energyPanel.transform.GetChild(i).gameObject);
        }
        var items = shop.ConsumableItems.FindAll(it => it.type == ConsumableItem.ConsumableType.Energy);
        foreach (var item in items)
        {
            var bt = Instantiate(consumableItemButtonPrefab, energyPanel.transform);
            bt.SetButtonData(item.sprite,item.name,item.price,item.amount, () =>
            {
              energyInventoryManager.AddItem(item.specialID,item.amount);
            });
        }
    }

}