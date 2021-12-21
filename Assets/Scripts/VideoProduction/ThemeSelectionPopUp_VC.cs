using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

struct UsedSlotInfo
{
    public ThemeType themeType;
    public GameObject button;
}

public class ThemeSelectionPopUp_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] private ThemesManager themesManager;

    [SerializeField] private ScrollRect scrollList;
    [SerializeField] private GameObject themeButtonsHolder;
    private List<GameObject> themeButtonsSpawned;
    [SerializeField] private GameObject themeButtonPrefab;
    [SerializeField] private GameObject[] draggableThemeObjects;
    int draggableBeingUsed;
    ThemeType themeTypeBeingDragged;
    GameObject buttonBeingDragged;
    private Dictionary<int, int> dragablesBeingUsedForSlots = new Dictionary<int, int>(); //Dictionary<"draggable index", "slot index">
    private Dictionary<int, UsedSlotInfo> usedSlotsInfo = new Dictionary<int, UsedSlotInfo> (); 
    [SerializeField] private GameObject[] themeSlots;
    [SerializeField] Sprite emptySlotImage;
    [SerializeField] Sprite usedSlotImage;
    private bool draggingTheme;

    [SerializeField] private Button confirmButton;

    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<ThemeHeldSignal> (StartDraggingTheme);
        signalBus.Subscribe<EndPublishVideoSignal> (InitialState);
        signalBus.Subscribe<CancelVideoRecordingSignal> (InitialState);
        signalBus.Subscribe<OpenVideoCreationSignal>(InitialState);

        confirmButton.onClick.AddListener (ConfirmThemes);
        mainCam = Camera.main;
        SetUpThemeButtons ();
        InitialState ();
    }

    void InitialState ()
    {

        dragablesBeingUsedForSlots.Clear ();
        foreach (UsedSlotInfo slotInfo in usedSlotsInfo.Values)
            slotInfo.button.GetComponent<Button> ().interactable = true;
        usedSlotsInfo.Clear ();
        foreach (GameObject draggable in draggableThemeObjects)
            draggable.SetActive (false);
        Debug.Log ("PopupCleared");
        foreach(GameObject slot in themeSlots)
        {
            slot.GetComponent<Image> ().sprite = emptySlotImage;
        }
    }
    void SetUpThemeButtons ()
    {
        foreach (ThemeType themeType in themesManager.GetThemes ())
        {
            CreateThemeButton (themeType);
        }
        int i = 0;
        foreach (GameObject draggable in draggableThemeObjects)
        {
            draggable.GetComponent<DraggableThemeElement> ().SetVisualController (this,i);
            i++;
        }
    }
    void CreateThemeButton (ThemeType _themeType)
    {
        GameObject button = Instantiate (themeButtonPrefab, themeButtonsHolder.transform);
        button.GetComponent<ButtonThemePreProductionView> ().themeType = _themeType;
        button.GetComponent<ButtonThemePreProductionView> ().SetSignaBus (signalBus);
        //button.GetComponent<Button> ().OnPointerDown().AddListener (() => OnThemeSelected (_themeType, button.GetComponentInChildren<TMP_Text>().text));
    }
    void StartDraggingTheme (ThemeHeldSignal signal)
    {
        for (int i = 0; i < draggableThemeObjects.Length; i++)
        {
            if (!dragablesBeingUsedForSlots.ContainsKey (i))
            {
                draggableBeingUsed = i;
                break;
            }
        } 
        scrollList.enabled = false;
        draggingTheme = true;
        draggableThemeObjects[draggableBeingUsed].SetActive (true);
        draggableThemeObjects[draggableBeingUsed].transform.position = signal.themeBox.transform.position;
        themeTypeBeingDragged = signal.themeType;
        buttonBeingDragged = signal.themeBox;
        draggableThemeObjects[draggableBeingUsed].GetComponentInChildren<TMP_Text> ().text = signal.buttonText;
        StartCoroutine (DragTheme());
    }
    public void StartDraggingTheme (int draggableIndex)
    {
        draggableBeingUsed = draggableIndex;
        int slotIndex = 0;
        foreach (GameObject slot in themeSlots)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint (slot.GetComponent<RectTransform> (), mainCam.ScreenToWorldPoint (Input.mousePosition)))
            {
                break;
            }
            slotIndex++;
        }
        scrollList.enabled = false;
        draggingTheme = true;
        draggableThemeObjects[draggableBeingUsed].SetActive (true);
        themeTypeBeingDragged = usedSlotsInfo[slotIndex].themeType;
        buttonBeingDragged = usedSlotsInfo[slotIndex].button;
        EmptySlot (slotIndex);
        StartCoroutine (DragTheme ());
    }
    void StopDraggingTheme ()
    {
        draggingTheme = false;
        scrollList.enabled = true;
        int slotIndex = 0;
        draggableThemeObjects[draggableBeingUsed].GetComponent<Image> ().enabled = false;
        foreach (GameObject slot in themeSlots)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint (slot.GetComponent<RectTransform> (), mainCam.ScreenToWorldPoint(Input.mousePosition)))
            {
                StartCoroutine( MoveObjectTo (draggableThemeObjects[draggableBeingUsed], slot.transform.position));
                if (dragablesBeingUsedForSlots.ContainsValue (slotIndex)) //slotBeingUse
                {
                    StopUsingSlot (slotIndex);
                }
                dragablesBeingUsedForSlots.Add (draggableBeingUsed, slotIndex);
                UsedSlotInfo slotInfo = new UsedSlotInfo () { themeType = themeTypeBeingDragged, button = buttonBeingDragged};
                usedSlotsInfo.Add (slotIndex, slotInfo);
                slot.GetComponent<Image> ().sprite = usedSlotImage;
                slot.transform.GetChild (0).gameObject.SetActive (false);
                return;
            }
            slotIndex++;
        }
        draggableThemeObjects[draggableBeingUsed].SetActive (false);
        buttonBeingDragged.GetComponent<Button> ().interactable = true;
    }
    void StopUsingSlot (int slotIndex)
    {
        int myKey = dragablesBeingUsedForSlots.FirstOrDefault (x => x.Value == slotIndex).Key;
        draggableThemeObjects[myKey].SetActive (false);
        dragablesBeingUsedForSlots.Remove (myKey);
        usedSlotsInfo[slotIndex].button.GetComponent<Button> ().interactable = true;
        usedSlotsInfo.Remove (slotIndex);
        themeSlots[slotIndex].GetComponent<Image> ().sprite = emptySlotImage;
        themeSlots[slotIndex].transform.GetChild (0).gameObject.SetActive (true);
    }
    void EmptySlot (int slotIndex)
    {
        dragablesBeingUsedForSlots.Remove (draggableBeingUsed);
        usedSlotsInfo.Remove (slotIndex);
        themeSlots[slotIndex].GetComponent<Image> ().sprite = emptySlotImage;
        themeSlots[slotIndex].transform.GetChild (0).gameObject.SetActive (true);
    }
    IEnumerator MoveObjectTo (GameObject objectToMove, Vector3 objective)
    {
        float lerpCounter = 0;

        while (lerpCounter < 1)
        {
            lerpCounter += Time.deltaTime * 3;
            objectToMove.transform.position = Vector3.Lerp (objectToMove.transform.position, objective, lerpCounter);
            yield return null;
        }
    }
        
    IEnumerator DragTheme ()
    {
        Vector3 offset = mainCam.ScreenToWorldPoint (Input.mousePosition) - draggableThemeObjects[draggableBeingUsed].transform.position;
        while (draggingTheme)
        {
            Vector3 themeNewPos = mainCam.ScreenToWorldPoint (Input.mousePosition) - offset;
            themeNewPos.z = 100;
            draggableThemeObjects[draggableBeingUsed].GetComponent<RectTransform>().position = themeNewPos;
            draggableThemeObjects[draggableBeingUsed].GetComponent<Image> ().enabled = true;
            if (!Input.anyKey)//Not being hold anymore
            {
                StopDraggingTheme ();
            }
            yield return null;
        }    
    }

    void ConfirmThemes ()
    {
        Dictionary<int, ThemeType> selectedThemesSlots = new Dictionary<int, ThemeType> ();

        foreach(KeyValuePair<int,UsedSlotInfo> info in usedSlotsInfo)
        {
            selectedThemesSlots.Add (info.Key, info.Value.themeType);
        }
        signalBus.Fire<ConfirmThemesSignal> (new ConfirmThemesSignal () { selectedThemesSlots = selectedThemesSlots });
    }
}
