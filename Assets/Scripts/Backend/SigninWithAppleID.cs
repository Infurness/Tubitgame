using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SignInWithApple;
using Zenject;

public class SigninWithAppleID
{
    private SignalBus _signalBus;
    private UnityEngine.SignInWithApple.SignInWithApple _signInWithApple;
    private GameObject _swa;
    public SigninWithAppleID(SignalBus sb)
    {
        _signalBus = sb;
        _swa = new GameObject();
        _signInWithApple = _swa.AddComponent<SignInWithApple>();
    }

    public void SigninWithApple()
    {
        Debug.Log("Start Apple SingIn ....");

        _signInWithApple.onLogin+=((arg0) =>
        {
            _signalBus.Fire<OnAppleLoginSuccessSignal>(new OnAppleLoginSuccessSignal()
            {
                IdToken = arg0.userInfo.idToken,
                Email = arg0.userInfo.email,
                Name = arg0.userInfo.displayName
            });
            
        });
        _signInWithApple.onError+=((arg0 =>
        {
            _signalBus.Fire<OnAppleLoginFailedSignal>(new OnAppleLoginFailedSignal()
            {
                Reason = arg0.error
            });
            _signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
            {
                Reason = arg0.error
            });
            Debug.Log("Failed to login with apple id");
            
        }));
           _signInWithApple.Login();
    }

    ~SigninWithAppleID()
    {
        GameObject.Destroy(_swa);
    }
    
    
      
}
