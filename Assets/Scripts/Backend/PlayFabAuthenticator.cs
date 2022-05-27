using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;
using Zenject;
using LoginResult = Facebook.Unity.LoginResult;

public class PlayFabAuthenticator : IAuthenticator
{
	[Inject] SignalBus signalBus;
	private SigninWithGoogle swg;
	private SigninWithFacebook swfb;
	private SigninWithAppleID _signinWithAppleID;
   private PlayFabAuthenticationContext playFabAuthenticationContext;

   public PlayFabAuthenticator(SignalBus sb)
   {
	   signalBus = sb;
	   swfb = new SigninWithFacebook(signalBus);
#if UNITY_IOS
	   _signinWithAppleID = new SigninWithAppleID(signalBus);
	   signalBus.Subscribe<OnAppleLoginSuccessSignal>((signal => { OnAppleLoginSuccess(signal); }));
#endif
	  swfb.SetAutoLoginCallBack(PlayFabFacebookLogin);
	  signalBus.Subscribe<OnFacebookLoginSuccessSignal>((signal => { PlayFabFacebookLogin(signal.AccessToken.TokenString); }));
   }
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
		PlayFabClientAPI.LoginWithAndroidDeviceID(req, OnLoginWithDeviceIDSuccess, OnLoginWithDeviceIDFailed);
	}

   
	void LoginWithIOSDevice()
	{
		
		LoginWithIOSDeviceIDRequest req = new LoginWithIOSDeviceIDRequest();
		req.CreateAccount = true;
		req.DeviceId = SystemInfo.deviceUniqueIdentifier;
		PlayFabClientAPI.LoginWithIOSDeviceID(req, OnLoginWithDeviceIDSuccess, OnLoginWithDeviceIDFailed);
	}

	void LoginWithCustomID()
	{
		LoginWithCustomIDRequest req = new LoginWithCustomIDRequest();
		req.CreateAccount = true;
		req.CustomId = SystemInfo.deviceUniqueIdentifier;

		PlayFabClientAPI.LoginWithCustomID(req, OnLoginWithDeviceIDSuccess, OnLoginWithDeviceIDFailed);
	}
	
	void OnLoginWithDeviceIDSuccess(PlayFab.ClientModels.LoginResult result)
	{
		playFabAuthenticationContext = result.AuthenticationContext;

		PlayerPrefs.SetString("LoginMethod","DeviceID");
		signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
		{
			PlayerID = result.PlayFabId,
			NewPlayer = result.NewlyCreated,
			
			AuthenticationContext = result.AuthenticationContext
		});
	}

	void OnLoginWithDeviceIDFailed(PlayFabError error)
	{
		signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
		{
			Reason = error.ErrorMessage
		});
		Debug.Log("Login Failed "+error.ErrorMessage);
	 
	}
	public async void LinkToGoogleAccount()
	{
		if (swg==null)
		{
			swg = new SigninWithGoogle();
		}

		signalBus.Subscribe<OnGoogleSignInSuccessSignal>((signal =>
		{
			LinkGoogleAccountRequest req = new LinkGoogleAccountRequest();
			req.ServerAuthCode = signal.AuthCode;
			req.AuthenticationContext = playFabAuthenticationContext;
		 
			PlayFabClientAPI.LinkGoogleAccount(req, (result =>
			{
				
			   
			}), (error =>
			{
				signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
				{
					Reason = error.ErrorMessage
				});
				Debug.Log("Login Failed " + error.ErrorMessage);

			}));
		}));
	  	swg.SigninWithGoogleID(signalBus);
		
		 
	}

	public async void LoginWithGoogle()
	{		
		if (swg==null)
		{
			swg = new SigninWithGoogle();
			signalBus.Subscribe<OnGoogleSignInSuccessSignal>((signal =>
			{
				LoginWithGoogleAccountRequest req = new LoginWithGoogleAccountRequest();
				req.ServerAuthCode = signal.AuthCode;
				req.CreateAccount = true;

				LoginWithGoogleAccountRequest(req);
			}));
		}
	  
		swg.SigninWithGoogleID(signalBus);
	}

    private void LoginWithGoogleAccountRequest(LoginWithGoogleAccountRequest req)
    {
        PlayFabClientAPI.LoginWithGoogleAccount(req, (result =>
        {
            signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
            {
                PlayerID = result.PlayFabId,
                NewPlayer = result.NewlyCreated,
                AuthenticationContext = result.AuthenticationContext
            });
            Debug.Log("PlayFabLogin with Google");
            PlayerPrefs.SetString("LoginMethod", "GoogleSignIn");

        }), (error =>
        {
            signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
            {
                Reason = error.ErrorMessage
            });
            Debug.Log("Login Failed " + error.ErrorMessage);

        }));
    }

    public void LoginWithFaceBook()
	{
		swfb.SingInFacebook();
	}

	private void PlayFabFacebookLogin(string accessToken)
	{
		OnFacebookLoginSuccessSignal signal;
		LoginWithFacebookRequest request = new LoginWithFacebookRequest();
		request.AccessToken =accessToken;
		request.CreateAccount = true;

		PlayFabClientAPI.LoginWithFacebook(request, (result =>
		{
			playFabAuthenticationContext = result.AuthenticationContext;
			signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
			{
				PlayerID = result.PlayFabId,
				NewPlayer = result.NewlyCreated,
				AuthenticationContext = result.AuthenticationContext
			});
			PlayerPrefs.SetString("LoginMethod", "Facebook");
		}), (error => { Debug.Log("PlayFab Facebook login failed " + error.ErrorMessage); }));
		
		
	}

	public void LoginWithAppleID()
	{
		if (PlayerPrefs.HasKey("AppleIDToken"))
		{
			OnAppleLoginSuccess(new OnAppleLoginSuccessSignal()
			{
				IdToken = PlayerPrefs.GetString("AppleIDToken")
			});
		}
		else
		{
			_signinWithAppleID.SigninWithApple();

		}
		
	}

	private void OnAppleLoginSuccess(OnAppleLoginSuccessSignal signal)
	{
		
		LoginWithAppleRequest req = new LoginWithAppleRequest();
		req.CreateAccount = true;
		req.IdentityToken = signal.IdToken;
		PlayFabClientAPI.LoginWithApple(req, (result =>
		{
			signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
			{
				PlayerID = result.PlayFabId,
				NewPlayer = result.NewlyCreated,
				AuthenticationContext = result.AuthenticationContext
			});
			PlayerPrefs.SetString("LoginMethod", "AppleID");
			PlayerPrefs.SetString("AppleIDToken",signal.IdToken);
			Debug.Log("Playfab Apple login success");
		}), (error =>
		{
			signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
			{
				Reason = error.ErrorMessage
			});
			PlayerPrefs.DeleteKey("AppleIDToken");
			Debug.Log("Failed to Login with apple id Playfab " + error.ErrorMessage);
		}));
	}

	public void LinkToAppleID()
	{
		
	}

}
