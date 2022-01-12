using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Customizations;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class CaharacterInventory_VC : MonoBehaviour
{
    [Inject] private PlayerInventory m_PlayerInventory;
    [Inject] private SignalBus signalBus;
    [SerializeField] private CharacterCustomizationsSlot  bodySlot,headBSlot;
    [SerializeField] private CharacterCustomizationsSlot  hairSlot;
    [SerializeField] private CharacterCustomizationsSlot  torsoSlot, legsSlot, feetSlot;
    [SerializeField] private InventoryButton inventoryButtonPrefab;
    [SerializeField] private TabView_VC inventoryTabView;
    private List<InventoryButton> inventoryButtons;
    [SerializeField] private GameObject equipPanel, selectPanel;
    [SerializeField] private Button equipBt, unEquipBt;
    [SerializeField] private TMP_Text selectedItemName,equippedItemName,selectedItemRareness,equippedItemRareness;
    [SerializeField] private TMP_Text equippedStatsText, selectedStatsText;
    [SerializeField] private Image equippedLogoImage,equippedRarenessImage, selectedLogoImage,selectedRarenessImgae;
    [SerializeField] private GameObject characterSlotsPanel;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private GameObject characterPreview;
    [SerializeField] private Sprite commonSprite, uncommonSprite, rareSprite;
    [SerializeField] private GameObject infoPanel,themeEffectPanel,slotsPanel;
    [SerializeField] private TMP_Text playerName, subscribersNum;
    private List<Sprite> rarenessSprites;

    private GenderItemType gender
    {
        get
        {
            return characterAvatar.bodyItem.GenderItemType;
        }
    }

    private CharacterAvatar characterAvatar;
    [SerializeField] private Button CharacterCustomizationButton;
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject effectsPanel;
    [SerializeField] private EffectCell effectCellPrefab;
    [SerializeField] private TMP_Text uploadedVideosNum;
    void OpenCharacterPanel()
    {
        inventoryCanvas.gameObject.SetActive(true);
        characterPanel.gameObject.SetActive(true);
        signalBus.Fire(new CharacterCustomizationVisibilityChanged()
        {
            Visibility = true
        });
        SetHomeButton();
    }

    private void SetHomeButton()
    {
        signalBus.Fire(new ChangeBackButtonSignal()
        {
            changeToHome = true,
            buttonAction = () =>
            {
                inventoryCanvas.gameObject.SetActive(false);
                characterPanel.gameObject.SetActive(false);
                signalBus.Fire(new CharacterCustomizationVisibilityChanged()
                {
                    Visibility = false
                });
            }
        });
    }

    void SetBackButton()
    {
        signalBus.Fire(new ChangeBackButtonSignal()
        {
            changeToHome = false,
            buttonAction = () => { 
                selectPanel.gameObject.SetActive(false);
                equipPanel.gameObject.SetActive(false);
                characterPreview.SetActive(true);
                inventoryTabView.gameObject.SetActive(false);
                infoPanel.gameObject.SetActive(true);
                characterSlotsPanel.gameObject.SetActive(true);
             //   themeEffectPanel.gameObject.SetActive(true);
                OnSlotClosed();
                
                SetHomeButton();
                
            }
         
        });
    }
    private void Awake()
    {
        characterAvatar = m_PlayerInventory.EquippedAvatar();
        bodySlot.SetButtonAction(OnBodyButtonClicked);
        headBSlot.SetButtonAction(OnHeadButtonClicked);
        hairSlot.SetButtonAction(OnHairButtonClicked);
        torsoSlot.SetButtonAction(OnTorsoButtonClicked);
        legsSlot.SetButtonAction(OnLegsButtonClicked);
        feetSlot.SetButtonAction(OnFeetButtonClicked);
        inventoryButtons = new List<InventoryButton>();
        rarenessSprites = new List<Sprite>() {commonSprite, uncommonSprite, rareSprite};
    //    signalBus.Subscribe<OnCharacterAvatarChanged>(UpdateItemsEffectText );
    }

    void Start()
    {

        inventoryCanvas.OnEnableAsObservable().Subscribe((unit => { OnSlotClosed(); }));
        characterPanel.OnEnableAsObservable().Subscribe((unit => OnCharacterPanelEnabled()));
        CharacterCustomizationButton.onClick.AddListener(OpenCharacterPanel);
        signalBus.Subscribe<RoomCustomizationVisibilityChanged>((() =>
        {
            characterPanel.gameObject.SetActive(false);
        }));
    }



    private void OnCharacterPanelEnabled()
    {
        UpdateSlots();
        playerName.text = PlayerDataManager.Instance.GetPlayerName().ToUpper();
        subscribersNum.text = PlayerDataManager.Instance.GetSubscribers().ToString();
        uploadedVideosNum.text = PlayerDataManager.Instance.GetPlayerTotalVideos().ToString();
       // UpdateItemsEffectText();


    }

    private void UpdateSlots()
    {
        var avatar = m_PlayerInventory.EquippedAvatar();
        ;
        var variantIndex = avatar.bodyItem.BodyIndex;
        bodySlot.SetIconSprite(avatar.bodyItem.sprite);
        bodySlot.SetRarenessSprite(GetRarenessSpriteByIndex(avatar.bodyItem.rareness));
        headBSlot.SetIconSprite(avatar.headItem.sprite);
        headBSlot.SetRarenessSprite(GetRarenessSpriteByIndex(avatar.headItem.rareness));
        if (avatar.hairItem != null)
        {
            hairSlot.SetIconSprite(avatar.hairItem.sprite);
            hairSlot.SetRarenessSprite(GetRarenessSpriteByIndex(avatar.hairItem.rareness));
        }
        else
        {
            hairSlot.SetSlotEmpty();
        }

        if (avatar.torsoItem != null)
        {
            torsoSlot.SetIconSprite(avatar.torsoItem.TorsoVariants[variantIndex]);
            torsoSlot.SetRarenessSprite(GetRarenessSpriteByIndex(avatar.torsoItem.rareness));
        }
        else
        {
            torsoSlot.SetSlotEmpty();
        }

        if (avatar.legsItem != null)
        {
            legsSlot.SetIconSprite(avatar.legsItem.LegsVariants[variantIndex]);
            legsSlot.SetRarenessSprite(GetRarenessSpriteByIndex(avatar.legsItem.rareness));
        }
        else
        {
            legsSlot.SetSlotEmpty();
        }

        if (avatar.feetItem != null)
        {
            feetSlot.SetIconSprite(avatar.feetItem.sprite);
            feetSlot.SetRarenessSprite(GetRarenessSpriteByIndex(avatar.feetItem.rareness));
        }
        else
        {
            feetSlot.SetSlotEmpty();
        }
    }

    Sprite GetRarenessSpriteByIndex(Rareness rareness)
    {
        return rarenessSprites[(int) rareness];
    }

    // void UpdateItemsEffectText()
    // {
    //     var themesNames = Enum.GetNames(typeof(ThemeType));
    //     var themesBounses= themesNames.ToDictionary(s=>s,k=>0f);
    //     
    //     var equippedItems = m_PlayerInventory.EquippedAvatar().GetThemesEffectItems();
    //     foreach (var equippedItem in equippedItems)
    //     {
    //         if (equippedItem!=null)
    //         {
    //             foreach (var themeEffect in equippedItem.affectedTheme)
    //             {
    //             
    //                 themesBounses[themeEffect.ThemeType.ToString()] += themeEffect.themePopularityFactor;
    //
    //             
    //             }    
    //         }
    //            
    //     }
    //
    //     for (int i = 0; i < effectsPanel.transform.childCount; i++)
    //     {
    //         Destroy(effectsPanel.transform.GetChild(i).gameObject);
    //     }
    //     foreach (var themeBouns in themesBounses)
    //     {
    //         if (themeBouns.Value == 0)
    //         {
    //             continue;
    //         }
    //
    //         var cell = Instantiate(effectCellPrefab, effectsPanel.transform);
    //         var value = themeBouns.Value * 100;
    //         
    //             cell.SetText(themeBouns.Key + "  : " + (int)value + "%" + "\n");
    //     }
    // }

    private void SetEquippedPanelData(string description,string newStats,Sprite logoSprite,string itemName,string rarenessText,Sprite rarenessSprite)
    {
        equippedStatsText.text =newStats;
        equippedLogoImage.sprite = logoSprite;
        equippedRarenessImage.sprite = rarenessSprite;
        equippedItemName.text = itemName;
        equippedItemRareness.text = rarenessText;
        equipPanel.SetActive(true);
    }
    private void SetSelectedPanelData(string description,string newStats,Sprite logoSprite,string itemName,string rarenessText,Sprite rarenessSprite)
    {
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

    void OnSlotOpened()
    {
        inventoryTabView.gameObject.SetActive(true);
        infoPanel.SetActive(false);
      //  themeEffectPanel.SetActive(false);
        characterPreview.SetActive(true);
        slotsPanel.SetActive(false);
    }

    void OnSlotClosed()
    {
        inventoryTabView.gameObject.SetActive(false);
        infoPanel.SetActive(true);
        //themeEffectPanel.SetActive(true);
        characterPreview.SetActive(true);
        equipPanel.gameObject.SetActive(false);
        selectPanel.SetActive(false);
        slotsPanel.SetActive(true);
        UpdateSlots();
        SetHomeButton();
    }

    private void OnBodyButtonClicked()
    {
        OnSlotOpened();
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }

        inventoryButtons.Clear();
        var equippedBody = characterAvatar.bodyItem;
        SetEquippedPanelData(equippedBody.descriptionText, equippedBody.newStatsText, equippedBody.sprite,
            equippedBody.name, equippedBody.rareness.ToString() + " " ,GetRarenessSpriteByIndex(equippedBody.rareness));

        foreach (var item in m_PlayerInventory.OwnedCharacterItems)
        {
            if (item.GetType() != typeof(BodyItem))
                continue;

            BodyItem bodyItem = (BodyItem) item;

            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = " ";
            invBt.SetButtonSprites(bodyItem.sprite, GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(() =>
                {
                    SetEquippedPanelData(bodyItem.descriptionText, bodyItem.newStatsText, bodyItem.sprite,
                        bodyItem.name, bodyItem.rareness.ToString() + " " +
                                      " ", GetRarenessSpriteByIndex(bodyItem.rareness));
                    if (gender!=bodyItem.GenderItemType)
                    {
                        characterAvatar.bodyItem = bodyItem;

                        characterAvatar.headItem = bodyItem.GenderItemType == GenderItemType.Male
                            ? m_PlayerInventory.GetDefaultMaleHead
                            : m_PlayerInventory.GetDefaultFemaleHead;
                        characterAvatar.hairItem = null;
                        characterAvatar.feetItem = null;
                        characterAvatar.legsItem = null;
                        characterAvatar.torsoItem = null;

                    }
                    else
                    {
                        characterAvatar.bodyItem = bodyItem;

                    }
                    m_PlayerInventory.ChangeAvatar(characterAvatar);
                    OnSlotClosed();

                });

                SetSelectedPanelData(bodyItem.descriptionText, bodyItem.newStatsText, bodyItem.sprite, bodyItem.name,
                    bodyItem.rareness.ToString() + " " , GetRarenessSpriteByIndex(bodyItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }
        SetBackButton();

    }

    public void OnHeadButtonClicked()
    {
        OnSlotOpened();
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedHead = characterAvatar.headItem;
        SetEquippedPanelData(equippedHead.descriptionText, equippedHead.newStatsText, equippedHead.sprite,
            equippedHead.name, equippedHead.rareness.ToString() + " " +
                               equippedHead.HeadItemType.ToString(),GetRarenessSpriteByIndex(equippedHead.rareness));

        foreach (var  item in m_PlayerInventory.OwnedCharacterItems)
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
                    equipPanel.gameObject.SetActive(false);
                    selectPanel.gameObject.SetActive(false);
                    SetEquippedPanelData(headItem.descriptionText,headItem.newStatsText,headItem.sprite,headItem.name,headItem.rareness.ToString() + " " +
                        headItem.HeadItemType.ToString(),GetRarenessSpriteByIndex(headItem.rareness));
                    if (gender==headItem.GenderItemType)
                    {
                        characterAvatar.headItem = headItem;

                    }
                    else
                    {
                        characterAvatar.headItem = gender == GenderItemType.Male
                            ? m_PlayerInventory.GetDefaultMaleHead
                            : m_PlayerInventory.GetDefaultFemaleHead;
                    }
                    m_PlayerInventory.ChangeAvatar(characterAvatar);
                    OnSlotClosed();

                });
     
                SetSelectedPanelData(headItem.descriptionText,headItem.newStatsText,headItem.sprite,headItem.name,headItem.rareness.ToString() + " " +
                    headItem.HeadItemType.ToString(),GetRarenessSpriteByIndex(headItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);

            SetBackButton();
        }

        var names = Enum.GetNames(typeof(HeadItemType)).ToList();

        inventoryTabView.InitTabView(
            names,
            ShowButtonsByItemType);
        inventoryTabView.SetButtonsTabVisibly(true);
        SetBackButtonBehaviour();
        //characterPreview.gameObject.SetActive(false);
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

     void OnHairButtonClicked()
     {

         OnSlotOpened();
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();

        var equippedHair = characterAvatar.hairItem;
        if (equippedHair!=null)
        {
            SetEquippedPanelData(equippedHair.descriptionText, equippedHair.newStatsText, equippedHair.sprite,
                equippedHair.name, equippedHair.rareness.ToString() + " " +
                                   equippedHair.hairItemType.ToString(),GetRarenessSpriteByIndex(equippedHair.rareness));
        }
       

        foreach (var item in m_PlayerInventory.OwnedCharacterItems)
        {
            if (item.GetType() != typeof(HairItem))
                continue;
            HairItem hairItem =(HairItem)item;
                InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = hairItem.hairItemType.ToString();
            invBt.SetButtonSprites(hairItem.sprite,GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    SetEquippedPanelData(hairItem.descriptionText,hairItem.newStatsText,hairItem.sprite,hairItem.name,hairItem.rareness.ToString() + " " +
                        hairItem.hairItemType.ToString(),GetRarenessSpriteByIndex(hairItem.rareness));
                    if (gender==hairItem.GenderItemType)
                    {
                        characterAvatar.hairItem = hairItem;

                    }
                    else
                    {
                        characterAvatar.hairItem = null;
                        hairSlot.SetSlotEmpty();
                    }
                    m_PlayerInventory.ChangeAvatar(characterAvatar);

                    OnSlotClosed();

                });
     
                SetSelectedPanelData(hairItem.descriptionText,hairItem.newStatsText,hairItem.sprite,hairItem.name,hairItem.rareness.ToString() + " " +
                    hairItem.hairItemType.ToString(),GetRarenessSpriteByIndex(hairItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
            SetBackButton();

        }

        var names = Enum.GetNames(typeof(HairItemType)).ToList();
 
        
        inventoryTabView.InitTabView(
            names,
            ShowButtonsByItemType);
        SetBackButtonBehaviour();
        //characterPreview.gameObject.SetActive(false);

    }

     void OnTorsoButtonClicked()
     {
         OnSlotOpened();
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedTorso = characterAvatar.torsoItem;
        int bodyIndex = characterAvatar.bodyItem.BodyIndex;
        if (equippedTorso!=null)
        {
            SetEquippedPanelData(equippedTorso.descriptionText, equippedTorso.newStatsText, equippedTorso.TorsoVariants[bodyIndex],
                equippedTorso.name, equippedTorso.rareness.ToString() + " " +
                                    equippedTorso.TorsoItemType.ToString(),GetRarenessSpriteByIndex(equippedTorso.rareness));
        }
    

        foreach (var item in m_PlayerInventory.OwnedCharacterItems)
        {
            if(item.GetType()!=typeof(TorsoItem))
                continue;
            TorsoItem torsoItem =(TorsoItem) item;
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = torsoItem.TorsoItemType.ToString();
            invBt.SetButtonSprites(torsoItem.TorsoVariants[bodyIndex],GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    SetEquippedPanelData(torsoItem.descriptionText,torsoItem.newStatsText,torsoItem.TorsoVariants[bodyIndex],torsoItem.name,torsoItem.rareness.ToString() + " " +
                        torsoItem.TorsoItemType.ToString(),GetRarenessSpriteByIndex(torsoItem.rareness));
                
                    if (gender==torsoItem.GenderItemType)
                    {
                        characterAvatar.torsoItem = torsoItem;

                    }
                    else
                    {
                        characterAvatar.torsoItem = null;
                        torsoSlot.SetSlotEmpty();
                    }
                    m_PlayerInventory.ChangeAvatar(characterAvatar);

                    OnSlotClosed();

                });
     
                SetSelectedPanelData(torsoItem.descriptionText,torsoItem.newStatsText,torsoItem.TorsoVariants[bodyIndex],torsoItem.name,torsoItem.rareness.ToString() + " " +
                    torsoItem.TorsoItemType.ToString(),GetRarenessSpriteByIndex(torsoItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        var names = Enum.GetNames(typeof(TorsoItemType)).ToList();
        names.Insert(0,"All");
        inventoryTabView.InitTabView(names,
            ShowButtonsByItemType);
        SetBackButtonBehaviour();
        //characterPreview.gameObject.SetActive(false);

        SetBackButton();

       
    }

     void OnLegsButtonClicked()
     {
         OnSlotOpened();
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedLegs = characterAvatar.legsItem;
        int bodyIndex = characterAvatar.bodyItem.BodyIndex;
        if (equippedLegs!=null)
        {
            SetEquippedPanelData(equippedLegs.descriptionText, equippedLegs.newStatsText, equippedLegs.LegsVariants[bodyIndex],
                equippedLegs.name, equippedLegs.rareness.ToString() + " " +
                                   equippedLegs.LegsType.ToString(),GetRarenessSpriteByIndex(equippedLegs.rareness)); 
        }
   

        foreach (var item in m_PlayerInventory.OwnedCharacterItems)
        {
            if(item.GetType()!=typeof(LegsItem))
                continue;
            LegsItem legsItem = (LegsItem) item;
            
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = legsItem.LegsType.ToString();
            invBt.SetButtonSprites(legsItem.LegsVariants[bodyIndex],GetRarenessSpriteByIndex(item.rareness));
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    SetEquippedPanelData(legsItem.descriptionText,legsItem.newStatsText,legsItem.LegsVariants[bodyIndex],legsItem.name,legsItem.rareness.ToString() + " " +
                        legsItem.LegsType.ToString(),GetRarenessSpriteByIndex(legsItem.rareness));

                    if (gender==legsItem.GenderItemType)
                    {
                        characterAvatar.legsItem = legsItem;

                    }
                    else
                    {
                        characterAvatar.legsItem = null;
                        legsSlot.SetSlotEmpty();
                    }
                    m_PlayerInventory.ChangeAvatar(characterAvatar);

                    OnSlotClosed();

                });
     
                SetSelectedPanelData(legsItem.descriptionText,legsItem.newStatsText,legsItem.LegsVariants[bodyIndex],legsItem.name,legsItem.rareness.ToString() + " " +
                    legsItem.LegsType.ToString(),GetRarenessSpriteByIndex(legsItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        var names = Enum.GetNames(typeof(LegsItemType)).ToList();
        inventoryTabView.InitTabView(
            names,
            ShowButtonsByItemType);
        SetBackButtonBehaviour();
        //characterPreview.gameObject.SetActive(false);

        SetBackButton();

        
    }

     void OnFeetButtonClicked()
     {

         OnSlotOpened();
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedFeet = characterAvatar.feetItem;
        if (equippedFeet!=null)
        {
            SetEquippedPanelData(equippedFeet.descriptionText, equippedFeet.newStatsText, equippedFeet.sprite,
                equippedFeet.name, equippedFeet.rareness.ToString() + " " +
                                   equippedFeet.FeetItemType.ToString(), GetRarenessSpriteByIndex(equippedFeet.rareness));
        }


        foreach (var  item in m_PlayerInventory.OwnedCharacterItems)
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
                    if (gender==feetItem.GenderItemType)
                    {
                        characterAvatar.feetItem = feetItem;

                    }
                    else
                    {
                        characterAvatar.feetItem = null;
                        feetSlot.SetSlotEmpty();
                    }
                    m_PlayerInventory.ChangeAvatar(characterAvatar);
                    OnSlotClosed();

                });
     
                SetSelectedPanelData(feetItem.descriptionText,feetItem.newStatsText,feetItem.sprite,feetItem.name,feetItem.rareness.ToString() + " " +
                    feetItem.FeetItemType.ToString(),GetRarenessSpriteByIndex(feetItem.rareness));

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }
        var names = Enum.GetNames(typeof(FeetItemType)).ToList();

        inventoryTabView.InitTabView(
          names,
            ShowButtonsByItemType);
           

        
        SetBackButtonBehaviour();
        //characterPreview.gameObject.SetActive(false);

        SetBackButton();

      
    }

     void UnEquipCharacterSlots()
     {
         torsoSlot.SetSlotEmpty();
         legsSlot.SetSlotEmpty();
         feetSlot.SetSlotEmpty();
         signalBus.Fire(new OnCharacterAvatarChanged()
         {
             
         });
     }

}
