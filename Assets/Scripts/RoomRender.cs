using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Customizations;
using ModestTree;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Zenject;

public class RoomRender : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlayerInventory PlayerInventory;

 
    [SerializeField] private RoomLayout currentRoomLayout=null;
    private RoomLayout tempLayout;
    [SerializeField] private Transform roomTransform;
    private List<RoomObjectData> currentRoomObjects;

    [SerializeField] GameObject VFX_scaler;
    [SerializeField] GameObject VFX_placementObjectEffect;
    private void Awake()
    {
        currentRoomObjects = new List<RoomObjectData>();
        tempLayout = currentRoomLayout;
        roomTransform = transform;
        currentRoomLayout = PlayerInventory.GetRoomLayout();
    }

    void Start()
    {
        
        signalBus.Subscribe<TestRoomThemeItemSignal>(OnTestRoomThemeItem);
        signalBus.Subscribe<TestRoomVideoQualityITemSignal>(OnTestVideoQualityItem);


        signalBus.Subscribe<SaveRoomLayoutSignal>((signal => { OnSaveRoomLayout(); }));
        signalBus.Subscribe<DiscardRoomLayoutSignal>((signal =>
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }));
   
    }

    private void OnEnable()
    {
     
        PopulateRoomLayout();
        print("Room Enabled");
        tempLayout = currentRoomLayout;

    }

    async void  PopulateRoomLayout()
    {
           currentRoomObjects.ForEach((go => Destroy(go.gameObject) ));
           currentRoomObjects.Clear();
        foreach (var themeCustomizationItem in PlayerInventory.EquippedThemeEffectRoomItems)
        {
            var loadOp = themeCustomizationItem.itemPrefab.InstantiateAsync(roomTransform);
            await loadOp.Task;
            currentRoomObjects.Add(loadOp.Result.GetComponent<RoomObjectData>());
            
        }

        foreach (var vqItem in PlayerInventory.EquippedVideoQualityRoomItems)
        {
            if (vqItem!=null)
            {
                var loadOp = vqItem.itemPrefab.InstantiateAsync(roomTransform);
                await loadOp.Task;
                currentRoomObjects.Add(loadOp.Result.GetComponent<RoomObjectData>());  
            }
          
        }
    }

   
    async void OnTestVideoQualityItem(TestRoomVideoQualityITemSignal testRoomVideoQualityITemSignal)
    {
        
        var item = testRoomVideoQualityITemSignal.VideoQualityCustomizationItem;
        var obj = currentRoomObjects.Find(ob => ob.slotItemType == item.SlotType);
        if (obj)
        {
            tempLayout.equippedVCITems.Remove(obj.assetName);
            Destroy(obj.gameObject);
            currentRoomObjects.Remove(obj);

        }
        var loadOp = Addressables.InstantiateAsync(item.itemPrefab,roomTransform);
        await loadOp.Task;
        var go = loadOp.Result;
        var objecdata = go.GetComponent<RoomObjectData>();
        currentRoomObjects.Add(objecdata);
        tempLayout.equippedVCITems.Add(objecdata.assetName);

        VFX_placementObjectEffect.SetActive(true);
        VFX_scaler.transform.position = go.transform.position;
        VFX_scaler.transform.localScale = new Vector3(1,1,1) + ( go.transform.localScale - new Vector3(0.7330219f, 0.7330219f, 0.7330219f));
        Sprite goSprite = go.GetComponent<SpriteRenderer>().sprite;
        VFX_placementObjectEffect.GetComponent<SpriteRenderer>().sprite = goSprite;
        VFX_placementObjectEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = goSprite;
        VFX_placementObjectEffect.transform.GetChild(0).GetComponent<SpriteMask>().sprite = goSprite;
        VFX_placementObjectEffect.GetComponent<Animator>().Play("Apearing Object");
    }

    async void OnTestRoomThemeItem(TestRoomThemeItemSignal testRoomThemeItem)
    {
        var item = testRoomThemeItem.ThemeCustomizationItem;
        var obj = currentRoomObjects.Find(ob => (ob.slotItemType == item.SlotType) || (ob.assetName==item.name));
        if (obj)
        {
            print("Old Item Destroied");
            tempLayout.equippedThemeITems.Remove(obj.assetName);
            Destroy(obj.gameObject);
            currentRoomObjects.Remove(obj);
        }
        var loadOp = Addressables.InstantiateAsync(item.itemPrefab,roomTransform);
        await loadOp.Task;
        var go = loadOp.Result;
        var objecdata = go.GetComponent<RoomObjectData>();
        currentRoomObjects.Add(objecdata);
        tempLayout.equippedThemeITems.Add(objecdata.assetName);

        VFX_placementObjectEffect.SetActive(true);
        VFX_scaler.transform.position = go.transform.position;
        VFX_scaler.transform.localScale = new Vector3(1, 1, 1) + (go.transform.localScale - new Vector3(0.7330219f, 0.7330219f, 0.7330219f));
        Sprite goSprite = go.GetComponent<SpriteRenderer>().sprite;
        VFX_placementObjectEffect.GetComponent<SpriteRenderer>().sprite = goSprite;
        VFX_placementObjectEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = goSprite;
        VFX_placementObjectEffect.transform.GetChild(0).GetComponent<SpriteMask>().sprite = goSprite;
        VFX_placementObjectEffect.GetComponent<Animator>().Play("Apearing Object");
    }

    
    

    public  void OnSaveRoomLayout()
    {
        if (tempLayout!=null)
        {
            currentRoomLayout = tempLayout;

        }

       PlayerInventory.UpdateRoomData(currentRoomLayout);
      gameObject.SetActive(false);
      gameObject.SetActive(true);
    }
    
    public void PopulateDataSlots()
    {
       
    }
}





