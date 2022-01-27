using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class EquipHouseTriggerButton : MonoBehaviour,IPointerClickHandler
    {
        [SerializeField] private string houseName;
        [Inject] private PlayerInventory playerInventory;
        [Inject] private SignalBus signalBus;
        private SpriteRenderer spriteRenderer;


        private void OnBecameVisible()
        {
         
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (playerInventory.OwnedRealEstateItems.Exists((item => item.name==houseName)))
            {
                print(houseName+ "House Visable");

                spriteRenderer.sortingOrder = 50;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            print(houseName+" Equipped");
            
            var houseItem = playerInventory.OwnedRealEstateItems.Find(item => item.name == houseName);
            if (houseItem)
            {
             
                    signalBus.Fire(new HousePopUp()
                    {
                        realEstateCustomizationItem = houseItem
                    });   
                
            }
           
        }

        private void OnMouseDown()
        {
            print(houseName+" Equipped");
            
            var houseItem = playerInventory.OwnedRealEstateItems.Find(item => item.name == houseName);
            if (houseItem)
            {
             
                signalBus.Fire(new HousePopUp()
                {
                    realEstateCustomizationItem = houseItem
                });   
                
            }
        }
    }
