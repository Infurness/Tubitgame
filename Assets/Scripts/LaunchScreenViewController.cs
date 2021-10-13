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
    [SerializeField] private GameObject signInCanvas;

    
    void Awake()
    {
#if !UNITY_IOS
        appleSignin_Bt.gameObject.SetActive(false);   
#endif
        if (PlayerPrefs.HasKey("LoginMethod"))
        {
            signInCanvas.gameObject.SetActive(false);
        }
        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>((signal =>
        {
            playFabIDText.text ="PlayFabID: "+ signal.playerID;

            playFabIDText.gameObject.SetActive(true);
            
        }) );
    }

    public void OnGoogleLoginPressed()
    {
        authenticator.LoginWithGoogle();
    }

    public void OnFacebookLoginPressed()
    {
        authenticator.LoginWithFaceBook();
    }

    public void OnSkipButtonPresses()
    {
        authenticator.LoginWithDeviceID();
    }

    public void OnAppleLoginPressed()
    {
        authenticator.LoginWithAppleID();
    }
}
