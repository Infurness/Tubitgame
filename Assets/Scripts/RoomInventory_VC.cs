using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Button backButton;
    public List<InventoryButton> roomInventoryButtons;
    [SerializeField] private GameObject installPanel;
    [SerializeField] private Button installButton;
    [SerializeField] private TMP_Text itemName, itemRareness, itemDescription, itemNewStats;
    [SerializeField] private Image itemLogo;
    [SerializeField] private Button saveButton, discardButton;

    public void PopulateInventoryButtons(string type)
    {

        roomInventoryButtons.ForEach((bt) => bt.gameObject.SetActive(true));
        roomInventoryButtons.FindAll((bt) => bt.Type != type).ForEach((bt) => bt.gameObject.SetActive(false));


    }

    private void OnEnable()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener((() => roomInventoryPanel.gameObject.SetActive(false)));

    }

    void Start()
    {
        roomInventoryButtons = new List<InventoryButton>();
        roomInventoryPanel.OnEnableAsObservable().Subscribe((s) => CreateInventoryButtons());
        saveButton.onClick.AddListener(SaveRoomLayout);
        discardButton.onClick.AddListener(DiscardRoomLayout);
    }

    public void SaveRoomLayout()
    {
        signalBus.Fire<SaveRoomLayoutSignal>();
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
            bt.SetButtonLogo(roomItem.logoSprite);
            bt.SetButtonAction(()=>
            {
                installPanel.gameObject.SetActive(true);

                installButton.onClick.RemoveAllListeners();
                installButton.onClick.AddListener(() =>
                {
                    installPanel.gameObject.SetActive(false);
                    playerInventory.TestThemeEffectRoomITem(roomItem);
                    SetSelectedPanelData(roomItem.name, roomItem.rareness.ToString(), roomItem.descriptionText,
                        roomItem.newStatsText, roomItem.logoSprite);

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
            bt.SetButtonLogo(videoQualityRoomItem.logoSprite);
            bt.SetButtonAction(()=>
            {
                installPanel.gameObject.SetActive(true);
                installButton.onClick.RemoveAllListeners();
                installButton.onClick.AddListener(() =>
                {
                    installPanel.gameObject.SetActive(false);
                    playerInventory.TestVideoQualityRoomItem(videoQualityRoomItem);
                    SetSelectedPanelData(videoQualityRoomItem.name, videoQualityRoomItem.rareness.ToString(),
                        videoQualityRoomItem.descriptionText, videoQualityRoomItem.newStatsText,
                        videoQualityRoomItem.logoSprite);
                });
            });
            
            roomInventoryButtons.Add(bt);
        }
        
        PopulateInventoryButtons("floor");
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
                
                bt.image.color = new Color(color.r, color.g, color.b, 0.5f);
             
            }
            else
            {
                bt.Select();
                bt.image.color = new Color(color.r, color.g, color.b, 255);

            }
        }
    }

 void SetSelectedPanelData(string _name,string _rareness,string _description,string _newStats,Sprite _logoSprite)
 {
     itemName.text = _name;
     itemRareness.text = _rareness;
     itemDescription.text = _description;
     itemNewStats.text = _newStats;
     itemLogo.sprite = _logoSprite;
 }
}
