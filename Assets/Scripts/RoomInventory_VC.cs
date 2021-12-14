using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Customizations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using  UniRx;
using UniRx.Triggers;

public class RoomInventory_VC : MonoBehaviour
{
    [Inject] private PlayerInventory playerInventory;
    [Inject] private SignalBus signalBus;
    [SerializeField] private GameObject roomInventoryPanel;
    [SerializeField] private GameObject tabsButtonsTransform;
    [SerializeField] private InventoryButton inventoryButtonPrefab;
    [SerializeField] private GameObject buttonsTransform;
    public List<InventoryButton> roomInventoryButtons;
    [SerializeField] private GameObject installPanel;
    [SerializeField] private Button installButton;
    [SerializeField] private TMP_Text itemName, itemRareness, itemDescription, itemNewStats;
    [SerializeField] private Image itemLogo,rarenessImage;
    [SerializeField] private Button saveButton, discardButton;
    [SerializeField] private Sprite commonSprite, uncommonSprite, rareSprite;
    [SerializeField] private TMP_Text equippedThemeItemsEffect;
    [SerializeField] private TMP_Text equippedVideoQualityItemsEffect;
    private List<Sprite> rarenessSprites;

    [SerializeField] private Button roomCustomizationButton;
    [SerializeField] private Canvas inventoryCanvas;
    
    void OpenRoomCustomizationPanel()
    {
        inventoryCanvas.gameObject.SetActive(true);
        roomInventoryPanel.gameObject.SetActive(true);
        signalBus.Fire(new RoomCustomizationVisibilityChanged()
        {
            Visibility = true
        });
        signalBus.Fire(new ChangeBackButtonSignal()
        {
            changeToHome = true,
            buttonAction = (() =>
            {
                inventoryCanvas.gameObject.SetActive(false);
                roomInventoryPanel.gameObject.SetActive(false);
                
                signalBus.Fire(new RoomCustomizationVisibilityChanged()
                {
                    Visibility = false
                });
            })
        });
    }
    private void Start()
    {
        UpdateVideoQualityItemsText(playerInventory.EquippedVideoQualityRoomItems);
        UpdateThemeEffectItemsText(playerInventory.EquippedThemeEffectRoomItems);
        roomCustomizationButton.onClick.AddListener(OpenRoomCustomizationPanel);
        
        signalBus.Subscribe<CharacterCustomizationVisibilityChanged>((() =>
        {
            roomInventoryPanel.gameObject.SetActive(false);
        }));
        
      
    }

    Sprite GetRarenessSpriteByIndex(Rareness rareness)
    {
        return rarenessSprites[(int) rareness];
    }
    public void PopulateInventoryButtons(string type)
    {

        roomInventoryButtons.ForEach((bt) => bt.gameObject.SetActive(true));
        roomInventoryButtons.FindAll((bt) => bt.Type != type).ForEach((bt) => bt.gameObject.SetActive(false));

    }

    private void OnEnable()
    {
       // backButton.onClick.RemoveAllListeners();
        //backButton.onClick.AddListener((() => roomInventoryPanel.gameObject.SetActive(false)));
        
    }

    void UpdateThemeEffectItemsText(List<ThemeCustomizationItem> themeCustomizationItems)
    {
        var themesNames = Enum.GetNames(typeof(ThemeType));
        themesNames.ToList().ForEach((s)=>print(s));   
        var themesBounses= themesNames.ToDictionary(s=>s,k=>0f);
        
       
        foreach (var equippedItem in themeCustomizationItems)
        {
            foreach (var themeEffect in equippedItem.affectedTheme)
            {
                print("Theme name = " +themeEffect.ThemeType.ToString());
                themesBounses[themeEffect.ThemeType.ToString()] += themeEffect.themePopularityFactor;
            }            
        }

        equippedThemeItemsEffect.text = null;
        foreach (var themeBouns in themesBounses)
        {
            if (themeBouns.Value==0)
            {
                continue;
            }
            equippedThemeItemsEffect.text += themeBouns.Key + "  : " + themeBouns.Value * 100 + "%" + "\n";
        }
    }

    void UpdateVideoQualityItemsText(List<VideoQualityCustomizationItem> videoQualityCustomizationItems)
    {
        float sum = 0;
        foreach (var vCItem in videoQualityCustomizationItems)
        {
            sum += vCItem.videoQualityBonus;
        }

        equippedVideoQualityItemsEffect.text ="Video Quality +% "+sum*100;
    }

    void Awake()
    {
        roomInventoryPanel.OnEnableAsObservable().Subscribe((unit =>
        {
            signalBus.Fire(new RoomZoomStateChangedSignal()
            {
                ZoomIn = false
            });
        }));

        roomInventoryPanel.OnDisableAsObservable().Subscribe((unit =>
        {
            signalBus.Fire(new RoomZoomStateChangedSignal()
            {
                ZoomIn = true
            });
        }));
        roomInventoryButtons = new List<InventoryButton>();
        roomInventoryPanel.OnEnableAsObservable().Subscribe((s) => CreateInventoryButtons());
        saveButton.onClick.AddListener(SaveRoomLayout);
        discardButton.onClick.AddListener(DiscardRoomLayout);
        rarenessSprites = new List<Sprite>() {commonSprite, uncommonSprite, rareSprite};
    

    }

  
    public void SaveRoomLayout()
    {
        signalBus.Fire<SaveRoomLayoutSignal>();
        UpdateThemeEffectItemsText(playerInventory.EquippedThemeEffectRoomItems);
        UpdateVideoQualityItemsText(playerInventory.EquippedVideoQualityRoomItems);
    }

    public void DiscardRoomLayout()
    {
        signalBus.Fire<DiscardRoomLayoutSignal>();
    }
    

    void CreateInventoryButtons()
    {
        foreach (var inventoryButton in roomInventoryButtons)
        {
            Destroy(inventoryButton.gameObject);
        }
        roomInventoryButtons.Clear();
        foreach (var roomItem in playerInventory.RoomThemeEffectItems)
        {
            string type;
            switch (roomItem)
            {
                case WallOrnament wallOrnament :
                    type = "wall"; break;
                case FloorOrnament floorOrnament:
                    type = "floor"; break;
                case RoomObject roomObject :
                    type = "misc"; break; 
                default: 
                    type = "misc"; break;
            }
            var bt = Instantiate(inventoryButtonPrefab, buttonsTransform.transform);
            bt.Type = type;
            print("Item Name" + roomItem.name);
            bt.SetButtonSprites(roomItem.sprite,GetRarenessSpriteByIndex(roomItem.rareness));
            bt.SetButtonAction(()=>
            {
                installPanel.gameObject.SetActive(true);

                installButton.onClick.RemoveAllListeners();
                SetSelectedPanelData(roomItem.name, roomItem.rareness.ToString(), roomItem.descriptionText,
                    roomItem.newStatsText, roomItem.sprite,GetRarenessSpriteByIndex(roomItem.rareness));
                installButton.onClick.AddListener(() =>
                {
                    installPanel.gameObject.SetActive(false);
                    playerInventory.TestThemeEffectRoomITem(roomItem);
                   

                });
            });
            
            roomInventoryButtons.Add(bt);
        }
        foreach (var videoQualityRoomItem in playerInventory.VideoQualityRoomItems)
        {
            string type;
            switch (videoQualityRoomItem.videoQualityItemType)
            {
                case VideoQualityItemType.Computer:
                    type = "computer";
                    break;
                case VideoQualityItemType.Camera:
                    type = "camera";
                    break;
                case VideoQualityItemType.Microphone:
                    type = "microphone";
                    break;
                case VideoQualityItemType.GreenScreen:
                    type = "misc";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var bt = Instantiate(inventoryButtonPrefab, buttonsTransform.transform);
            bt.Type = type;
            bt.SetButtonSprites(videoQualityRoomItem.itemSprite,GetRarenessSpriteByIndex(videoQualityRoomItem.rareness));
            bt.SetButtonAction(()=>
            {
                installPanel.gameObject.SetActive(true);
                SetSelectedPanelData(videoQualityRoomItem.name, videoQualityRoomItem.rareness.ToString(),
                    videoQualityRoomItem.descriptionText, videoQualityRoomItem.newStatsText,
                    videoQualityRoomItem.itemSprite,GetRarenessSpriteByIndex(videoQualityRoomItem.rareness));
                installButton.onClick.RemoveAllListeners();
                installButton.onClick.AddListener(() =>
                {
                    installPanel.gameObject.SetActive(false);
                    playerInventory.TestVideoQualityRoomItem(videoQualityRoomItem);
                    
                });
            });
            
            roomInventoryButtons.Add(bt);
        }
        
        PopulateInventoryButtons("wall");
        HighLightCurrentTab(0);
    }

 public  void HighLightCurrentTab(int tabIndex)
    {
        for (int i = 0; i < tabsButtonsTransform.transform.childCount; i++)
        {
            var bt = tabsButtonsTransform.transform.GetChild(i).GetComponent<Button>();
            var color = bt.image.color;
            if (i!=tabIndex)
            {
                
                bt.image.color = new Color(color.r, color.g, color.b, 0);
             
            }
            else
            {
                bt.Select();
                bt.image.color = new Color(color.r, color.g, color.b, 1f);

            }
        }
    }

 void SetSelectedPanelData(string _name,string _rareness,string _description,string _newStats,Sprite _logoSprite,Sprite rarenessSprite)
 {
     itemName.text = _name;
     itemRareness.text = _rareness;
     itemDescription.text = _description;
     itemNewStats.text = _newStats;
     itemLogo.sprite = _logoSprite;
     rarenessImage.sprite = rarenessSprite;
 }
}
