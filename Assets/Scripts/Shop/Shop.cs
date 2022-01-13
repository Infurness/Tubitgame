using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class Shop : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    public List<ThemeCustomizationItem> Clothes => clothes;

    public List<ThemeCustomizationItem> Furniture => furniture;

    public List<VideoQualityCustomizationItem> Equipments => equipments;

    public List<RealEstateCustomizationItem> Houses => houses;
    public List<ConsumableItem> ConsumableItems => consumableItems;

    public List<Car> Cars => cars;


    [SerializeField] private List<ThemeCustomizationItem> clothes;
    [SerializeField] private List<ThemeCustomizationItem> furniture;
    [SerializeField] private List<VideoQualityCustomizationItem> equipments;
    [SerializeField] private List<RealEstateCustomizationItem> houses;
    [SerializeField] private List<ConsumableItem> consumableItems;
    [SerializeField] private List<Car> cars;

    void Start()
    {
        LoadEquipments();
        LoadClothes();
        LoadFurniture();
        LoadHouses();
        LoadConsumablesItems();
        LoadCars();
        
    }
    
    async void  LoadEquipments()
    {
        var loadOP = Addressables.LoadAssetsAsync<VideoQualityCustomizationItem>(key: "roomvc",null);
        await loadOP.Task;
        var roomVCITems =(List<VideoQualityCustomizationItem>) loadOP.Result;

        equipments = roomVCITems.FindAll((vc) => vc.Owned == false);
    }

    async void LoadFurniture()
    {
        var loadOP = Addressables.LoadAssetsAsync<ThemeCustomizationItem>(key: "furniture",null);
        await loadOP.Task;
        var roomVCITems =(List<ThemeCustomizationItem>) loadOP.Result;

        furniture = roomVCITems.FindAll((vc) => vc.Owned == false);
    }

    async void LoadClothes()
    {
        var loadOP = Addressables.LoadAssetsAsync<ThemeCustomizationItem>(key: "cloth",null);
        await loadOP.Task;
        var roomVCITems =(List<ThemeCustomizationItem>) loadOP.Result;

        clothes = roomVCITems.FindAll((vc) => vc.Owned == false);
    }
    async void LoadHouses()
    {
        var loadOP = Addressables.LoadAssetsAsync<RealEstateCustomizationItem>("house", null);
        await loadOP.Task;
        houses = (List<RealEstateCustomizationItem>)loadOP.Result;
    }

    async void LoadConsumablesItems()
    {
        var loadOP = Addressables.LoadAssetsAsync<ConsumableItem>("consumable", null);
        await loadOP.Task;
        consumableItems =(List<ConsumableItem>) loadOP.Result;
    }

    async void LoadCars()
    {
        var loadOp = Addressables.LoadAssetsAsync<Car>("car", null);
        await loadOp.Task;
        cars = (List<Car>) loadOp.Result;
    }
}
