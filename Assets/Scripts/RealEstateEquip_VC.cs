using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


    public class RealEstateEquip_VC : MonoBehaviour
    {
        [SerializeField] private Canvas realEstateUICanvas;
        [SerializeField] private TMP_Text roomSlotsText;
        [SerializeField] private TMP_Text garageSlotsText;
        [SerializeField] private TMP_Text houseName;
        [SerializeField] private Image houseViewImage;
        [SerializeField] private Button moveButton;
        [Inject] private SignalBus signalBus;
        private void Start()
        {
            signalBus.Subscribe<HousePopUp>(ShowEquipHousePopUp);
        }

        
         void ShowEquipHousePopUp(HousePopUp housePopUp)
        {
            realEstateUICanvas.gameObject.SetActive(true);
            roomSlotsText.text = housePopUp.realEstateCustomizationItem.roomSlots.ToString();
            houseName.text = housePopUp.realEstateCustomizationItem.itemName;
            houseViewImage.sprite = housePopUp.realEstateCustomizationItem.streetViewSprite;
            garageSlotsText.text = housePopUp.realEstateCustomizationItem.garageSlots.ToString();
            moveButton.onClick.AddListener((() =>
            {
                signalBus.Fire(new HouseChangedSignal()
                {
                    realEstateCustomizationItem = housePopUp.realEstateCustomizationItem
                });
                realEstateUICanvas.gameObject.SetActive(false);
            }));
        }
        
    }
