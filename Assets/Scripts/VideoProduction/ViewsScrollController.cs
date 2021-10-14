using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ViewsScrollController : MonoBehaviour,IEndDragHandler,IBeginDragHandler
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform[] views;
    int currentViewIndex;
    private ScrollRect scrollRect;
    private float startContentPosX;
    [SerializeField] float movementSpeed;

    private Vector2 previousScreenSize;

    // Start is called before the first frame update
    void Start()
    { 
        scrollRect = GetComponent<ScrollRect> ();
        if (scrollRect == null)
            Debug.LogError ($"No ScrollRect set in {gameObject.name}");
        ResizeContentPanel ();
        previousScreenSize = new Vector2 (Screen.width, Screen.height);
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
        int newViewIndex = currentViewIndex;
        if (scrollDir < 0)
            newViewIndex += 1;
        else if (scrollDir > 0)
            newViewIndex -= 1;

        if(newViewIndex >= 0 && newViewIndex < views.Length)
        {
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
