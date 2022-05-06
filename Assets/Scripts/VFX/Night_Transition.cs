using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Night_Transition : MonoBehaviour
{
    [SerializeField] private VFX_VC fX_VC;
    [SerializeField] private GameObject [] vfxElements;

    private bool isResting;
    

    private void OnEnable()
    {
        if (isResting)
        {
            EnableElements();
        }
    }

    public void ChangeIsResting(bool resting)
    {
        isResting = resting;
    }

    public void DisableElements()
    {
        foreach (var vfxObject in vfxElements)
        {
            vfxObject.SetActive(false);
        }
    }

    public void EnableElements()
    {    
       fX_VC.EnableLights(true);
    }
}
