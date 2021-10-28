using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ViewsScroll_VC : MonoBehaviour,IEndDragHandler,IBeginDragHandler
{
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

    // Start is called before the first frame update
    void Start()
    { 
        ResizeContentPanel ();
        previousScreenSize = new Vector2 (Screen.width, Screen.height);
        rightButton.onClick.AddListener (()=> { ScrollView (1); });
        leftButton.onClick.AddListener (() => { ScrollView (-1); });
        StartingState ();
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
    private void Update ()
    {
        if(previousScreenSize != new Vector2 (Screen.width, Screen.height))
        {
            ResizeContentPanel ();
        }
    }
    public void OnEndDrag (PointerEventData _eventData)
    {
        float scrollDir = scrollRect.content.position.x - startContentPosX;
        int movement = 0;
        if (scrollDir < 0)
            movement = 1;
        else if (scrollDir > 0)
            movement = -1;

        ScrollView (movement);
    }

    void ScrollView(int movement)
    {
        int newViewIndex = currentViewIndex + movement;
        if (newViewIndex >= 0 && newViewIndex < views.Length)
        {
            if (newViewIndex == 0)
            {
                rightButtonPanel.SetActive(true);
                leftButtonPanel.SetActive (false);
            }
            else if( newViewIndex == views.Length - 1)
            {
                rightButtonPanel.SetActive (false);
                leftButtonPanel.SetActive (true);
            }
            else
            {
                rightButtonPanel.SetActive (true);
                leftButtonPanel.SetActive (true);
            }
                

            currentViewIndex = newViewIndex;
            StartCoroutine (SmoothSnapTo (views[newViewIndex]));
        }
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
}
