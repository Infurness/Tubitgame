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

    [SerializeField] private List<ThemeCustomizationItem> clothes;
    [SerializeField] private List<ThemeCustomizationItem> furniture;
    [SerializeField] private List<VideoQualityCustomizationItem> equipments;
   
    void Start()
    {
        LoadEquipments();
        LoadClothes();
        LoadFurniture();
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
