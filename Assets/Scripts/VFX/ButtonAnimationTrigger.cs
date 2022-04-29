using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using Zenject;

[RequireComponent(typeof(Animator))]
public class ButtonAnimationTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator> ();
    }
    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        anim.Play("Idle");
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
            anim.runtimeAnimatorController = controller;
        }
    }
}
