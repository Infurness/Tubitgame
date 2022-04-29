using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

public class ViewsScroll_VC : MonoBehaviour,IEndDragHandler,IBeginDragHandler
{
    [Inject] private SignalBus signalBus;
    [Inject] private GlobalAudioManager audioManager;
    [Inject] private SoundsHolder soundsHolder;

    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform[] views;
    [SerializeField] private GameObject rightButtonPanel;
    [SerializeField] private GameObject leftButtonPanel;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    int currentViewIndex;   
    private float startContentPosX;
    [SerializeField] float movementSpeed;

    private Vector2 previousScreenSize;

    [SerializeField] private Transform viewsHolder;
    [SerializeField] private Transform houseView;
    [SerializeField] private Transform streetView;

    [Inject] private GameAnalyticsManager gameAnalyticsManager;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenHomePanelSignal> (ForceHomePanel);
        ResizeContentPanel ();
        previousScreenSize = new Vector2 (Screen.width, Screen.height);
        rightButton.onClick.AddListener (()=> { ScrollView (1); });
        leftButton.onClick.AddListener (() => { ScrollView (-1); });
        StartingState ();
        
        signalBus.Subscribe<RoomCustomizationVisibilityChanged>(((signal) =>
        {
            enabled = !signal.Visibility;
            SetButtonVisibility(!signal.Visibility);
            if (signal.Visibility)
            {
                ForceHomePanel();
            }
            
        }));
        
        signalBus.Subscribe<CharacterCustomizationVisibilityChanged>((signal) =>
        {
            if (signal.Visibility)
            {
                ForceHomePanel();
            }
        });
        
        
    }

    void StartingState ()
    {
        currentViewIndex = 0;
        leftButtonPanel.SetActive (false);
    }

    void ResizeContentPanel ()
    {
        contentPanel.sizeDelta = new Vector2 (canvas.sizeDelta.x * (views.Length-1), contentPanel.sizeDelta.y);
    }
    public void OnBeginDrag (PointerEventData _eventData)
    {
        startContentPosX = scrollRect.content.position.x;
        StopAllCoroutines ();
    }
    // private void Update ()
    // {
    //     if(previousScreenSize != new Vector2 (Screen.width, Screen.height))
    //     {
    //         ResizeContentPanel ();
    //     }
    // }
    public void OnEndDrag (PointerEventData _eventData)
    {
        float scrollDir = scrollRect.content.position.x - startContentPosX;
        int movement = 0;
        if (scrollDir < 0)
            movement = 1;
        else if (scrollDir > 0)
            movement = -1;
        if (TutorialManager.Instance != null)
            return;
        ScrollView (movement);
    }

    void ScrollView(int movement)
    {
        int newViewIndex = currentViewIndex + movement;
        if (newViewIndex >= 0 && newViewIndex < 3)
        {
            if (newViewIndex == 0)
            {
                rightButtonPanel.SetActive(true);
                leftButtonPanel.SetActive (false);
                audioManager.TerminateSound(soundsHolder.streetSounds);
            }
            else if( newViewIndex == 3 - 1)
            {
                rightButtonPanel.SetActive (false);
                leftButtonPanel.SetActive (true);
                audioManager.PlaySound(soundsHolder.streetSounds, AudioType.Effect, true, true);
                 gameAnalyticsManager.SendCustomEvent("MainScreen");

            }
            else
            {
                rightButtonPanel.SetActive (true);
                leftButtonPanel.SetActive (true);
                audioManager.PlaySound(soundsHolder.streetSounds, AudioType.Effect, true, true);
            }
                

            currentViewIndex = newViewIndex;
            //StartCoroutine (SmoothSnapTo (views[newViewIndex]));
        }
        //Dummy
        if (currentViewIndex == 2)
        {
            gameAnalyticsManager.SendCustomEvent("NeighbourHoodViewScreen");

            float movementLength = viewsHolder.position.x - streetView.position.x;
            signalBus.Fire(new SetCarsCanvasButtonVisibility()
            {
                visibility = false
            });
            StartCoroutine (SmoothSnapTo (new Vector3 (movementLength, 0, 0)));
            signalBus.Fire<VFX_ActivateNightSignal>(new VFX_ActivateNightSignal { activate = false });
        }
        else if (currentViewIndex == 1)
        {
            gameAnalyticsManager.SendCustomEvent("StreetViewScreen");

            signalBus.Fire(new SetCarsCanvasButtonVisibility()
            {
                visibility = true
            });
            float movementLength = viewsHolder.position.x - houseView.position.x;
            StartCoroutine (SmoothSnapTo (new Vector3 (movementLength, 0, 0)));
            signalBus.Fire<VFX_ActivateNightSignal>(new VFX_ActivateNightSignal { activate = false });
        }
        else
        {
            signalBus.Fire(new SetCarsCanvasButtonVisibility()
            {
                visibility = false
            });
            StartCoroutine (SmoothSnapTo (Vector3.zero));
            signalBus.Fire<VFX_ActivateNightSignal>(new VFX_ActivateNightSignal { activate = true });
        }
    }
    void ForceHomePanel ()
    {
        int movement = -currentViewIndex;
        ScrollView (movement);
    }
    public IEnumerator SmoothSnapTo (RectTransform _target)
    {
        float lerpCounter = 0;
        Vector2 contentPanelNewPos = (Vector2)scrollRect.transform.InverseTransformPoint (contentPanel.position)
                                     - (Vector2)scrollRect.transform.InverseTransformPoint (_target.position);
        contentPanelNewPos.x += canvas.sizeDelta.x/2;
        contentPanelNewPos.y = 0;

        while (lerpCounter<1)
        {
            lerpCounter += Time.deltaTime* movementSpeed;
            contentPanel.anchoredPosition = Vector2.Lerp (contentPanel.anchoredPosition , contentPanelNewPos, lerpCounter);
            yield return null;
        } 
    }

    public IEnumerator SmoothSnapTo (Vector3 newPos)
    {
        float lerpCounter = 0;
        while (lerpCounter < 1)
        {
            lerpCounter += Time.deltaTime * movementSpeed;
            viewsHolder.position = Vector3.Lerp (viewsHolder.position, newPos, lerpCounter);
            signalBus.Fire<SnapToNeighborhoodViewSignal>();
            yield return null;
        }
    }

    public void SetButtonVisibility(bool state)
    {
        rightButton.gameObject.SetActive(state);
        leftButton.gameObject.SetActive(state);
    }
}
