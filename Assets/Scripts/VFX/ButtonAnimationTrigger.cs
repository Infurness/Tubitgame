using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimationTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator> ();
    }

    public void OnPointerDown (PointerEventData eventData)
    {
        if(anim)
        {
            anim.Play ("Button_Pressed");
        }
        else
        {
            Debug.LogError ($"Animator not found on{name}, cannot start VFX");
        }
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        if (anim)
        {
            anim.Play ("Button_Release");
        }
        else
        {
            Debug.LogError ($"Animator not found on{name}, cannot start VFX");
        }
    }
}
