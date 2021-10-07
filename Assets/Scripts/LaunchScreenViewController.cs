using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class LaunchScreenViewController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] private TMP_Text playFabIDText;
    void Start()
    {
        signalBus.Subscribe<OnLoginSuccessesSignal>((signal =>
        {
            playFabIDText.text += signal.playFabID;

            playFabIDText.gameObject.SetActive(true);
        }) );
    }

  
}
