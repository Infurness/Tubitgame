using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;
using Zenject;

public class PlayFabAuthenticator : IAuthenticator
{
    [Inject] SignalBus signalBus;
   private SigninWithGoogle swg;
   
   private PlayFabAuthenticationContext playFabAuthenticationContext;
    public void LoginWithDeviceID()
    {
       #if UNITY_EDITOR
        LoginWithCustomID();
      #elif UNITY_ANDROID 
        LoginWithAndroidDevice();
      #elif UNITY_IOS
        LoginWithIOSDevice();
      #endif
    }

    public void LinkToFaceBook()
    {
        
    }

    void LoginWithAndroidDevice()
    {
        LoginWithAndroidDeviceIDRequest req = new LoginWithAndroidDeviceIDRequest();
        req.CreateAccount = true;
        req.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
        PlayFabClientAPI.LoginWithAndroidDeviceID(req,(result) =>
        {
            playFabAuthenticationContext = result.AuthenticationContext;

            signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
            {
                playerID = result.PlayFabId,
                authenticationContext = result.AuthenticationContext
            });
        }, (error) =>
        {
            signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
            {
                reason = error.ErrorMessage
            });
            Debug.Log("Login Failed "+error.ErrorMessage);
        } );
    }

    void LoginWithIOSDevice()
    {
        
        LoginWithIOSDeviceIDRequest req = new LoginWithIOSDeviceIDRequest();
        req.CreateAccount = true;
        req.DeviceId = SystemInfo.deviceUniqueIdentifier;
        PlayFabClientAPI.LoginWithIOSDeviceID(req,(result) =>
        {
            playFabAuthenticationContext = result.AuthenticationContext;
            signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
            {
                playerID = result.PlayFabId,
                authenticationContext = result.AuthenticationContext
            });
        }, (error) =>
        {
            signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
            {
                reason = error.ErrorMessage
            });
            Debug.Log("Login Failed "+error.ErrorMessage);
        } );
    }

    void LoginWithCustomID()
    {
        LoginWithCustomIDRequest req = new LoginWithCustomIDRequest();
        req.CreateAccount = true;
        req.CustomId = SystemInfo.deviceUniqueIdentifier;
        
        PlayFabClientAPI.LoginWithCustomID(req,(result) =>
        {
            playFabAuthenticationContext = result.AuthenticationContext;

            signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
            {
                playerID = result.PlayFabId,
                authenticationContext = result.AuthenticationContext                
            });
        }, (error) =>
        {
            signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
            {
                reason = error.ErrorMessage
            });
            Debug.Log("Login Failed "+error.ErrorMessage);
        } );
    }
    
    public void FacebookLogin()
    {
        
    }

    public async void LinkToGoogleAccount()
    {
        if (swg==null)
        {
                    swg = new SigninWithGoogle(signalBus);

        }
        signalBus.Subscribe<OnGoogleSignInSuccessSignal>((signal =>
        {
            LinkGoogleAccountRequest req = new LinkGoogleAccountRequest();
            req.ServerAuthCode = signal.authCode;
            req.AuthenticationContext = playFabAuthenticationContext;
         
            PlayFabClientAPI.LinkGoogleAccount(req, (result =>
            {
                
               
            }), (error =>
            {
                signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
                {
                    reason = error.ErrorMessage
                });
                Debug.Log("Login Failed " + error.ErrorMessage);

            }));
        }));
      await  swg.SingInWithGoogleID();
        
         
    }


    public async void LoginWithGoogle()
    {
        if (swg==null)
        {
            swg = new SigninWithGoogle(signalBus);

        }
        signalBus.Subscribe<OnGoogleSignInSuccessSignal>((signal =>
        {
            LoginWithGoogleAccountRequest req = new LoginWithGoogleAccountRequest();
            req.ServerAuthCode = signal.authCode;
         
            PlayFabClientAPI.LoginWithGoogleAccount(req, (result =>
            {
                
               
            }), (error =>
            {
                signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
                {
                    reason = error.ErrorMessage
                });
                Debug.Log("Login Failed " + error.ErrorMessage);

            }));
        }));
        await  swg.SingInWithGoogleID();

    }

    public void LoginWithFaceBook()
    {
        
    }

    public void LoginWithAppleID()
    {
        
    }

    public void LinkToAppleID()
    {
        
    }

}
