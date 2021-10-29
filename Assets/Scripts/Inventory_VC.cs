using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class Inventory_VC : MonoBehaviour
{
    [Inject] private PlayerInventory playerInventory;
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
        headBt.image.sprite = playerInventory.currentHead.logoSprite;
        // faceBt.image.sprite = playerInventory.currentFace.logoSprite;
        // torsoBt.image.sprite = playerInventory.currentTorso.logoSprite;
        // legsBt.image.sprite = playerInventory.currentLegs.logoSprite;
        // feetBt.image.sprite = playerInventory.currentFeet.logoSprite;
    }

    public void OnHeadButtonClicked()
    {
        inventoryTabView.gameObject.SetActive(true);
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }
             inventoryButtons.Clear();
             var equippedHead = playerInventory.currentHead;
        equippedText.text =equippedHead.descriptionText;
        equippedStatsText.text =equippedHead.newStatsText;
        equippedImage.sprite = equippedHead.logoSprite;
        equippedItemName.text = equippedHead.name;
        equippedItemRareness.text = equippedHead.rareness.ToString()+" "+equippedHead.HeadItemType.ToString();
        equipPanel.SetActive(true);

        foreach (var headItem in playerInventory.HeadItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = headItem.HeadItemType.ToString();
            invBt.SetButtonLogo(headItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>playerInventory.EquipHead(headItem));
     
                selectedText.text = headItem.descriptionText;
                selectedStatsText.text = headItem.newStatsText;
                selectedImage.sprite = headItem.logoSprite;
                selectPanel.gameObject.SetActive(true);
                selectedItemName.text = headItem.name;
                selectedItemRareness.text = headItem.rareness.ToString() +headItem.HeadItemType.ToString();

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });

            inventoryButtons.Add(invBt);
        }

        inventoryTabView.InitTabView(
            new string[] {"All", HeadItemType.Hats.ToString(), HeadItemType.Hair_Style.ToString()},
            ShowButtonsByItemType);
    }

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
    public void OnFaceButtonClicked()
    {
       
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }

        var equippedFace = playerInventory.currentFace;
        equippedText.text =equippedFace.descriptionText;
        equippedStatsText.text =equippedFace.newStatsText;
        equippedImage.sprite = equippedFace.logoSprite;
        equippedItemName.text = equippedFace.name;
        equippedItemRareness.text = equippedFace.rareness.ToString()+equippedFace.FaceItemType.ToString();
        equipPanel.SetActive(true);

        foreach (var faceItem in playerInventory.FaceItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = faceItem.faceSprite.ToString();
            invBt.SetButtonLogo(faceItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>playerInventory.EquipFace(faceItem));
     
                selectedText.text = faceItem.descriptionText;
                selectedStatsText.text = faceItem.newStatsText;
                selectedImage.sprite = faceItem.logoSprite;
                selectPanel.gameObject.SetActive(true);
                selectedItemName.text = faceItem.name;
                selectedItemRareness.text = faceItem.rareness.ToString() +faceItem.FaceItemType.ToString();

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });
           

        }

        inventoryTabView.InitTabView(
            new string[] {"All", FaceItemType.Glasses.ToString(), FaceItemType.Masks.ToString(),FaceItemType.Piercings.ToString(),FaceItemType.Bold_Makeup_Applications.ToString()},
            ShowButtonsByItemType);
    }

    public void OnTorsoButtonClicked()
    {
       
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }

        var equippedHead = playerInventory.currentHead;
        equippedText.text =equippedHead.descriptionText;
        equippedStatsText.text =equippedHead.newStatsText;
        equippedImage.sprite = equippedHead.logoSprite;
        equippedItemName.text = equippedHead.name;
        equippedItemRareness.text = equippedHead.rareness.ToString()+equippedHead.GetType().ToString();
        equipPanel.SetActive(true);

        foreach (var headItem in playerInventory.HeadItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = headItem.HeadItemType.ToString();
            invBt.SetButtonLogo(headItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>playerInventory.EquipHead(headItem));
     
                selectedText.text = headItem.descriptionText;
                selectedStatsText.text = headItem.newStatsText;
                selectedImage.sprite = headItem.logoSprite;
                selectPanel.gameObject.SetActive(true);
                selectedItemName.text = headItem.name;
                selectedItemRareness.text = headItem.rareness.ToString() +headItem.HeadItemType.ToString();

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });
           

        }

        inventoryTabView.InitTabView(
            new string[] {"All", HeadItemType.Hats.ToString(), HeadItemType.Hair_Style.ToString()},
            ShowButtonsByItemType);
    }

    public void OnLegsButtonClicked()
    {
      
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }

        var equippedHead = playerInventory.currentHead;
        equippedText.text =equippedHead.descriptionText;
        equippedStatsText.text =equippedHead.newStatsText;
        equippedImage.sprite = equippedHead.logoSprite;
        equippedItemName.text = equippedHead.name;
        equippedItemRareness.text = equippedHead.rareness.ToString()+equippedHead.GetType().ToString();
        equipPanel.SetActive(true);

        foreach (var headItem in playerInventory.HeadItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = headItem.HeadItemType.ToString();
            invBt.SetButtonLogo(headItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>playerInventory.EquipHead(headItem));
     
                selectedText.text = headItem.descriptionText;
                selectedStatsText.text = headItem.newStatsText;
                selectedImage.sprite = headItem.logoSprite;
                selectPanel.gameObject.SetActive(true);
                selectedItemName.text = headItem.name;
                selectedItemRareness.text = headItem.rareness.ToString() +headItem.HeadItemType.ToString();

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });
           

        }

        inventoryTabView.InitTabView(
            new string[] {"All", HeadItemType.Hats.ToString(), HeadItemType.Hair_Style.ToString()},
            ShowButtonsByItemType);
    }

    public void OnFeetButtonClicked()
    {
      
        foreach (var button in inventoryButtons)
        {
            Destroy(button.gameObject);
        }

        var equippedHead = playerInventory.currentHead;
        equippedText.text =equippedHead.descriptionText;
        equippedStatsText.text =equippedHead.newStatsText;
        equippedImage.sprite = equippedHead.logoSprite;
        equippedItemName.text = equippedHead.name;
        equippedItemRareness.text = equippedHead.rareness.ToString()+equippedHead.GetType().ToString();
        equipPanel.SetActive(true);

        foreach (var headItem in playerInventory.HeadItems)
        {
            InventoryButton invBt =
                Instantiate(inventoryButtonPrefab.gameObject, inventoryTabView.buttonsView.transform)
                    .GetComponent<InventoryButton>();
            invBt.Type = headItem.HeadItemType.ToString();
            invBt.SetButtonLogo(headItem.logoSprite);
            invBt.SetButtonAction(() =>
            {
                equipBt.onClick.RemoveAllListeners();
                equipBt.onClick.AddListener(()=>playerInventory.EquipHead(headItem));
     
                selectedText.text = headItem.descriptionText;
                selectedStatsText.text = headItem.newStatsText;
                selectedImage.sprite = headItem.logoSprite;
                selectPanel.gameObject.SetActive(true);
                selectedItemName.text = headItem.name;
                selectedItemRareness.text = headItem.rareness.ToString() +headItem.HeadItemType.ToString();

                // unEquipBt.onClick.RemoveAllListeners();
                // unEquipBt.onClick.AddListener(()=>);
            });
           

        }

        inventoryTabView.InitTabView(
            new string[] {"All", HeadItemType.Hats.ToString(), HeadItemType.Hair_Style.ToString()},
            ShowButtonsByItemType);
    }

    void Update()
    {
        
    }
}
