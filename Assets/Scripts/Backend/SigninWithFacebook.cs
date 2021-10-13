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

    public SigninWithFacebook(SignalBus sb)
    {
        signalBus = sb;
        FB.Init();
    }

    public void SingInFacebook()
    {
        FB.LogInWithReadPermissions(null,(result =>
        {
            if (result.Cancelled)
            {
                signalBus.Fire<OnFacebookLoginFailedSignal>(new OnFacebookLoginFailedSignal()
                {
                    canceled = true
                });
                return;
            }else if ((result.AccessToken == null)||(result.Error!=String.Empty))
            {
                signalBus.Fire<OnFacebookLoginFailedSignal>();
            }
            else
            {
                signalBus.Fire<OnFacebookLoginSuccessSignal>(new OnFacebookLoginSuccessSignal()
                {
                    accessToken = result.AccessToken
                });
            }

        }));
    }
    

}
