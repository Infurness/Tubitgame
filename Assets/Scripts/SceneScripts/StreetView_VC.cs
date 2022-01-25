using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Customizations;

public class StreetView_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] PlayerInventory playerInventory;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform roomViewTransform;
    [SerializeField] private Transform houseViewTransform;
    [SerializeField] private Transform streetViewTransform;

    [SerializeField] private Transform basicHousePivot;
    [SerializeField] private Transform niceApartmentPivot;
    [SerializeField] private Transform hugeHousePivot;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Button basicHouseButton;
    [SerializeField] private Button niceApartmentButton;
    [SerializeField] private Button hugeHouseButton;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<SnapToNeighborhoodViewSignal>(SetButtonsPosition);
        basicHouseButton.onClick.AddListener(()=>ShopButton("BasicHouse"));
        niceApartmentButton.onClick.AddListener(() => ShopButton("ResidentialHouse"));
        hugeHouseButton.onClick.AddListener(() => ShopButton("HugeHouse"));
        signalBus.Subscribe<BuyHouseSignal>(UpdateHouseButtons);
        StartImage (); 
        StartHouseButtons();
    }
    void StartImage ()
    {
        ResizeForCameraWidth (houseViewTransform);
        MoveSpriteNextToOther (houseViewTransform, roomViewTransform);

        ResizeForCameraWidth (streetViewTransform);
        MoveSpriteNextToOther (streetViewTransform, houseViewTransform);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void ResizeForCameraHeight (Transform resizeableTransform)
    {
        SpriteRenderer sr = resizeableTransform.GetComponent<SpriteRenderer>();
        if (sr == null)
            return;

        // Set filterMode
        sr.sprite.texture.filterMode = FilterMode.Point;

        // Get stuff
        double height = sr.sprite.bounds.size.y;
        double worldScreenHeight = mainCamera.orthographicSize * 2.0;
        // Resize
        resizeableTransform.localScale = new Vector2 (1, 1) * (float)(worldScreenHeight / height);
    }
    void ResizeForCameraWidth (Transform resizeableTransform)
    {
        SpriteRenderer sr = resizeableTransform.GetComponent<SpriteRenderer> ();
        if (sr == null)
            return;

        // Set filterMode
        sr.sprite.texture.filterMode = FilterMode.Point;

        // Get stuff
        double width = sr.sprite.bounds.size.x;
        double worldScreenHeight = mainCamera.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        // Resize
        resizeableTransform.localScale = new Vector2 (1, 1) * (float)(worldScreenWidth / width);
    }
    void MoveSpriteNextToOther(Transform transformToMove, Transform refTransform)
    {
        SpriteRenderer refSprite = refTransform.GetComponent<SpriteRenderer> ();
        SpriteRenderer spriteToMove = transformToMove.GetComponent<SpriteRenderer> ();

        transformToMove.position = new Vector3(refTransform.position.x, 0, refTransform.position.z);

        float refWidth = refSprite.sprite.bounds.size.x * refTransform.localScale.x;
        transformToMove.position += new Vector3 (refWidth / 2, 0, 0);

        float width = spriteToMove.sprite.bounds.size.x * transformToMove.localScale.x;
        transformToMove.position += new Vector3 (width / 2, 0, 0);
    }
    void SetButtonsPosition()
    {
        Vector2 viewportPosition = mainCamera.WorldToViewportPoint(basicHousePivot.position);
        viewportPosition *= canvas.GetComponent<RectTransform>().sizeDelta;
        basicHouseButton.GetComponent<RectTransform>().anchoredPosition = viewportPosition;

        viewportPosition = mainCamera.WorldToViewportPoint(niceApartmentPivot.position);
        viewportPosition *= canvas.GetComponent<RectTransform>().sizeDelta;
        niceApartmentButton.GetComponent<RectTransform>().anchoredPosition = viewportPosition;

        viewportPosition = mainCamera.WorldToViewportPoint(hugeHousePivot.position);
        viewportPosition *= canvas.GetComponent<RectTransform>().sizeDelta;
        hugeHouseButton.GetComponent<RectTransform>().anchoredPosition = viewportPosition;
    }
    void ShopButton(string houseName)
    {
        signalBus.Fire<OpenRealEstateShopSignal>(new OpenRealEstateShopSignal { houseName = houseName});
    }
    void StartHouseButtons()
    {
        foreach(RealEstateCustomizationItem item in playerInventory.OwnedRealEstateItems)
        {
            DeactivateButton(item.name);
        }
    }
    void UpdateHouseButtons(BuyHouseSignal signal)
    {
        DeactivateButton(signal.houseName);
    }

    void DeactivateButton(string name)
    {
        if (name == "ResidentialHouse")
        {
            niceApartmentButton.gameObject.SetActive(false);
            niceApartmentPivot.parent.gameObject.SetActive(false);
        }
        else if (name == "HugeHouse")
        {
            hugeHouseButton.gameObject.SetActive(false);
            hugeHousePivot.parent.gameObject.SetActive(false);
        }
    }
}
