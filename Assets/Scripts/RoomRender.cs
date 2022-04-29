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
        tempLayout = new RoomLayout(currentRoomLayout);

    }

     void  PopulateRoomLayout()
    {
           currentRoomObjects.ForEach((go => Destroy(go.gameObject) ));
           currentRoomObjects.Clear();
        foreach (var themeCustomizationItem in PlayerInventory.EquippedThemeEffectRoomItems)
        {

            var loadOp = themeCustomizationItem.itemPrefab.InstantiateAsync(roomTransform);
            loadOp.WaitForCompletion();
           
            currentRoomObjects.Add(loadOp.Result.gameObject.GetComponent<RoomObjectData>());
            
            
        }

        foreach (var vqItem in PlayerInventory.EquippedVideoQualityRoomItems)
        {
            if (vqItem!=null)
            {

                var loadOp = vqItem.itemPrefab.InstantiateAsync(roomTransform);
                loadOp.WaitForCompletion();
                
                currentRoomObjects.Add(loadOp.Result.gameObject.GetComponent<RoomObjectData>());  
            }
          
        }
    }

   
     void OnTestVideoQualityItem(TestRoomVideoQualityITemSignal testRoomVideoQualityITemSignal)
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
         loadOp.WaitForCompletion();
        var go = loadOp.Result;
        var objecdata = go.GetComponent<RoomObjectData>();
        currentRoomObjects.Add(objecdata);
        tempLayout.equippedVCITems.Add(objecdata.assetName);

        PlacementVFX(go);
    }

     void OnTestRoomThemeItem(TestRoomThemeItemSignal testRoomThemeItem)
    {
        
            var item = testRoomThemeItem.ThemeCustomizationItem;
           
            if (item.GetType()==typeof(RoomObject))
            {
                var condObj = (RoomObject) item;
                if (condObj.ConditionalSlot!=ItemSlotType.none)
                {
                    if (!currentRoomObjects.Exists((obj) => obj.slotItemType == condObj.ConditionalSlot))
                    {
                        signalBus.Fire(new OpenDefaultMessagePopUpSignal()
                        {
                            message = "Can't Place This Object Please Place  "+ condObj.ConditionalSlot.ToString()  
                        });
                        return;

                    };
                }
            }
        var obj = currentRoomObjects.Find(ob => (ob.slotItemType == item.SlotType) || (ob.assetName==item.name));
        if (obj)
        {
            tempLayout.equippedThemeITems.Remove(obj.assetName);
            Destroy(obj.gameObject);
            currentRoomObjects.Remove(obj);
        }
        var loadOp = Addressables.InstantiateAsync(item.itemPrefab,roomTransform);
         loadOp.WaitForCompletion();
        var go = loadOp.Result;
        var objecdata = go.GetComponent<RoomObjectData>();
        currentRoomObjects.Add(objecdata);
        tempLayout.equippedThemeITems.Add(objecdata.assetName);
        
        PlacementVFX(go);
    }

    void PlacementVFX( GameObject go)
    {
        go.SetActive(false);
        VFX_placementObjectEffect.SetActive(true);
        SpriteRenderer goSpriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
        VFX_scaler.transform.position = goSpriteRenderer.gameObject.transform.position;
        Vector3 absScale = new Vector3(Mathf.Abs(go.transform.localScale.x), Mathf.Abs(go.transform.localScale.y), Mathf.Abs(go.transform.localScale.z));
        Vector3 realScale = new Vector3(1, 1, 1) + (absScale - new Vector3(0.7330219f, 0.7330219f, 0.7330219f));
        if (go.transform.localScale.x < 0)
            realScale.x *= -1;
        if (go.transform.localScale.y < 0)
            realScale.y *= -1;
        if (go.transform.localScale.z < 0)
            realScale.z *= -1;
        VFX_scaler.transform.localScale = realScale;
        Sprite goSprite = goSpriteRenderer.sprite;
        VFX_placementObjectEffect.GetComponent<SpriteRenderer>().sortingOrder = goSpriteRenderer.sortingOrder;
        VFX_placementObjectEffect.GetComponent<SpriteRenderer>().sprite = goSprite;
        VFX_placementObjectEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = goSpriteRenderer.sortingOrder-1;
        VFX_placementObjectEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = goSprite;
        VFX_placementObjectEffect.transform.GetChild(0).GetComponent<SpriteMask>().sprite = goSprite;
        VFX_placementObjectEffect.GetComponent<Animator>().Play("Apearing Object");
        StartCoroutine(WaitVFXAnimation(VFX_placementObjectEffect.GetComponent<Animator>(), go));
    }

    IEnumerator WaitVFXAnimation(Animator anim, GameObject realGo)
    {
        yield return null;
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Apearing Object"))
        {
            yield return null;
        }
        VFX_placementObjectEffect.SetActive(false);
        realGo.SetActive(true);
    }
    

    public  void OnSaveRoomLayout()
    {
        if (tempLayout!=null)
        {
            currentRoomLayout=new RoomLayout(tempLayout);

        }

       PlayerInventory.UpdateRoomData(currentRoomLayout);
      gameObject.SetActive(false);
      gameObject.SetActive(true);
    }
    
    public void PopulateDataSlots()
    {
       
    }
}





