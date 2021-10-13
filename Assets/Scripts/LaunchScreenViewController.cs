using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LaunchScreenViewController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] private TMP_Text playFabIDText;
    [Inject] private IAuthenticator authenticator;
    [SerializeField] private Button googleSignIn_Bt, appleSignin_Bt, facebookSignIn_Bt;
    [SerializeField] private GameObject signInButtonsPanel;

    
    void Start()
    {
#if !UNITY_IOS
        appleSignin_Bt.gameObject.SetActive(false);   
#endif
        if (PlayerPrefs.HasKey("PlayerLogedIn"))
        {
            signInButtonsPanel.gameObject.SetActive(false);
        }
        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>((signal =>
        {
            playFabIDText.text += signal.playerID;

            playFabIDText.gameObject.SetActive(true);
        }) );
    }

    public void OnGoogleLoginPressed()
    {
        authenticator.LoginWithGoogle();
    }
  
}
