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

    public void LoginWithDeviceID()
    {
       #if UNITY_EDITOR
        LoginWithCustomID();
      #elif UNITY_ANDROID 
        LoginWithAndroidDevice();
      #elif UNITY_IOS
        LoginWithIOSDevice
      #endif
    }

    void LoginWithAndroidDevice()
    {
        LoginWithAndroidDeviceIDRequest req = new LoginWithAndroidDeviceIDRequest();
        req.CreateAccount = true;
        req.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
        PlayFabClientAPI.LoginWithAndroidDeviceID(req,(result) =>
        {
            signalBus.Fire<OnLoginSuccessesSignal>(new OnLoginSuccessesSignal()
            {
                playFabID = result.PlayFabId
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
            signalBus.Fire<OnLoginSuccessesSignal>(new OnLoginSuccessesSignal()
            {
                playFabID = result.PlayFabId
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
            signalBus.Fire<OnLoginSuccessesSignal>(new OnLoginSuccessesSignal()
            {
                playFabID = result.PlayFabId
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

    public void GoogleLogin()
    {
    }

    public void AppleLogin()
    {
        
    }
}