using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Customizations;
using TMPro;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class CaharacterInventory_VC : MonoBehaviour
{
    [Inject] private PlayerInventory m_PlayerInventory;
    [Inject] private SignalBus signalBus;
    [SerializeField] private CharacterCustomizationsSlot  headBSlot, faceSlot, torsoSlot, legsSlot, feetSlot;
    [SerializeField] private InventoryButton inventoryButtonPrefab;
    [SerializeField] private TabView_VC inventoryTabView;
    private List<InventoryButton> inventoryButtons;
    [SerializeField] private GameObject equipPanel, selectPanel;
    [SerializeField] private Button equipBt, unEquipBt;
    [SerializeField] private TMP_Text selectedItemName,equippedItemName,selectedItemRareness,equippedItemRareness;
    [SerializeField] private TMP_Text equippedText, selectedText;
    [SerializeField] private TMP_Text equippedStatsText, selectedStatsText;
    [SerializeField] private Image equippedLogoImage,equippedRarenessImage, selectedLogoImage,selectedRarenessImgae;
    [SerializeField] private GameObject characterSlotsPanel,roomSlotsPanel,buttonsPanel;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private GameObject characterPreview;
    [SerializeField] private Sprite commonSprite, uncommonSprite, rareSprite;
    [SerializeField] private GameObject infoPanel,themeEffectPanel;
    [SerializeField] private TMP_Text playerName, SubsNum;
    [SerializeField] private TMP_Text equippedItemsEffect;
    private List<Sprite> rarenessSprites;

    void Start()
    {
        headBSlot.SetButtonAction(OnHeadButtonClicked);
        faceSlot.SetButtonAction(OnFaceButtonClicked);
        torsoSlot.SetButtonAction(OnTorsoButtonClicked);
        legsSlot.SetButtonAction(OnLegsButtonClicked);
        feetSlot.SetButtonAction(OnFeetButtonClicked);
        inventoryButtons = new List<InventoryButton>();
        rarenessSprites = new List<Sprite>() {commonSprite, uncommonSprite, rareSprite};
        signalBus.Subscribe<OnCharacterItemEquippedSignal>(UpdateItemsEffectText );
        
   ;
    }

    private void OnEnable()
    {

        foreach (var characterItem in m_PlayerInventory.EquippedCharacterItems)
        {
         
            switch (characterItem)
            {
                case  HeadItem headItem:headBSlot.SetIconSprite(headItem.sprite);
                    headBSlot.SetRarenessSprite(GetRarenessSpriteByIndex(headItem.rareness));
                    break;
                case  FaceItem faceItem: faceSlot.SetIconSprite(faceItem.sprite);
                    faceSlot.SetIconSprite(GetRarenessSpriteByIndex(faceItem.rareness));
                    break;
                case  TorsoItem torsoItem: headBSlot.SetIconSprite(torsoItem.sprite);
                    torsoSlot.SetIconSprite(GetRarenessSpriteByIndex(torsoItem.rareness));
                    break;
                case  LegsItem legsItem:legsSlot.SetIconSprite(legsItem.sprite);
                    legsSlot.SetIconSprite(GetRarenessSpriteByIndex(legsItem.rareness));
                    break;
                case FeetItem feetItem: feetSlot.SetIconSprite (feetItem.sprite);              
                    feetSlot.SetIconSprite(GetRarenessSpriteByIndex(feetItem.rareness));

                    break;
            }
        }

        playerName.text = PlayerDataManager.Instance.GetPlayerName().ToUpper();
        SubsNum.text = PlayerDataManager.Instance.GetSubscribers().ToString();
        UpdateItemsEffectText();


    }

    Sprite GetRarenessSpriteByIndex(Rareness rareness)
    {
        return rarenessSprites[(int) rareness];
    }

    void UpdateItemsEffectText()
    {
        var themesNames = Enum.GetNames(typeof(ThemeType));
        var themesBounses= themesNames.ToDictionary(s=>s,k=>0f);
        
        var equippedItems = m_PlayerInventory.EquippedCharacterItems;
        foreach (var equippedItem in equippedItems)
        {
            foreach (var themeEffect in equippedItem.affectedTheme)
            {
                themesBounses[themeEffect.ThemeType.ToString()] += themeEffect.themePopularityFactor;
            }            
        }

        equippedItemsEffect.text = null;
        foreach (var themeBouns in themesBounses)
        {
            if (themeBouns.Value==0)
            {
                continue;
            }
            equippedItemsEffect.text += themeBouns.Key + "  : " + themeBouns.Value * 100 + "%" + "\n";
        }
    }

    private void SetEquippedPanelData(string description,string newStats,Sprite logoSprite,string itemName,string rarenessText,Sprite rarenessSprite)
    {
        equippedText.text = description;
        equippedStatsText.text =newStats;
        equippedLogoImage.sprite = logoSprite;
        equippedRarenessImage.sprite = rarenessSprite;
        equippedItemName.text = itemName;
        equippedItemRareness.text = rarenessText;
        equipPanel.SetActive(true);
    }
    private void SetSelectedPanelData(string description,string newStats,Sprite logoSprite,string itemName,string rarenessText,Sprite rarenessSprite)
    {
        selectedText.text = description;
        selectedStatsText.text =newStats;
        selectedLogoImage.sprite = logoSprite;
        selectedRarenessImgae.sprite = rarenessSprite;
        selectedItemName.text = itemName;
        selectedItemRareness.text = rarenessText;
        selectPanel.gameObject.SetActive(true);    }
    void ShowButtonsByItemType(string type)
    {
        foreach (var inventoryButton in inventoryButtons)
        {
            if (inventoryButton.Type==type||type=="All")
            {
                inventoryButton.gameObject.SetActive(true);
            }
            else
            {
                inventoryButton.gameObject.SetActive(false);
            }
        }
    }
    public void OnHeadButtonClicked()
    {
        inventoryTabView.gameObject.SetActive(true);
        infoPanel.SetActive(false);
        themeEffectPanel.SetActive(false);
        characterPreview.SetActive(false);
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedHead = m_PlayerInventory.GetEquippedItem<HeadItem>();
        SetEquippedPanelData(equippedHead.descriptionText, equippedHead.newStatsText, equippedHead.sprite,
            equippedHead.name, equippedHead.rareness.ToString() + " " +
                               equippedHead.HeadItemType.ToString(),GetRarenessSpriteByIndex(equippedHead.rareness));

        foreach (var  item in m_PlayerInventory.CharacterItems)
        {
            if(item.GetType()!=typeof(HeadItem))
                continue;

            HeadItem headItem = (HeadItem) item;
            
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = headItem.HeadItemType.ToString();
            invBt.SetButtonSprites(headItem.sprite,GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    SetEquippedPanelData(headItem.descriptionText,headItem.newStatsText,headItem.sprite,headItem.name,headItem.rareness.ToString() + " " +
                        headItem.HeadItemType.ToString(),GetRarenessSpriteByIndex(headItem.rareness));
                    m_PlayerInventory.EquipCharacterItem(headItem);

                });
     
                SetSelectedPanelData(headItem.descriptionText,headItem.newStatsText,headItem.sprite,headItem.name,headItem.rareness.ToString() + " " +
                    headItem.HeadItemType.ToString(),GetRarenessSpriteByIndex(headItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        inventoryTabView.InitTabView(
            new string[] {"All", HeadItemType.Hats.ToString(), HeadItemType.Hair_Style.ToString()},
            ShowButtonsByItemType);
        inventoryTabView.SetButtonsTabVisibly(true);
        SetBackButtonBehaviour();
        characterPreview.gameObject.SetActive(false);
    }

    private void SetBackButtonBehaviour()
    {
        // backButton.onClick.RemoveAllListeners();
        //
        // backButton.onClick.AddListener((() =>
        // {
        //     infoPanel.SetActive(true);
        //     characterPreview.SetActive(true);
        //     themeEffectPanel.SetActive(true);
        //
        //     inventoryTabView.gameObject.SetActive(false);
        //     equipPanel.SetActive(false);
        //     selectPanel.SetActive(false);
        //     backButton.onClick.AddListener(() =>
        //     {
        //         backButton.onClick.RemoveAllListeners();
        //
        //         inventoryCanvas.gameObject.SetActive(false);
        //     });
        // }));
    }

     void OnFaceButtonClicked()
    {
       
        inventoryTabView.gameObject.SetActive(true);
        infoPanel.SetActive(false);
        themeEffectPanel.SetActive(false);

        characterPreview.SetActive(false);
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedFace = m_PlayerInventory.GetEquippedItem<FaceItem>();
        SetEquippedPanelData(equippedFace.descriptionText, equippedFace.newStatsText, equippedFace.sprite,
            equippedFace.name, equippedFace.rareness.ToString() + " " +
                               equippedFace.FaceItemType.ToString(),GetRarenessSpriteByIndex(equippedFace.rareness));

        foreach (var item in m_PlayerInventory.CharacterItems)
        {
            if (item.GetType() != typeof(FaceItem))
                continue;
            FaceItem faceItem =(FaceItem)item;
                InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = faceItem.FaceItemType.ToString();
            invBt.SetButtonSprites(faceItem.sprite,GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    m_PlayerInventory.EquipCharacterItem(faceItem);
                    SetEquippedPanelData(faceItem.descriptionText,faceItem.newStatsText,faceItem.sprite,faceItem.name,faceItem.rareness.ToString() + " " +
                        faceItem.FaceItemType.ToString(),GetRarenessSpriteByIndex(faceItem.rareness));

                });
     
                SetSelectedPanelData(faceItem.descriptionText,faceItem.newStatsText,faceItem.sprite,faceItem.name,faceItem.rareness.ToString() + " " +
                    faceItem.FaceItemType.ToString(),GetRarenessSpriteByIndex(faceItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        inventoryTabView.InitTabView(
            new string[]
            {
                "All", FaceItemType.Glasses.ToString(), FaceItemType.Masks.ToString(),
                FaceItemType.Piercings.ToString(), FaceItemType.Bold_Makeup_Applications.ToString()
            },
            ShowButtonsByItemType);
        SetBackButtonBehaviour();
        characterPreview.gameObject.SetActive(false);

    }

     void OnTorsoButtonClicked()
    {
        inventoryTabView.gameObject.SetActive(true);
        infoPanel.SetActive(false);
        themeEffectPanel.SetActive(false);

        characterPreview.SetActive(false);
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedTorso = m_PlayerInventory.GetEquippedItem<TorsoItem>();
        SetEquippedPanelData(equippedTorso.descriptionText, equippedTorso.newStatsText, equippedTorso.sprite,
            equippedTorso.name, equippedTorso.rareness.ToString() + " " +
                                equippedTorso.TorsoItemType.ToString(),GetRarenessSpriteByIndex(equippedTorso.rareness));

        foreach (var item in m_PlayerInventory.CharacterItems)
        {
            if(item.GetType()!=typeof(TorsoItem))
                continue;
            TorsoItem torsoItem =(TorsoItem) item;
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = torsoItem.TorsoItemType.ToString();
            invBt.SetButtonSprites(torsoItem.sprite,GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    m_PlayerInventory.EquipCharacterItem(torsoItem);
                    SetEquippedPanelData(torsoItem.descriptionText,torsoItem.newStatsText,torsoItem.sprite,torsoItem.name,torsoItem.rareness.ToString() + " " +
                        torsoItem.TorsoItemType.ToString(),GetRarenessSpriteByIndex(torsoItem.rareness));
                });
     
                SetSelectedPanelData(torsoItem.descriptionText,torsoItem.newStatsText,torsoItem.sprite,torsoItem.name,torsoItem.rareness.ToString() + " " +
                    torsoItem.TorsoItemType.ToString(),GetRarenessSpriteByIndex(torsoItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        inventoryTabView.InitTabView(
            new string[]
            {
                "All", TorsoItemType.Jackets.ToString(), TorsoItemType.Tshirts.ToString(),
               
            },
            ShowButtonsByItemType);
        SetBackButtonBehaviour();
        characterPreview.gameObject.SetActive(false);


       
    }

     void OnLegsButtonClicked()
    {
        inventoryTabView.gameObject.SetActive(true);
        infoPanel.SetActive(false);
        themeEffectPanel.SetActive(false);

        characterPreview.SetActive(false);
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedLegs = m_PlayerInventory.GetEquippedItem<LegsItem>();
        SetEquippedPanelData(equippedLegs.descriptionText, equippedLegs.newStatsText, equippedLegs.sprite,
            equippedLegs.name, equippedLegs.rareness.ToString() + " " +
                               equippedLegs.LegsType.ToString(),GetRarenessSpriteByIndex(equippedLegs.rareness));

        foreach (var item in m_PlayerInventory.CharacterItems)
        {
            if(item.GetType()!=typeof(LegsItem))
                continue;
            LegsItem legsItem = (LegsItem) item;
            
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = legsItem.LegsType.ToString();
            invBt.SetButtonSprites(legsItem.sprite,GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    m_PlayerInventory.EquipCharacterItem(legsItem);
                    SetEquippedPanelData(legsItem.descriptionText,legsItem.newStatsText,legsItem.sprite,legsItem.name,legsItem.rareness.ToString() + " " +
                        legsItem.LegsType.ToString(),GetRarenessSpriteByIndex(legsItem.rareness));
                });
     
                SetSelectedPanelData(legsItem.descriptionText,legsItem.newStatsText,legsItem.sprite,legsItem.name,legsItem.rareness.ToString() + " " +
                    legsItem.LegsType.ToString(),GetRarenessSpriteByIndex(legsItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        inventoryTabView.InitTabView(
            new string[]
            {
                "All", LegsItemType.Pants.ToString(),LegsItemType.Shorts.ToString(),LegsItemType.Bathing_Suits.ToString()
               
            },
            ShowButtonsByItemType);
        SetBackButtonBehaviour();
        characterPreview.gameObject.SetActive(false);


        
    }

     void OnFeetButtonClicked()
    {
      
        inventoryTabView.gameObject.SetActive(true);
        infoPanel.SetActive(false);
        themeEffectPanel.SetActive(false);

        characterPreview.SetActive(false);
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedFeet = m_PlayerInventory.GetEquippedItem<FeetItem>();
        SetEquippedPanelData(equippedFeet.descriptionText, equippedFeet.newStatsText, equippedFeet.sprite,
            equippedFeet.name, equippedFeet.rareness.ToString() + " " +
                               equippedFeet.FeetItemType.ToString(), GetRarenessSpriteByIndex(equippedFeet.rareness));

        foreach (var  item in m_PlayerInventory.CharacterItems)
        {
            if(item.GetType()!=typeof(FeetItem))
                continue;
            FeetItem feetItem = (FeetItem) item;
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = feetItem.FeetItemType.ToString();
            invBt.SetButtonSprites(feetItem.sprite,GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    SetEquippedPanelData(feetItem.descriptionText,feetItem.newStatsText,feetItem.sprite,feetItem.name,feetItem.rareness.ToString() + " " +
                        feetItem.FeetItemType.ToString(),GetRarenessSpriteByIndex(feetItem.rareness));
                    m_PlayerInventory.EquipCharacterItem(feetItem);

                });
     
                SetSelectedPanelData(feetItem.descriptionText,feetItem.newStatsText,feetItem.sprite,feetItem.name,feetItem.rareness.ToString() + " " +
                    feetItem.FeetItemType.ToString(),GetRarenessSpriteByIndex(feetItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        inventoryTabView.InitTabView(
            new string[]
            {
                "All", FeetItemType.Boots.ToString(), FeetItemType.Sandals.ToString(), FeetItemType.Sneakers.ToString(), FeetItemType.Ankle_Bracelets.ToString()
               
            },
            ShowButtonsByItemType);
           

        
        SetBackButtonBehaviour();
        characterPreview.gameObject.SetActive(false);


      
    }

    void Update()
    {
        
    }
}
