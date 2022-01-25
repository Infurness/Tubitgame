using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LaunchScreenViewController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] private TMP_Text playFabIDText,stateText;
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
            playFabIDText.text ="PlayFabID: "+ signal.PlayerID;

            playFabIDText.gameObject.SetActive(true);
            
        }) );
        Application.targetFrameRate = 300;
        
    }

    private void Start()
    {
        GameAnalytics.StartTimer("LaunchScreenTime");
        GameAnalytics.NewDesignEvent("LaunchScreen");
        signalBus.Subscribe<OnLoginFailedSignal>(signal =>
        {
           signInCanvas.gameObject.SetActive(true);
        });
        
        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>((signal) =>
        {
            SetStateText("Check Remote Assets Updates" );
        });
        signalBus.Subscribe<OnLoginFailedSignal>((signal)=>
        {
            SetStateText(signal.Reason);
        });
        signalBus.Subscribe<RemoteAssetsCheckSignal>((signal =>
        {
            SetStateText("Get Player Data");
        } ));
        
        
    }

    private void OnDestroy()
    {
        var loadTime=  GameAnalytics.StopTimer("LaunchScreenTime");
        GameAnalytics.NewDesignEvent("LaunchScreenTime", loadTime);
    }

    public void OnGoogleLoginPressed()
    {
        authenticator.LoginWithGoogle();
        signInCanvas.gameObject.SetActive(false);

    }

    public void OnFacebookLoginPressed()
    {
        authenticator.LoginWithFaceBook();
        signInCanvas.gameObject.SetActive(false);

    }

    public void OnSkipButtonPresses()
    {
        authenticator.LoginWithDeviceID();
        signInCanvas.gameObject.SetActive(false);

    }

    public void OnAppleLoginPressed()
    {
        authenticator.LoginWithAppleID();
        signInCanvas.gameObject.SetActive(false);

    }

    void SetStateText(string text)
    {
        stateText.text = text+".......";
    }
}
