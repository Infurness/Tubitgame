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
    [SerializeField] private HeadItem defaultFemaleHead,defaultMaleHead;
    [SerializeField] private List<ThemeCustomizationItem> characterItems;
    [SerializeField] private List<ThemeCustomizationItem> roomThemeEffectItems;
    [SerializeField] private List<ThemeCustomizationItem> equippedThemeEffectRoomItems;
    [SerializeField] private List<VideoQualityCustomizationItem> videoQualityRoomItems;
    [SerializeField] private List<VideoQualityCustomizationItem> equippedVideoQualityRoomItems;
    [SerializeField] private List<RealEstateCustomizationItem> realEstateItems;
    [SerializeField] private RealEstateCustomizationItem equippedHouse;
    [SerializeField] private RealEstateCustomizationItem defaultHouse;
    [SerializeField] private List<HeadItem> headItems;
    [SerializeField] private List<HairItem> hairItems;

    public List<ThemeCustomizationItem> CharacterItems => characterItems.ToList();

    public List<ThemeCustomizationItem> RoomThemeEffectItems => roomThemeEffectItems.ToList();

    public List<ThemeCustomizationItem> EquippedThemeEffectRoomItems => equippedThemeEffectRoomItems.ToList();

    public List<VideoQualityCustomizationItem> VideoQualityRoomItems => videoQualityRoomItems.ToList();

    public List<VideoQualityCustomizationItem> EquippedVideoQualityRoomItems => equippedVideoQualityRoomItems.ToList();

    public List<RealEstateCustomizationItem> RealEstateItems => realEstateItems.ToList();

    public RealEstateCustomizationItem EquippedHouse => equippedHouse;


    public HeadItem GetDefaultMaleHead => defaultMaleHead;

    public HeadItem GetDefaultFemaleHead => defaultFemaleHead;

    public CharacterAvatar EquippedAvatar()
    {
        if (equippedCharacterAvatar==null)
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
        if (Instance==null)
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

    }

    async Task LoadCharacterData()
    {
        var caharacterItemsLoadOp= Addressables.LoadAssetsAsync<ThemeCustomizationItem>("character", null);
        await caharacterItemsLoadOp.Task;
        if (caharacterItemsLoadOp.Status == AsyncOperationStatus.Succeeded)
        {
            var chItems = caharacterItemsLoadOp.Result;
            characterItems =(List<ThemeCustomizationItem>) chItems;
            if (characterAvatarAddressedData!=null)
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
                (BodyItem) characterItems.Find((it) => it.name == characterAvatarAddressedData.BodyType);
            equippedCharacterAvatar.headItem =
                (HeadItem) characterItems.Find((it) => it.name == characterAvatarAddressedData.Head);
            equippedCharacterAvatar.hairItem =
                (HairItem) characterItems.Find((it) => it.name == characterAvatarAddressedData.Hair);
            equippedCharacterAvatar.torsoItem =
                (TorsoItem) characterItems.Find((it) => it.name == characterAvatarAddressedData.Torso);
            equippedCharacterAvatar.legsItem =
                (LegsItem) characterItems.Find((it) => it.name == characterAvatarAddressedData.Legs);
            equippedCharacterAvatar.feetItem =
                (FeetItem) characterItems.Find((it) => it.name == characterAvatarAddressedData.Feet);

        }
        else
        {
            equippedCharacterAvatar = new CharacterAvatar(defaultMaleAvatar);
        }

         
        
    }
    async Task LoadThemeEffectAddressedAssets()
    {
        
        var themeEffectAssets= Addressables.LoadAssetsAsync<ThemeCustomizationItem>("roomtheme", null);
        await themeEffectAssets.Task;
        if (themeEffectAssets.Status == AsyncOperationStatus.Succeeded)
        {
            var themeEffectItems= (List<ThemeCustomizationItem>) themeEffectAssets.Result;
            roomThemeEffectItems = themeEffectItems;
       
            var roomLayout = playerInventoryAddressedData.RoomLayout;
            foreach (var slot in roomLayout.roomSlots)
            {
                var item = themeEffectItems.Find((it) => it.name == slot.ItemName);
                if (item)
                {
                    equippedThemeEffectRoomItems.Add(item);
   
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
         var videoQualityItems= Addressables.LoadAssetsAsync<VideoQualityCustomizationItem>("default", null);
         await videoQualityItems.Task;
         if (videoQualityItems.Status == AsyncOperationStatus.Succeeded)
         {
             var vcItems= (List<VideoQualityCustomizationItem>) videoQualityItems.Result;

             videoQualityRoomItems = vcItems;

             // foreach (var videoQualityItemName in playerInventoryAddressedData.videoQualityItemsNames)
             // {
             //     videoQualityRoomItems.Add(vcItems.Find((item => item.name==videoQualityItemName)));     //todo uncomment when shop implemented
             // }
             
             

             foreach (var qualityItemsName in playerInventoryAddressedData.equippedVideoQualityItemsNames)
             {
                 equippedVideoQualityRoomItems.Add(videoQualityRoomItems.Find((item => item.name==qualityItemsName)));
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
            var vcItems = (List<RealEstateCustomizationItem>)realEstateAddressedItems.Result;

            
            foreach (var realStateItemName in playerInventoryAddressedData.realEstateItemsNames)
            {
                var item = vcItems.Find((item => item.name == realStateItemName));
                realEstateItems.Add(item);

            }

          equippedHouse=  vcItems.Find((rsItem => rsItem.name == playerInventoryAddressedData.equippedHouse));
          if (equippedHouse==null)
          {
              equippedHouse = defaultHouse;
          }

        }
        else
        {
            print("Failed to load Assets ");
        }
        realEstateItems.Add(defaultHouse);

    }
    async  void  OnPlayerInventoryFetched(OnPlayerInventoryFetchedSignal playerInventoryFetchedSignal)
    {
        playerInventoryAddressedData = playerInventoryFetchedSignal.PlayerInventoryAddressedData;
        characterAvatarAddressedData = playerInventoryFetchedSignal.CharacterAvatarAddressedData;
        await LoadThemeEffectAddressedAssets();
        await LoadVideoQualityAddressedAssets();
        await LoadCharacterData();
        await LoadRealEstateAddressedAssets();

        signalBus.Fire<AssetsLoadedSignal>();
    }
    public void AddCharacterItem(ThemeCustomizationItem themeCustomizationItem)
    {
        
        playerInventoryAddressedData.characterItemsNames.Add(themeCustomizationItem.name);
        characterItems.Add(themeCustomizationItem);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        
    }

    public void AddVCItem(VideoQualityCustomizationItem videoQualityCustomizationItem)
    {
        videoQualityRoomItems.Add(videoQualityCustomizationItem);
        playerInventoryAddressedData.videoQualityItemsNames.Add(videoQualityCustomizationItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);

    }

    public void AddRoomItem(ThemeCustomizationItem themeCustomizationItem)
    {
        roomThemeEffectItems.Add(themeCustomizationItem);
        playerInventoryAddressedData.roomItemsNames.Add(themeCustomizationItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
    }
   
    public void AddRealEstateItem(RealEstateCustomizationItem realEstateCustomizationItem)
    {
        realEstateItems.Add(realEstateCustomizationItem);
        playerInventoryAddressedData.realEstateItemsNames.Add(realEstateCustomizationItem.name);
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);
        signalBus.Fire(new BuyHouseSignal { houseName = realEstateCustomizationItem.name });
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


    }

    public  void UpdateRoomData(RoomLayout roomLayout,List<VideoQualityCustomizationItem> vCItems)
    {
        
        playerInventoryAddressedData.RoomLayout = roomLayout;
        equippedThemeEffectRoomItems.Clear();
        // foreach (var floorItem in roomLayout.FloorLayoutSlots)
        // { 
        //    equippedThemeEffectRoomItems.Add(roomThemeEffectItems.Find((it) => it.name == floorItem.ItemName));
        //
        // }
        // foreach (var wallItem in roomLayout.WallLayoutSlots)
        // { 
        //     equippedThemeEffectRoomItems.Add(roomThemeEffectItems.Find((it) => it.name == wallItem.ItemName));
        // }
        // foreach (var objectItem in roomLayout.ObjectsLayoutSlots)
        // { 
        //     equippedThemeEffectRoomItems.Add(roomThemeEffectItems.Find((it) => it.name == objectItem.ItemName));
        // }
        equippedVideoQualityRoomItems.Clear();
        equippedVideoQualityRoomItems = vCItems;
        playerInventoryAddressedData.equippedVideoQualityItemsNames.Clear();
        equippedVideoQualityRoomItems.ForEach((vc) =>
            playerInventoryAddressedData.equippedVideoQualityItemsNames.Add(vc.name));
        var allThemItems = new List<ThemeCustomizationItem>();
        allThemItems.AddRange(equippedCharacterAvatar.GetThemesEffectItems());
        allThemItems.AddRange(EquippedThemeEffectRoomItems);
        signalBus.Fire(new OnPlayerEquippedThemeItemChangedSignal()
        {
            CustomizationItems = allThemItems
        });
        playerDataManager.UpdatePlayerQuality(equippedVideoQualityRoomItems.Sum((item => item.videoQualityBonus)));
        playerDataManager.UpdatePlayerInventoryData(playerInventoryAddressedData);

    }

    

    public void TestThemeEffectRoomITem(ThemeCustomizationItem themeCustomizationItem)
    {
        signalBus.Fire(new TestRoomThemeItemSignal(){ThemeCustomizationItem = themeCustomizationItem});
    }

    public void TestVideoQualityRoomItem(VideoQualityCustomizationItem videoQualityCustomizationItem)
    {
        signalBus.Fire(new TestRoomVideoQualityITemSignal(){VideoQualityCustomizationItem = videoQualityCustomizationItem});
    }

    public RoomLayout GetRoomLayout()
    {
        return  new RoomLayout( playerInventoryAddressedData.RoomLayout);
    }

    public HairItem GetHairItem(string hairName)
    {
        return hairItems.Find ((it) => it.name == hairName);
    }

    public HeadItem GetHeadItem (string headName)
    {
        return headItems.Find ((it) => it.name == headName);
    }

}
[System.Serializable]
    public class PlayerInventoryAddressedData
    {    
        public  List<string> characterItemsNames;
        public List<string> roomItemsNames;
        public List<string> videoQualityItemsNames;
        public List<string> equippedVideoQualityItemsNames;
        public List<string> realEstateItemsNames;
        public string equippedHouse;
        public RoomLayout RoomLayout;
        public  PlayerInventoryAddressedData()
        {
            characterItemsNames = new List<string>();
            roomItemsNames = new List<string>();
            videoQualityItemsNames = new List<string>();
            equippedVideoQualityItemsNames = new List<string>();
            realEstateItemsNames = new List<string>();
            RoomLayout = new RoomLayout();

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
        Hair = "RedHair";
        Torso = "T_Shirt";
        Legs = "MaleJeans";
        Feet = "RareShoes";
    }
}