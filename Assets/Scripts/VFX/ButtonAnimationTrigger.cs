using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

[RequireComponent(typeof(Animator))]
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
            anim.SetTrigger("Pressed");
            anim.ResetTrigger("Release");
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
            anim.SetTrigger("Release");
            anim.ResetTrigger("Pressed");
        }
        else
        {
            Debug.LogError ($"Animator not found on{name}, cannot start VFX");
        }
    }

    void Reset()
    {
        Debug.Log("A");
        if(anim = GetComponent<Animator>())
        {
            RuntimeAnimatorController controller = null;
            foreach (RuntimeAnimatorController asset in Resources.FindObjectsOfTypeAll(typeof(RuntimeAnimatorController)) as RuntimeAnimatorController[])
            {
                if (asset.name == "Buttons")
                {
                    controller = asset;
                    break;
                }    
            }
            Debug.Log(controller);
            anim.runtimeAnimatorController = controller;
            Debug.Log("Yes anim");
        }
        else
        {
            Debug.Log("No anim");
        }
    }
}
