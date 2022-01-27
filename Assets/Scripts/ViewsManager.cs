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
        [SerializeField] private SpriteRenderer carSpriteRenderer;
        [SerializeField] private SpriteRenderer rightWall, leftWall, floor;
        [Inject] private PlayerInventory playerInventory;
        [Inject] private SignalBus signalBus;
        private void Start()
        {
            EquipHouse(playerInventory.EquippedHouse);
            signalBus.Subscribe<HouseChangedSignal>(signal =>
            {
                EquipHouse(signal.realEstateCustomizationItem);
            });
         
    
            
            signalBus.Subscribe<EquipCarSignal>((signal) =>
            {
                EqipCar(signal.car);
            });
            EqipCar(playerInventory.EquippedCar);
            
            
        }
        

        void EqipCar(Car car)
        {
            if (car)
            {
                carSpriteRenderer.sprite = car.carSprite;
                playerInventory.SetEquippedCar(car);
                if (playerInventory.EquippedHouse.garageSlots>0)
                {
                    carSpriteRenderer.gameObject.SetActive(true); 
                }
                else
                {
                    carSpriteRenderer.gameObject.SetActive(false); 
                }
               
            }
       
        }
        void EquipHouse(RealEstateCustomizationItem realEstateCustomizationItem)
        {
            houseCloseView.sprite = realEstateCustomizationItem.houseCloseSpite;
            houseStreetView.sprite = realEstateCustomizationItem.streetViewSprite;
            carSpriteRenderer.transform.localPosition = realEstateCustomizationItem.garagePosition;
            playerInventory.SetEquippedHouse(realEstateCustomizationItem);
            leftWall.sprite = realEstateCustomizationItem.leftWall;
            leftWall.transform.localPosition = realEstateCustomizationItem.leftWallPosition;
            
            rightWall.sprite = realEstateCustomizationItem.rightWall;
            rightWall.transform.localPosition = realEstateCustomizationItem.rightWallPosition;
            
            floor.sprite = realEstateCustomizationItem.floor;
            floor.transform.localPosition = realEstateCustomizationItem.floorPosition;
            if (playerInventory.EquippedHouse.garageSlots>0)
            {
                carSpriteRenderer.gameObject.SetActive(true); 
               
            }
            else
            {
                carSpriteRenderer.gameObject.SetActive(false); 
            }
        }
        
        
    }
