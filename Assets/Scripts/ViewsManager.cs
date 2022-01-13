﻿using System;
using Customizations;
using UniRx.Triggers;
using UniRx;
using UnityEngine;
using Zenject;

    public class ViewsManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer houseCloseView;
        [SerializeField] private SpriteRenderer houseStreetView;
        [SerializeField] private SpriteRenderer carSpriteRenderer;
        [Inject] private PlayerInventory playerInventory;
        [Inject] private SignalBus signalBus;
        private void Start()
        {
            EquipHouse(playerInventory.EquippedHouse);
            signalBus.Subscribe<HouseChangedSignal>(signal =>
            {
                EquipHouse(signal.realEstateCustomizationItem);
            });
            houseStreetView.gameObject.OnBecameVisibleAsObservable()
                .Subscribe((s) => signalBus.Fire(new SetCarsCanvasButtonVisibility()
                {
                    visibility =true
                }));
            houseStreetView.gameObject.OnBecameInvisibleAsObservable().
                Subscribe((s) => signalBus.Fire(new SetCarsCanvasButtonVisibility()
                    {
                        visibility =false
                    }));
            signalBus.Subscribe<EquipCarSignal>((signal) =>
            {
                EqipCar(signal.car);
            });
            EqipCar(playerInventory.EquippedCar);
        }

        void EqipCar(Car car)
        {
            carSpriteRenderer.sprite = car.carSprite;
            playerInventory.SetEquippedCar(car);
        }
        void EquipHouse(RealEstateCustomizationItem realEstateCustomizationItem)
        {
            houseCloseView.sprite = realEstateCustomizationItem.houseCloseSpite;
            houseStreetView.sprite = realEstateCustomizationItem.streetViewSprite;
            carSpriteRenderer.transform.localPosition = realEstateCustomizationItem.garagePosition;
            playerInventory.SetEquippedHouse(realEstateCustomizationItem);

        }
        
        
    }
