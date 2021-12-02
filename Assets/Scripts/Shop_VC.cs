using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using  UniRx;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Zenject;

public class Shop_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject itemsPanel,offersPanel,realEstatePanel;
    [SerializeField] private Button offersButton, clothingButton, furnitureButton, equipmentsButton, realEstateButton;
    [SerializeField] private GameObject itemsScrollView, offersButtonsScrollView, offerRedeemPanel;
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

    }

    void OpenFurniturePanel()
    {
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);

    }

    void OpenEquipmentsPanel()
    {
        itemsPanel.gameObject.SetActive(true);
        offersPanel.gameObject.SetActive(false);
        realEstatePanel.gameObject.SetActive(false);

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
  
}
