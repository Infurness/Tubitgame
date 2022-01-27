using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UniRx.Triggers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CarsPopup_VC : MonoBehaviour
{
    [SerializeField] private Canvas carsCanvasPopUp;
    [SerializeField] private Button popupButton;
    [SerializeField] private GameObject carsButtonPanel;
    [SerializeField] private GameObject carsPanel;
    [Inject] private SignalBus signalBus;

    [Inject] private PlayerInventory playerInventory;
    [SerializeField] private CarInventoryButton carInventoryButton;
    [SerializeField] private GameObject buttonsRoot;
    [SerializeField] private Button selectButton;
    private Car selectedCar;
    void Start()
    {
   
        
        
        popupButton.onClick.AddListener((() =>
        {
            carsPanel.gameObject.SetActive(true);
         //   popupButton.gameObject.SetActive(false);
            PopulateCarsButtons();
        }));
       selectButton.onClick.AddListener((() =>
       {
           signalBus.Fire(new EquipCarSignal()
           {
               car = selectedCar
           });
       }));

       carsPanel.OnEnableAsObservable().Subscribe((s) =>
       {
           selectButton.interactable = false;

       });
       
       signalBus.Subscribe<RoomCustomizationVisibilityChanged>((signal) =>
       {
           popupButton.gameObject.SetActive(!signal.Visibility);
       });
       
       signalBus.Subscribe<CharacterCustomizationVisibilityChanged>((signal) =>
       {
           popupButton.gameObject.SetActive(!signal.Visibility);
       });
    }

    
    void PopulateCarsButtons()
    {
        for (int i = 0; i < buttonsRoot.transform.childCount; i++)
        {
            Destroy(buttonsRoot.transform.GetChild(i).gameObject);
        }
        var cars = playerInventory.OwnedCars;
        
        foreach (var car in cars)
        {
          var bt=  Instantiate(carInventoryButton, buttonsRoot.transform);
          bt.SetButtonData(car.carSprite,car.name.ToString(), () =>
          {
              selectedCar = car;
              selectButton.interactable = true;
          });
        }
    }
    void SetButtonVisibility(bool state)
    {
        print("Street View Changed "+state);
        carsButtonPanel.gameObject.SetActive(state);

    }

  
}
