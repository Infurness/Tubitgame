using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Night_Transition : MonoBehaviour
{
    [SerializeField] private VFX_VC fX_VC;
    [SerializeField] private GameObject [] vfxElements;

    private void OnEnable()
    {
        EnableElements();
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
