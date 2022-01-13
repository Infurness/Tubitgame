using System;
using Customizations;
using UniRx.Triggers;
using UniRx;
using UnityEngine;
using Zenject;

    public class ViewsManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer houseCloseView;
        [SerializeField] private SpriteRenderer houseStreetView;
        [Inject] private PlayerInventory playerInventory;
        [Inject] private SignalBus signalBus;
        private void Start()
        {
            EquipHouse(playerInventory.EquippedHouse);
            signalBus.Subscribe<HouseChangedSignal>(signal =>
            {
                EquipHouse(signal.realEstateCustomizationItem);
            });
          //  houseStreetView.gameObject.OnBecameVisibleAsObservable().Subscribe()
        }

        
        void EquipHouse(RealEstateCustomizationItem realEstateCustomizationItem)
        {
            houseCloseView.sprite = realEstateCustomizationItem.houseCloseSpite;
            houseStreetView.sprite = realEstateCustomizationItem.streetViewSprite;
            playerInventory.SetEquippedHouse(realEstateCustomizationItem);

        }
        
        
    }
