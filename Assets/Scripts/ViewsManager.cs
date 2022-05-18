using Customizations;
using UnityEngine;
using Zenject;

public class ViewsManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer houseCloseView;
    [SerializeField] private SpriteRenderer houseStreetView;
    [SerializeField] private SpriteRenderer carSpriteRendererStreetView;
    [SerializeField] private SpriteRenderer carSpriteRendererNeighbourhoodView;
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
            EquipCar(signal.car);
        });
        EquipCar(playerInventory.EquippedCar);   
    }
    
    void EquipCar(Car car)
    {
        if (car)
        {
            carSpriteRendererStreetView.sprite = car.carSprite;
            carSpriteRendererNeighbourhoodView.sprite = car.carSprite;
            playerInventory.SetEquippedCar(car);
            SetCar();
        }
    }

    void EquipHouse(RealEstateCustomizationItem realEstateCustomizationItem)
    {
        houseCloseView.sprite = realEstateCustomizationItem.houseCloseSpite;
        houseStreetView.sprite = realEstateCustomizationItem.streetViewSprite;
        carSpriteRendererStreetView.transform.localPosition = realEstateCustomizationItem.garagePosition;
        playerInventory.SetEquippedHouse(realEstateCustomizationItem);
        leftWall.sprite = realEstateCustomizationItem.leftWall;
        leftWall.transform.localPosition = realEstateCustomizationItem.leftWallPosition;
        
        rightWall.sprite = realEstateCustomizationItem.rightWall;
        rightWall.transform.localPosition = realEstateCustomizationItem.rightWallPosition;
        
        floor.sprite = realEstateCustomizationItem.floor;
        floor.transform.localPosition = realEstateCustomizationItem.floorPosition;
        
        SetCar();
    }    
    
    private void SetCar()
    {
        var hasGarageSlot = playerInventory.EquippedHouse.garageSlots > 0;
        carSpriteRendererStreetView.gameObject.SetActive(hasGarageSlot);
        carSpriteRendererNeighbourhoodView.gameObject.SetActive(hasGarageSlot);
    }
}
