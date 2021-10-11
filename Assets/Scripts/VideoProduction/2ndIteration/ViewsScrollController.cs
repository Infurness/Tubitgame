using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ViewsScrollController : MonoBehaviour,IEndDragHandler,IBeginDragHandler
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform roomViewPanelRect;
    [SerializeField] private RectTransform streetViewPanelRect;
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
        contentPanel.sizeDelta = new Vector2 (canvas.sizeDelta.x, contentPanel.sizeDelta.y);
    }
    public void OnBeginDrag (PointerEventData eventData)
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
    public void OnEndDrag (PointerEventData eventData)
    {
        float scrollDir = scrollRect.content.position.x - startContentPosX;
        if (scrollDir < 0)
            StartCoroutine (SmoothSnapTo (streetViewPanelRect));
        else if (scrollDir > 0)
            StartCoroutine (SmoothSnapTo (roomViewPanelRect));
    }

    public IEnumerator SmoothSnapTo (RectTransform target)
    {
        float lerpCounter = 0;
        Vector2 contentPanelNewPos = (Vector2)scrollRect.transform.InverseTransformPoint (contentPanel.position)
                                     - (Vector2)scrollRect.transform.InverseTransformPoint (target.position);
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
