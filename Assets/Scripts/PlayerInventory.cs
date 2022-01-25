using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    [Inject] private PlayerDataManager playerDataManager;
    [Inject] private SignalBus signalBus;
    [SerializeField] private PlayerInventoryAddressedData playerInventoryAddressedData;
    [SerializeField] private CharacterAvatarAddressedData characterAvatarAddressedData;
    [SerializeField] private CharacterAvatar equippedCharacterAvatar;
    [SerializeField] private CharacterAvatar defaultMaleAvatar;
    [SerializeField] private HeadItem defaultFemaleHead, defaultMaleHead;
    [SerializeField] private List<ThemeCustomizationItem> ownedCharacterItems;
    [SerializeField] private List<ThemeCustomizationItem> ownedRoomThemeEffectItems;
    [SerializeField] private List<ThemeCustomizationItem> equippedThemeEffectRoomItems;
    [SerializeField] private List<VideoQualityCustomizationItem> ownedVideoQualityRoomItems;
    [SerializeField] private List<VideoQualityCustomizationItem> equippedVideoQualityRoomItems;
    [SerializeField] private List<RealEstateCustomizationItem> ownedRealEstateItems;



    [SerializeField] private List<Car> ownedCars;
    private Car equippedCar;
    [SerializeField] private RealEstateCustomizationItem equippedHouse;
    [SerializeField] private RealEstateCustomizationItem defaultHouse;
    [SerializeField] private RoomLayout defaultRoomLayout;
     private GameAnalyticsManager gameAnalyticsManager;
    public List<ThemeCustomizationItem> OwnedCharacterItems => ownedCharacterItems.ToList();

    public List<ThemeCustomizationItem> OwnedRoomThemeEffectItems => ownedRoomThemeEffectItems.ToList();

    public List<ThemeCustomizationItem> EquippedThemeEffectRoomItems => equippedThemeEffectRoomItems.ToList();

    public List<VideoQualityCustomizationItem> OwnedVideoQualityRoomItems => ownedVideoQualityRoomItems.ToList();

    public List<VideoQualityCustomizationItem> EquippedVideoQualityRoomItems => equippedVideoQualityRoomItems.ToList();

    public List<RealEstateCustomizationItem> OwnedRealEstateItems => ownedRealEstateItems.ToList();

    public RealEstateCustomizationItem EquippedHouse => equippedHouse;


    public List<Car> OwnedCars => ownedCars;

    public Car EquippedCar => equippedCar;


    public HeadItem GetDefaultMaleHead => defaultMaleHead;

    public HeadItem GetDefaultFemaleHead => defaultFemaleHead;

    public CharacterAvatar EquippedAvatar()
    {
        if (equippedCharacterAvatar == null)
        {
            equippedCharacterAvatar = new CharacterAvatar(defaultMaleAvatar);
            return equippedCharacterAvatar;
        }
        else
        {
            return equippedCharacterAvatar;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void Start()
    {
        signalBus.Subscribe<OnPlayerInventoryFetchedSignal>(OnPlayerInventoryFetched);

        playerDataManager = PlayerDataManager.Instance;
        
        gameAnalyticsManager=GameAnalyticsManager.Instance;

    }

    async Task LoadCharacterData()
    {
        var caharacterItemsLoadOp = Addressables.LoadAssetsAsync<ThemeCustomizationItem>("character", null);
        await caharacterItemsLoadOp.Task;
        if (caharacterItemsLoadOp.Status == AsyncOperationStatus.Succeeded)
        {
            var chItems = (List<ThemeCustomizationItem>) caharacterItemsLoadOp.Result;
            ownedCharacterItems = chItems.FindAll(item => item.Owned == true);
            if (characterAvatarAddressedData != null)
            {

                if (string.IsNullOrEmpty(characterAvatarAddressedData.BodyType))
                {
                    equippedCharacterAvatar = new CharacterAvatar(defaultMaleAvatar);
                    return;
                }
            }
            else
            {
                equippedCharacterAvatar = new CharacterAvatar(defaultMaleAvatar);
                return;
            }


            equippedCharacterAvatar.bodyItem =
                (BodyItem) ownedCharacterItems.Find((it) => it.name == characterAvatarAddressedData.BodyType);
            equippedCharacterAvatar.headItem =
                (HeadItem) ownedCharacterItems.Find((it) => it.name == characterAvatarAddressedData.Head);
            equippedCharacterAvatar.hairItem =
                (HairItem) ownedCharacterItems.Find((it) => it.name == characterAvatarAddressedData.Hair);
            equippedCharacterAvatar.torsoItem =
                (TorsoItem) ownedCharacterItems.Find((it) => it.name == characterAvatarAddressedData.Torso);
            equippedCharacterAvatar.legsItem =
                (LegsItem) ownedCharacterItems.Find((it) => it.name == characterAvatarAddressedData.Legs);
            equippedCharacterAvatar.feetItem =
                (FeetItem) ownedCharacterItems.Find((it) => it.name == characterAvatarAddressedData.Feet);

        }
        else
        {
            equippedCharacterAvatar = new CharacterAvatar(defaultMaleAvatar);
        }



    }

    async Task LoadRoomThemeEffectAssets()
    {

        var themeEffectAssets = Addressables.LoadAssetsAsync<ThemeCustomizationItem>("roomtheme", null);
        await themeEffectAssets.Task;
        if (themeEffectAssets.Status == AsyncOperationStatus.Succeeded)
        {
            var themeEffectItems = (List<ThemeCustomizationItem>) themeEffectAssets.Result;

            foreach (var item in themeEffectItems)
            {
                if (item.Owned || playerInventoryAddressedData.ownedRoomThemeEffectItemsNames.Contains(item.name))
                {
                    item.Owned = true;
                    ownedRoomThemeEffectItems.Add(item);

                }
            }

            var roomLayoutItems = playerInventoryAddressedData.currentRoomLayout.equippedThemeITems;
            if (roomLayoutItems != null)
            {
                foreach (var itemName in roomLayoutItems)
                {
                    var eqItem = themeEffectItems.Find((it) => it.name == itemName);
                    if (eqItem)
                    {
                        equippedThemeEffectRoomItems.Add(eqItem);

                    }
                }
            }



        }
        else
        {
            print("Failed to load Assets ");
        }



    }

    async Task LoadVideoQualityAddressedAssets()
    {
        var videoQualityItems = Addressables.LoadAssetsAsync<VideoQualityCustomizationItem>("default", null);
        await videoQualityItems.Task;
        if (videoQualityItems.Status == AsyncOperationStatus.Succeeded)
        {
            var vcItems = (List<VideoQualityCustomizationItem>) videoQualityItems.Result;

            foreach (var item in vcItems)
            {
                if (item.Owned || playerInventoryAddressedData.ownedVideoQualityItemsNames.Contains(item.name))
                {
                    item.Owned = true;
                    ownedVideoQualityRoomItems.Add(item);
                }

            }

            var vcitemnames = playerInventoryAddressedData.currentRoomLayout.equippedVCITems;
            if (vcitemnames != null)
            {
                foreach (var qualityItemsName in vcitemnames)
                {
                    equippedVideoQualityRoomItems.Add(
                        ownedVideoQualityRoomItems.Find((it => it.name == qualityItemsName)));
                }
            }


        }
        else
        {
            print("Failed to load Assets ");
        }
    }

    async Task LoadRealEstateAddressedAssets()
    {
        var realEstateAddressedItems = Addressables.LoadAssetsAsync<RealEstateCustomizationItem>("house", null);
        await realEstateAddressedItems.Task;
        if (realEstateAddressedItems.Status == AsyncOperationStatus.Succeeded)
        {
            var vcItems = (List<RealEstateCustomizationItem>) realEstateAddressedItems.Result;


            foreach (var realStateItemName in playerInventoryAddressedData.ownedRealEstateItemsNames)
            {
                var item = vcItems.Find((item => (item.name == realStateItemName)));
                item.Owned = true;
                ownedRealEstateItems.Add(item);

            }

            equippedHouse = vcItems.Find((rsItem => rsItem.name == playerInventoryAddressedData.equippedHouse));
            if (equippedHouse == null)
            {
                equippedHouse = defaultHouse;
            }

        }
        else
        {
            print("Failed to load Assets ");
        }

        ownedRealEstateItems.Add(defaultHouse);

    }

    async Task LoadCarsAssets()
    {
        var carsLoadOp = Addressables.LoadAssetsAsync<Car>("car", null);
        await carsLoadOp.Task;
        if (carsLoadOp.Status==AsyncOperationStatus.Succeeded)
        {
            var cars = (List<Car>) carsLoadOp.Result;
            foreach (var ownedCarName in playerInventoryAddressedData.ownedCarNames)
            {
                var car = cars.Find((c => c.name == ownedCarName));
                if (car)
                {
                    car.Owned = true;
                    ownedCars.Add(car);
                }
            }

            equippedCar = cars.Find((c) => c.name == playerInventoryAddressedData.equippedCarName);
        }
    }

    async  void  OnPlayerInventoryFetched(OnPlayerInventoryFetchedSignal playerInventoryFetchedSignal)
    {
        playerInventoryAddressedData = playerInventoryFetchedSignal.PlayerInventoryAddressedData;
        characterAvatarAddressedData = playerInventoryFetchedSignal.CharacterAvatarAddressedData;

        if (playerInventoryAddressedData.currentRoomLayout.equippedThemeITems.Count==0)
        {
            playerInventoryAddressedData.currentRoomLayout = defaultRoomLayout;
        }
        await LoadRoomThemeEffectAssets();
        await LoadVideoQualityAddressedAssets();
        await LoadCharacterData();
        await LoadRealEstateAddressedAssets();
        await LoadCarsAssets();

        signalBus.Fire<AssetsLoadedSignal>();
    }
    public void AddCharacterItem(ThemeCustomizationItem themeCustomizationItem)
    {
        
        playerInventoryAddressedData.characterItemsNames.Add(themeCustomizationItem.name);
        ownedCharacterItems.Add(themeCustomizationItem);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }

    public void AddVCItem(VideoQualityCustomizationItem videoQualityCustomizationItem)
    {
        ownedVideoQualityRoomItems.Add(videoQualityCustomizationItem);
        playerInventoryAddressedData.ownedVideoQualityItemsNames.Add(videoQualityCustomizationItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);

    }

    public void AddRoomItem(ThemeCustomizationItem themeCustomizationItem)
    {
        ownedRoomThemeEffectItems.Add(themeCustomizationItem);
        playerInventoryAddressedData.ownedRoomThemeEffectItemsNames.Add(themeCustomizationItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
    }
   
    public void AddRealEstateItem(RealEstateCustomizationItem realEstateCustomizationItem)
    {
        ownedRealEstateItems.Add(realEstateCustomizationItem);
        playerInventoryAddressedData.ownedRealEstateItemsNames.Add(realEstateCustomizationItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        signalBus.Fire(new BuyHouseSignal { houseName = realEstateCustomizationItem.name });
    }

    public void AddCar(Car car)
    {
        ownedCars.Add(car);
        playerInventoryAddressedData.ownedCarNames.Add(car.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }

    public void SetEquippedCar(Car car)
    {
        equippedCar = car;
        playerInventoryAddressedData.equippedCarName = car.name;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);

    }

    public void SetEquippedHouse(RealEstateCustomizationItem realEstateCustomizationItem)
    {
        equippedHouse = realEstateCustomizationItem;
        playerInventoryAddressedData.equippedHouse = equippedHouse.name;
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);

        
    }
    public void ChangeAvatar(CharacterAvatar avatar)
    {
        equippedCharacterAvatar = avatar;
        characterAvatarAddressedData = new CharacterAvatarAddressedData();
        characterAvatarAddressedData.BodyType = avatar.bodyItem.name;
        characterAvatarAddressedData.Head = avatar.headItem.name;
        characterAvatarAddressedData.Hair = avatar.hairItem == null? "" : avatar.hairItem.name;
        characterAvatarAddressedData.Torso = avatar.torsoItem == null ? "" : avatar.torsoItem.name;
        characterAvatarAddressedData.Legs = avatar.legsItem == null ? "" : avatar.legsItem.name;
        characterAvatarAddressedData.Feet = avatar.feetItem==null ? "":avatar.feetItem.name;
        
        signalBus.Fire(new OnCharacterAvatarChanged()
        {
            NewAvatar = avatar
        });
        var totalThemeEquippedItems = new List<ThemeCustomizationItem>(avatar.GetThemesEffectItems());
        totalThemeEquippedItems.AddRange(EquippedThemeEffectRoomItems);
        signalBus.Fire(new OnPlayerEquippedThemeItemChangedSignal()
        {
            CustomizationItems =totalThemeEquippedItems
        });
       playerDataManager.UpdateCharacterAvatar(characterAvatarAddressedData);
       gameAnalyticsManager.SendCustomEvent("PlayerAvatarChanged");

    }

    public  void UpdateRoomData( RoomLayout roomLayout)
    {
        
        playerInventoryAddressedData.currentRoomLayout = roomLayout;
           equippedThemeEffectRoomItems.Clear();
        foreach (var themeItemName in roomLayout.equippedThemeITems)
        { 
           equippedThemeEffectRoomItems.Add(ownedRoomThemeEffectItems.Find((it) => it.name == themeItemName));
        
        }
      
        equippedVideoQualityRoomItems.Clear();
        foreach (var vcItemName in roomLayout.equippedVCITems)
        {
            equippedVideoQualityRoomItems.Add(ownedVideoQualityRoomItems.Find(item => item.name==vcItemName));
        }
        var allThemItems = new List<ThemeCustomizationItem>();
        allThemItems.AddRange(equippedCharacterAvatar.GetThemesEffectItems());
        allThemItems.AddRange(EquippedThemeEffectRoomItems);
        signalBus.Fire(new OnPlayerEquippedThemeItemChangedSignal()
        {
            CustomizationItems = allThemItems
        });
        playerDataManager.UpdatePlayerQuality(equippedVideoQualityRoomItems.Sum((item => item.videoQualityBonus)));
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        gameAnalyticsManager.SendCustomEvent("Room Changed");

    }

    

    public void TestThemeEffectRoomITem(ThemeCustomizationItem themeCustomizationItem)
    {
        signalBus.Fire(new TestRoomThemeItemSignal(){ThemeCustomizationItem = themeCustomizationItem});
        gameAnalyticsManager.SendCustomEvent("Room Theme Effect Item Equipped",new []{themeCustomizationItem.name});
    }

    public void TestVideoQualityRoomItem(VideoQualityCustomizationItem videoQualityCustomizationItem)
    {
        signalBus.Fire(new TestRoomVideoQualityITemSignal(){VideoQualityCustomizationItem = videoQualityCustomizationItem});
        gameAnalyticsManager.SendCustomEvent("Room Room Video Quality Item Equipped",new []{videoQualityCustomizationItem.name});

    }

    public RoomLayout GetRoomLayout()
    {
        return   playerInventoryAddressedData.currentRoomLayout;
    }



}
[System.Serializable]
    public class PlayerInventoryAddressedData
    {    
        public  List<string> characterItemsNames;
        public List<string> ownedRoomThemeEffectItemsNames;
        public List<string> ownedVideoQualityItemsNames;
        public RoomLayout currentRoomLayout;
        public List<string> ownedRealEstateItemsNames;
        public string equippedHouse;
        public List<string> ownedCarNames;
        public string equippedCarName;
        public  PlayerInventoryAddressedData()
        {
            characterItemsNames = new List<string>();
            ownedRoomThemeEffectItemsNames = new List<string>();
            ownedVideoQualityItemsNames = new List<string>();
            ownedRealEstateItemsNames = new List<string>();
            ownedCarNames = new List<string>();
            currentRoomLayout = new RoomLayout();

        } 
    }
[System.Serializable]
public class CharacterAvatarAddressedData
{
    public string BodyType;
    public string Head;
    public string Hair;
    public string Torso;
    public string Legs;
    public string Feet;

    public CharacterAvatarAddressedData()
    {
        BodyType = "BodyItem 1";
        Head = "MaleHead1";
        Hair = "DefualtMale";
        Torso = "HawaiianShirt";
        Legs = "MaleJeans";
        Feet = "BasicShoes";
    }
}