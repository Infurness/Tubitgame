using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class LaunchScreenViewController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] private TMP_Text playFabIDText;
    [Inject] private IAuthenticator authenticator;

    void Start()
    {
        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>((signal =>
        {
            playFabIDText.text += signal.playerID;

            playFabIDText.gameObject.SetActive(true);
        }) );
    }

    public void OnGoogleLoginPressed()
    {
        authenticator.LinkToGoogleAccount();
    }
  
}
