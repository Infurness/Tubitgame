using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class SigninWithFacebook
{
    private SignalBus signalBus;
    private Action<string> AutoLoginCallback;
    public SigninWithFacebook(SignalBus sb)
    {
        signalBus = sb;
        if (!FB.IsInitialized)
        {
            FB.Init((() =>
            {
                if (FB.IsLoggedIn)
                {
                    AutoLoginCallback.Invoke(AccessToken.CurrentAccessToken.TokenString); 
                }
            }));
           

        }
      
    }

    public void SetAutoLoginCallBack(Action<string> autoLoginCallBack)
    {
        AutoLoginCallback = autoLoginCallBack;
    }

    public void SingInFacebook()
    {
        FB.LogInWithReadPermissions(null,(result =>
        {
            if (result.Cancelled)
            {
                signalBus.Fire<OnFacebookLoginFailedSignal>(new OnFacebookLoginFailedSignal()
                {
                    Canceled = true
                });
                signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
                {
                    Reason ="Canceled"
                });
                Debug.Log("Facebook Login Cancelled ");
            }else if ((result.AccessToken == null)||(result.Error!=null))
            {
                signalBus.Fire<OnFacebookLoginFailedSignal>();
                Debug.Log("Facebook Login failed "+result.Error );
                signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
                {
                    Reason = result.Error
                });

            }
            else
            {
                signalBus.Fire<OnFacebookLoginSuccessSignal>(new OnFacebookLoginSuccessSignal()
                {
                    AccessToken = result.AccessToken
                });
                Debug.Log("Facebook Logged in");
            }

        }));
    }
    

}
