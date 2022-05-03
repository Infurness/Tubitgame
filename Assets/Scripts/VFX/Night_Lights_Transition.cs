using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Night_Lights_Transition : MonoBehaviour
{
    [SerializeField] GameObject[] nightElements;
    [SerializeField] float timeToTurnOn;

    private void OnEnable()
    {
        DisableNightElements();
        Invoke(nameof(EnableNightElements), timeToTurnOn);
    }

    public void DisableNightElements()
    {
        foreach (var element in nightElements)
        {
            element.SetActive(false);
        }
    }

    public void EnableNightElements()
    {
        foreach (var element in nightElements)
        {
            element.SetActive(true);
        }
    }
}
