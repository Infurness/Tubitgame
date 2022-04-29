using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameAuthenticator : MonoBehaviour
{

    [Inject] private IAuthenticator authenticator;
    
    

    void Start()
    {
        if (PlayerPrefs.HasKey("LoginMethod"))
        {
            var loginMethod = PlayerPrefs.GetString("LoginMethod");
            switch (loginMethod)
            {
                case "DeviceID": authenticator.LoginWithDeviceID(); break;
                
                case  "AppleID": authenticator.LoginWithAppleID(); break;
                
                case  "GoogleSignIn": authenticator.LoginWithGoogle(); break;
            }
        }
        
    }

    public void LoginWithDeviceID()
    {
     authenticator.LoginWithDeviceID();   
    }

     public void LoginWithGoogle()
    {
        authenticator.LoginWithGoogle();
    }

    public void LoginWithFacebook()
    {
        authenticator.LoginWithFaceBook();
    }

    public void LoginWithAppleID()
    {
        authenticator.LoginWithAppleID();
    }
}
