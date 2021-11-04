using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class CaharacterInventory_VC : MonoBehaviour
{
    [Inject] private PlayerInventory m_PlayerInventory;
    [SerializeField] private Button headBt, faceBt, torsoBt, legsBt, feetBt;
    [SerializeField] private InventoryButton inventoryButtonPrefab;
    [SerializeField] private TabView_VC inventoryTabView;
    private List<InventoryButton> inventoryButtons;
    [SerializeField] private GameObject equipPanel, selectPanel;
    [SerializeField] private Button equipBt, unEquipBt;
    [SerializeField] private TMP_Text selectedItemName,equippedItemName,selectedItemRareness,equippedItemRareness;
    [SerializeField] private TMP_Text equippedText, selectedText;
    [SerializeField] private TMP_Text equippedStatsText, selectedStatsText;
    [SerializeField] private Image equippedImage, selectedImage;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject characterSlotsPanel,roomSlotsPanel,buttonsPanel;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private GameObject characterPreview;
    void Start()
    {
        headBt.onClick.AddListener(OnHeadButtonClicked);
        faceBt.onClick.AddListener(OnFaceButtonClicked);
        torsoBt.onClick.AddListener(OnTorsoButtonClicked);
        legsBt.onClick.AddListener(OnLegsButtonClicked);
        feetBt.onClick.AddListener(OnFeetButtonClicked);
        inventoryButtons = new List<InventoryButton>();
    }

    private void OnEnable()
    { 
         headBt.image.sprite = m_PlayerInventory?.currentHead.logoSprite;
         faceBt.image.sprite = m_PlayerInventory?.currentFace.logoSprite;
         torsoBt.image.sprite = m_PlayerInventory?.currentTorso.logoSprite;
         legsBt.image.sprite = m_PlayerInventory?.currentLegs.logoSprite;
         feetBt.image.sprite = m_PlayerInventory?.currentFeet.logoSprite;
    }

   

    private void SetEquippedPanelData(string description,string newStats,Sprite logoSprite,string itemName,string rarenessText)
    {
        equippedText.text = description;
        equippedStatsText.text =newStats;
        equippedImage.sprite = logoSprite;
        equippedItemName.text = itemName;
        equippedItemRareness.text = rarenessText;
        equipPanel.SetActive(true);
    }
    private void SetSelectedPanelData(string description,string newStats,Sprite logoSprite,string itemName,string rarenessText)
    {
        selectedText.text = description;
        selectedStatsText.text =newStats;
        selectedImage.sprite = logoSprite;
        selectedItemName.text = itemName;
        selectedItemRareness.text = rarenessText;
        selectPanel.gameObject.SetActive(true);    }
    void ShowButtonsByItemType(string type)
    {
        print("Filter clicked "+ type);
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
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedHead = m_PlayerInventory.currentHead;
        SetEquippedPanelData(equippedHead.descriptionText, equippedHead.newStatsText, equippedHead.logoSprite,
            equippedHead.name, equippedHead.rareness.ToString() + " " +
                               equippedHead.HeadItemType.ToString());

        foreach (var headItem in m_PlayerInventory.HeadItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = headItem.HeadItemType.ToString();
            invBt.SetButtonLogo(headItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    SetEquippedPanelData(headItem.descriptionText,headItem.newStatsText,headItem.logoSprite,headItem.name,headItem.rareness.ToString() + " " +
                        headItem.HeadItemType.ToString());
                    m_PlayerInventory.EquipHead(headItem);

                });
     
                SetSelectedPanelData(headItem.descriptionText,headItem.newStatsText,headItem.logoSprite,headItem.name,headItem.rareness.ToString() + " " +
                    headItem.HeadItemType.ToString());

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
        backButton.onClick.RemoveAllListeners();

        backButton.onClick.AddListener((() =>
        {
            characterPreview.gameObject.SetActive(true);

            inventoryTabView.gameObject.SetActive(false);
            equipPanel.SetActive(false);
            selectPanel.SetActive(false);
            backButton.onClick.AddListener(() =>
            {
                backButton.onClick.RemoveAllListeners();

                inventoryCanvas.gameObject.SetActive(false);
            });
        }));
    }

     void OnFaceButtonClicked()
    {
       
        inventoryTabView.gameObject.SetActive(true);
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedFace = m_PlayerInventory.currentFace;
        SetEquippedPanelData(equippedFace.descriptionText, equippedFace.newStatsText, equippedFace.logoSprite,
            equippedFace.name, equippedFace.rareness.ToString() + " " +
                               equippedFace.FaceItemType.ToString());

        foreach (var faceItem in m_PlayerInventory.FaceItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = faceItem.FaceItemType.ToString();
            invBt.SetButtonLogo(faceItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    m_PlayerInventory.EquipFace(faceItem);
                    SetEquippedPanelData(faceItem.descriptionText,faceItem.newStatsText,faceItem.logoSprite,faceItem.name,faceItem.rareness.ToString() + " " +
                        faceItem.FaceItemType.ToString());

                });
     
                SetSelectedPanelData(faceItem.descriptionText,faceItem.newStatsText,faceItem.logoSprite,faceItem.name,faceItem.rareness.ToString() + " " +
                    faceItem.FaceItemType.ToString());

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
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedTorso = m_PlayerInventory.currentTorso;
        SetEquippedPanelData(equippedTorso.descriptionText, equippedTorso.newStatsText, equippedTorso.logoSprite,
            equippedTorso.name, equippedTorso.rareness.ToString() + " " +
                                equippedTorso.TorsoItemType.ToString());

        foreach (var torsoItem in m_PlayerInventory.TorsoItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = torsoItem.TorsoItemType.ToString();
            invBt.SetButtonLogo(torsoItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    m_PlayerInventory.EquipTorso(torsoItem);
                    SetEquippedPanelData(torsoItem.descriptionText,torsoItem.newStatsText,torsoItem.logoSprite,torsoItem.name,torsoItem.rareness.ToString() + " " +
                        torsoItem.TorsoItemType.ToString());
                });
     
                SetSelectedPanelData(torsoItem.descriptionText,torsoItem.newStatsText,torsoItem.logoSprite,torsoItem.name,torsoItem.rareness.ToString() + " " +
                    torsoItem.TorsoItemType.ToString());

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
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedLegs = m_PlayerInventory.currentLegs;
        SetEquippedPanelData(equippedLegs.descriptionText, equippedLegs.newStatsText, equippedLegs.logoSprite,
            equippedLegs.name, equippedLegs.rareness.ToString() + " " +
                               equippedLegs.LegsType.ToString());

        foreach (var legsItem in m_PlayerInventory.LegsItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = legsItem.LegsType.ToString();
            invBt.SetButtonLogo(legsItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    m_PlayerInventory.EquipLegs(legsItem);
                    SetEquippedPanelData(legsItem.descriptionText,legsItem.newStatsText,legsItem.logoSprite,legsItem.name,legsItem.rareness.ToString() + " " +
                        legsItem.LegsType.ToString());
                });
     
                SetSelectedPanelData(legsItem.descriptionText,legsItem.newStatsText,legsItem.logoSprite,legsItem.name,legsItem.rareness.ToString() + " " +
                    legsItem.LegsType.ToString());

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
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
        inventoryButtons.Clear();
        var equippedFeet = m_PlayerInventory.currentFeet;
        SetEquippedPanelData(equippedFeet.descriptionText, equippedFeet.newStatsText, equippedFeet.logoSprite,
            equippedFeet.name, equippedFeet.rareness.ToString() + " " +
                               equippedFeet.FeetItemType.ToString());

        foreach (var feetItem in m_PlayerInventory.FeetItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = feetItem.FeetItemType.ToString();
            invBt.SetButtonLogo(feetItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>
                {
                    SetEquippedPanelData(feetItem.descriptionText,feetItem.newStatsText,feetItem.logoSprite,feetItem.name,feetItem.rareness.ToString() + " " +
                        feetItem.FeetItemType.ToString());
                    m_PlayerInventory.EquipFeet(feetItem);

                });
     
                SetSelectedPanelData(feetItem.descriptionText,feetItem.newStatsText,feetItem.logoSprite,feetItem.name,feetItem.rareness.ToString() + " " +
                    feetItem.FeetItemType.ToString());

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
