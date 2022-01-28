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
        realEstateButton,
        vehiclesButton;
    [SerializeField] private GameObject itemsScrollView, offersButtonsScrollView, offerRedeemPanel;

    [SerializeField] private ShopItemSlot shopItemButton;

    //RealState
    [SerializeField] private GameObject realStateButton;
    [SerializeField] private GameObject realStateButtonsContainer;
    [SerializeField] private Image realEstateMap;
    [SerializeField] private Vector2[] housePositions;
    [SerializeField] private Button houseBuyButton;
    [SerializeField] private TMP_Text housePrice;
    [SerializeField] private TMP_Text housePriceMessage;
    [SerializeField] private Image housePriceCurrencyIcon;
    [SerializeField] private Sprite selectedHouseButtonSprite;
    [SerializeField] private Sprite unselectedHouseButtonSprite;
    [SerializeField] private Color selectedHouseTextColor;
    [SerializeField] private Color unselectedHouseTextColor;
    private List<GameObject> realEstateShopButtons = new List<GameObject>();
    //

    [SerializeField] private Sprite[] rarenessSprites;

    [SerializeField] private GameObject buyPanel;
    [SerializeField] private Image rarenessImage, iconImage, coinImage;
    [SerializeField] private TMP_Text descriptionText, statsText, nameText, rarenessText, priceText;

    [SerializeField] private GameObject realEstateBuyPanel;
    [SerializeField] private Image realEstateIconImage, realEstateCoinImage;
    [SerializeField] private TMP_Text realEstateNameText, realEstatePriceText;

    [SerializeField] private Sprite hcCoin, scCoin;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button realEstateBuyButton;
    [SerializeField] private GameObject softCurrencyPanel, energyPanel;
    [SerializeField] private Button SCButton;
    [SerializeField] private Button HCButton;
    [SerializeField] private GameObject consumablesPanel;
    [SerializeField] private ConsumableInventoryButton consumableItemButtonPrefab;
    [Inject] private GameAnalyticsManager gameAnalyticsManager;
    
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
        vehiclesButton.onClick.AddListener(OpenVehiclesPanel);
        shopPanel.OnEnableAsObservable().Subscribe((unit =>
        {
            offersButton.onClick.Invoke();
            buyPanel.gameObject.SetActive(false);
            realEstateBuyPanel.gameObject.SetActive(false);
            _signalBus.Fire<ShopPanelOpened>();

        }));
        _signalBus.Subscribe<OpenRealEstateShopSignal>(OpenRealEstateFromSignal);
        _signalBus.Subscribe<OpenSCCurrenciesPanelSignal>(OpenCurrenciesPanel);
        _signalBus.Subscribe<OpenHCCurrenciesPanelSignal>(OpenCurrenciesPanel);
        _signalBus.Subscribe<OpenSCCurrenciesPanelSignal>(OpenSCPanel);
        _signalBus.Subscribe<OpenHCCurrenciesPanelSignal>(OpenHCPanel);
        consumablesPanel.OnEnableAsObservable().Subscribe((unit =>
        {
            PopulateSoftCurrencyBundles();
            PopulateEnergyItems();
        }));

        _signalBus.Subscribe<BuyHouseSignal>(UpdateHouseDisplay);

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
    void OpenSCPanel()
    {
        SCButton.onClick.Invoke();
    }
    void OpenHCPanel()
    {
        HCButton.onClick.Invoke();
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
                if (TutorialManager.Instance != null)
                {
                    if(item.SCPrice==0)
                    {
                        var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
                        switch (item.PriceType)
                        {
                            case PriceType.Free:
                                Destroy(shopButton.gameObject);

                                break;
                            case PriceType.SC:
                                shopButton.SetSCBuyButton(item.SCPrice, item.name, item.sprite,
                                    (() => BuyClothItem(item, PriceType.SC)));

                                break;
                            case PriceType.HC:
                                shopButton.SetHCBuyButton(item.HCPrice, item.name, item.sprite,
                                    () => BuyClothItem(item, PriceType.HC));
                                break;

                            case PriceType.Exchangeable:
                                shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                                    item.name, item.sprite,
                                    (() => BuyClothItem(item, PriceType.SC)),
                                    (() => BuyClothItem(item, PriceType.HC)));
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }   
                }
                else
                {
                    var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
                    switch (item.PriceType)
                    {
                        case PriceType.Free:
                            Destroy(shopButton.gameObject);

                            break;
                        case PriceType.SC:
                            shopButton.SetSCBuyButton(item.SCPrice, item.name, item.sprite,
                                (() => BuyClothItem(item, PriceType.SC)));

                            break;
                        case PriceType.HC:
                            shopButton.SetHCBuyButton(item.HCPrice, item.name, item.sprite,
                                () => BuyClothItem(item, PriceType.HC));
                            break;

                        case PriceType.Exchangeable:
                            shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                                item.name, item.sprite,
                                (() => BuyClothItem(item, PriceType.SC)),
                                (() => BuyClothItem(item, PriceType.HC)));
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
                if (TutorialManager.Instance != null)
                {
                    if (item.SCPrice == 0)
                    {
                        var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
                        switch (item.PriceType)
                        {
                            case PriceType.Free:
                                Destroy(shopButton.gameObject);

                                break;
                                return;
                            case PriceType.SC:
                                shopButton.SetSCBuyButton(item.SCPrice, item.name, item.sprite,
                                    (() => BuyRoomItem(item, PriceType.SC)));
                                break;
                            case PriceType.HC:
                                shopButton.SetHCBuyButton(item.HCPrice, item.name, item.sprite,
                                    () => BuyRoomItem(item, PriceType.HC));

                                break;
                            case PriceType.Exchangeable:
                                shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                                    item.name, item.sprite,
                                    (() => BuyRoomItem(item, PriceType.SC)),
                                    (() => BuyRoomItem(item, PriceType.HC)));

                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                else
                {
                    var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);
                    switch (item.PriceType)
                    {
                        case PriceType.Free:
                            Destroy(shopButton.gameObject);

                            break;
                            return;
                        case PriceType.SC:
                            shopButton.SetSCBuyButton(item.SCPrice, item.name, item.sprite,
                                (() => BuyRoomItem(item, PriceType.SC)));
                            break;
                        case PriceType.HC:
                            shopButton.SetHCBuyButton(item.HCPrice, item.name, item.sprite,
                                () => BuyRoomItem(item, PriceType.HC));

                            break;
                        case PriceType.Exchangeable:
                            shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                                item.name, item.sprite,
                                (() => BuyRoomItem(item, PriceType.SC)),
                                (() => BuyRoomItem(item, PriceType.HC)));

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
                        shopButton.SetSCBuyButton(item.SCPrice, item.name,
                            item.itemSprite,
                            (() => BuyVCItem(item, PriceType.SC)));

                        break;
                    case PriceType.HC:
                        shopButton.SetHCBuyButton(item.HCPrice, item.name,
                            item.itemSprite,
                            () => BuyVCItem(item, PriceType.HC));

                        break;
                    case PriceType.Exchangeable:
                        shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                            item.name, item.itemSprite,
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
        realEstateButton.gameObject.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 1000, 0);
        RealEstateCustomizationItem house = shop.Houses.Where((house) => house.name == signal.houseName).FirstOrDefault();
        SetHouseDisplay(house);
        switch (house.realEstateHouse)
        {
            case RealEstateHouse.BasicHouse:
                RealEstateButtonPressed(realEstateShopButtons[0]);
                break;
            case RealEstateHouse.ResidentialHouse:
                RealEstateButtonPressed(realEstateShopButtons[2]);
                break;
            case RealEstateHouse.HugeHouse:
                RealEstateButtonPressed(realEstateShopButtons[1]);
                break;
        }    
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
            shopButton.GetComponentInChildren<TMP_Text>().text = string.Concat(item.name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            shopButton.GetComponentInChildren<Button>().onClick.AddListener(() => SetHouseDisplay(item));
            shopButton.GetComponentInChildren<Button>().onClick.AddListener(() => RealEstateButtonPressed(shopButton));
            realEstateShopButtons.Add(shopButton);
        }
        SetHouseDisplay(playerInventory.OwnedRealEstateItems[0]);
        RealEstateButtonPressed(realEstateShopButtons[0]);
    }

    void OpenVehiclesPanel()
    {
        ClearItemsPanel();

        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);
        itemsPanel.gameObject.SetActive(true);
        currenciesPanel.SetActive(false);
        
        var cars = shop.Cars;
        ;
        foreach (var item in cars)
        {
            if (!item.Owned)
            {
                var shopButton = Instantiate(shopItemButton, itemsScrollView.transform);

                switch (item.priceType)
                {
                    case PriceType.Free:
                        Destroy(shopButton.gameObject);

                        break;
                    case PriceType.SC:
                        shopButton.SetSCBuyButton(item.SCPrice, item.name,
                            item.carSprite,
                            (() => BuyCar(item, PriceType.SC)));

                        break;
                    case PriceType.HC:
                        shopButton.SetHCBuyButton(item.HCPrice, item.name,
                            item.carSprite,
                            () => BuyCar(item, PriceType.HC));

                        break;
                    case PriceType.Exchangeable:
                        shopButton.SetBuyByBothButtons(item.HCPrice, item.SCPrice,
                            item.name, item.carSprite,
                            (() => BuyCar(item, PriceType.SC)),
                            (() => BuyCar(item, PriceType.HC)));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    void SetHouseDisplay(RealEstateCustomizationItem item)
    {
        bool ownedHouse = item.Owned || playerInventory.OwnedRealEstateItems.Find(house => house.itemName == item.itemName);

        MoveMap(item.realEstateHouse);

        if (ownedHouse)
        {
            housePriceMessage.text = "Owned";
            housePrice.text = "";
            Color transparent = Color.white;
            transparent.a = 0;
            housePriceCurrencyIcon.color = transparent;
            houseBuyButton.interactable = !ownedHouse;
        }     
        else
        {
            housePriceCurrencyIcon.color = Color.white;
            if(item.realEstateHouse == RealEstateHouse.HugeHouse)
            {
                Color transparent = Color.white;
                transparent.a = 0;
                housePriceCurrencyIcon.color = transparent;
                housePriceMessage.text = "Coming soon";
                housePrice.text = "";
                buyButton.onClick.RemoveAllListeners();
                houseBuyButton.interactable = false;
            }
            else
            {
                housePriceMessage.text = "";
                if (item.PriceType == PriceType.HC)
                {
                    housePriceCurrencyIcon.sprite = hcCoin;
                    housePrice.text = $"{item.HCPrice}";
                }
                else if (item.PriceType == PriceType.SC)
                {
                    housePriceCurrencyIcon.sprite = scCoin;
                    housePrice.text = $"{item.SCPrice}";
                }
                AspectRatioFitter fitter = housePriceCurrencyIcon.GetComponent<AspectRatioFitter>();
                fitter.aspectRatio = housePriceCurrencyIcon.sprite.rect.width / housePriceCurrencyIcon.sprite.rect.height;

                houseBuyButton.onClick.AddListener(() => BuyRealEstateItem(item, item.PriceType));
                houseBuyButton.interactable = !ownedHouse;
            }
          
        }
        
    }
    void MoveMap(RealEstateHouse realEstateHouse)
    {
        Vector2 newPos = housePositions[0];
        switch (realEstateHouse)
        {
            case RealEstateHouse.BasicHouse:
                newPos = housePositions[0];
                break;
            case RealEstateHouse.ResidentialHouse:
                newPos = housePositions[1];
                break;
            case RealEstateHouse.HugeHouse:
                newPos = housePositions[2];
                break;
        }
        StartCoroutine(MoveMapSmooth(newPos));
    }
    System.Collections.IEnumerator MoveMapSmooth(Vector2 newPos)
    {
        Vector2 initialPos = realEstateMap.GetComponent<RectTransform>().localPosition;
        float lerp = 0;
        while (lerp < 1)
        {
            lerp += Time.deltaTime*3;
            realEstateMap.GetComponent<RectTransform>().localPosition = Vector2.Lerp(initialPos, newPos, lerp);
            yield return null;
        }
        realEstateMap.GetComponent<RectTransform>().localPosition = newPos;
    }
    void RealEstateButtonPressed(GameObject button)
    {
        foreach(GameObject shopbutton in realEstateShopButtons)
        {
            if(button != shopbutton)
            {
                shopbutton.transform.GetChild(0).GetComponent<Image>().sprite = unselectedHouseButtonSprite;
                shopbutton.GetComponentInChildren<TMP_Text>().color = unselectedHouseTextColor;
            }
            else
            {
                shopbutton.transform.GetChild(0).GetComponent<Image>().sprite = selectedHouseButtonSprite;
                shopbutton.GetComponentInChildren<TMP_Text>().color = selectedHouseTextColor;
            }
        }
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
                    gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name, PriceType.SC,item.SCPrice});

                    playerDataManager.ConsumeSoftCurrency((ulong) item.SCPrice, () =>
                    {
                        item.Owned = true;

                        playerInventory.AddCharacterItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
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
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name,PriceType.HC,item.HCPrice});

                        playerInventory.AddCharacterItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
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
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name,PriceType.SC,item.SCPrice});

                        playerInventory.AddRoomItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
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
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name,PriceType.HC,item.HCPrice});

                        playerInventory.AddRoomItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
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
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name,PriceType.SC,item.SCPrice});


                        playerInventory.AddVCItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
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
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name,PriceType.HC,item.HCPrice});

                        playerInventory.AddVCItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
                        OpenEquipmentsPanel();

                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                });


                break;

        }
    }

    void BuyRealEstateItem(RealEstateCustomizationItem item, PriceType priceType)
    {
        SetRealStateBuyPanelData(item.name, item.streetViewSprite);

        realEstateBuyButton.onClick.RemoveAllListeners();
        switch (priceType)
        {
            case PriceType.SC:
                realEstatePriceText.text = item.SCPrice.ToString();
                realEstateCoinImage.sprite = scCoin;
                realEstateBuyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeSoftCurrency((ulong) item.SCPrice, () =>
                    {
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name,PriceType.SC,item.SCPrice});

                        playerInventory.AddRealEstateItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
                        OpenRealEstatePanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                }));
                break;

            case PriceType.HC:
                realEstatePriceText.text = item.HCPrice.ToString();
                realEstateCoinImage.sprite = hcCoin;

                realEstateBuyButton.onClick.AddListener(() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong) item.HCPrice, () =>
                    {
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{item.name,PriceType.HC,item.HCPrice});

                        playerInventory.AddRealEstateItem(item);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
                        OpenRealEstatePanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                });
                break;

        }
    }

    void BuyCar(Car car,PriceType priceType)
    {
        SetBuyPanelData(car.name, car.rareness, "", "", car.carSprite);
        buyButton.onClick.RemoveAllListeners();
        switch (priceType)
        {

            case PriceType.SC:
                priceText.text = car.SCPrice.ToString();
                coinImage.sprite = scCoin;
                buyButton.onClick.AddListener((() =>
                {
                    playerDataManager.ConsumeSoftCurrency((ulong) car.SCPrice, () =>
                    {
                        car.Owned = true;
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{car.name,PriceType.SC,car.SCPrice});

                        playerInventory.AddCar(car);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
                        OpenVehiclesPanel();
                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                }));


                break;
            case PriceType.HC:
                priceText.text = car.HCPrice.ToString();
                coinImage.sprite = hcCoin;

                buyButton.onClick.AddListener(() =>
                {
                    playerDataManager.ConsumeHardCurrency((ulong) car.HCPrice, () =>
                    {
                        car.Owned = true;
                        gameAnalyticsManager.SendCustomEvent("purchase_item",new object[]{car.name,PriceType.HC,car.HCPrice});

                        playerInventory.AddCar(car);
                        buyPanel.gameObject.SetActive(false);
                        realEstateBuyPanel.gameObject.SetActive(false);
                        OpenVehiclesPanel();

                        _signalBus.Fire<BuyItemSoundSignal>();
                    });
                });


                break;

        }
    }

    void SetBuyPanelData(string itemName, Rareness rareness, string description, string stats, Sprite icon)
    {
        buyPanel.gameObject.SetActive(true);
        nameText.text = string.Concat(itemName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        nameText.enableAutoSizing = true;
        rarenessText.text = rareness.ToString();
        descriptionText.text = description;
        statsText.text = stats;
        rarenessImage.sprite = GetRarenessSpriteByIndex(rareness);
        iconImage.sprite = icon;

    }
    void SetRealStateBuyPanelData(string itemName, Sprite icon)
    {
        realEstateBuyPanel.gameObject.SetActive(true);
        realEstateNameText.text = string.Concat(itemName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        realEstateIconImage.sprite = icon;
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
            },item.priceType);
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
                playerDataManager.ConsumeSoftCurrency((uint)item.price, () =>
                {
                    energyInventoryManager.AddItem(item.specialID,item.amount);

                });
            },item.priceType);
        }
    }

}