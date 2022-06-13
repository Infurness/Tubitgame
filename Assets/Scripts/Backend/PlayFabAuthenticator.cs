using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;
using Zenject;

public class PlayFabAuthenticator : IAuthenticator
{
    [Inject]
    SignalBus signalBus;

    SigninWithGoogle swg;
    SigninWithFacebook swfb;
    SigninWithAppleID _signinWithAppleID;
    PlayFabAuthenticationContext playFabAuthenticationContext;

    public PlayFabAuthenticator(SignalBus sb)
    {
        signalBus = sb;
        swfb = new SigninWithFacebook(signalBus);
#if UNITY_IOS
        _signinWithAppleID = new SigninWithAppleID(signalBus);
        signalBus.Subscribe<OnAppleLoginSuccessSignal>(signal => { OnAppleLoginSuccess(signal); });
#endif
        swfb.SetAutoLoginCallBack(PlayFabFacebookLogin);
        signalBus.Subscribe<OnFacebookLoginSuccessSignal>(signal =>
        {
            PlayFabFacebookLogin(signal.AccessToken.TokenString);
        });
    }

    public void LoginWithDeviceID()
    {
		LoginWithCustomID();
    }

    public void LinkToFaceBook()
    {
        var req = new LoginWithPlayFabRequest();
    }
    void LoginWithCustomID()
    {
        var req = new LoginWithCustomIDRequest();
        req.CreateAccount = true;
        req.CustomId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Login With google Custom ID");
        PlayFabClientAPI.LoginWithCustomID(req, (result =>
        {
            signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
            {
                PlayerID = result.PlayFabId,
                NewPlayer = result.NewlyCreated,
                AuthenticationContext = result.AuthenticationContext
            });
        } ), OnLoginWithDeviceIDFailed);
    }

    void OnLoginWithDeviceIDSuccess(LoginResult result)
    {
        playFabAuthenticationContext = result.AuthenticationContext;

        PlayerPrefs.SetString("LoginMethod", "DeviceID");
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
        Debug.Log("Login Failed " + error.ErrorMessage);
    }

    public async void LinkToGoogleAccount()
    {
        if (swg == null) swg = new SigninWithGoogle();

        signalBus.Subscribe<OnGoogleSignInSuccessSignal>(signal =>
        {
            var req = new LinkGoogleAccountRequest();
            req.ServerAuthCode = signal.AuthCode;
            req.AuthenticationContext = playFabAuthenticationContext;
            Debug.Log("Server Auth code" + signal.AuthCode);
            PlayFabClientAPI.LinkGoogleAccount(req, result => { }, error =>
            {
                signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
                {
                    Reason = error.ErrorMessage
                });
                Debug.Log("Login Failed " + error.ErrorMessage);
            });
        });
        swg.SigninWithGoogleID(signalBus);
    }

    public async void LoginWithGoogle()
    {
     

        if (swg == null)
        {
            swg = new SigninWithGoogle();
            signalBus.Subscribe<OnGoogleSignInSuccessSignal>(signal =>
            {
                Debug.Log("Server Auth code" + signal.AuthCode);
                if (PlayerPrefs.HasKey("GoogleCustomId"))
                {
                    GoogleCustomLogin(signal.EmailUserName);
                    return;
                }
                var req = new LoginWithGoogleAccountRequest();
                req.ServerAuthCode = signal.AuthCode;
                req.CreateAccount = true;
                LoginWithGoogleAccountRequest(req,signal.EmailUserName);
            });
        }

        swg.SigninWithGoogleID(signalBus);
    }

    void LoginWithGoogleAccountRequest(LoginWithGoogleAccountRequest req,string emailUserName)
    {
        PlayFabClientAPI.LoginWithGoogleAccount(req, result =>
        {
            Debug.Log("PlayFabLogin with Google");
            PlayerPrefs.SetString("LoginMethod", "GoogleSignIn");
            PlayerPrefs.SetString("GoogleCustomId", emailUserName);
            var linkReq = new LinkCustomIDRequest();
            linkReq.CustomId = emailUserName;
            linkReq.ForceLink = false;

            PlayFabClientAPI.LinkCustomID(linkReq, (linkResult) =>
            {
                signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
                {
                    PlayerID = result.PlayFabId,
                    NewPlayer = result.NewlyCreated,
                    AuthenticationContext = result.AuthenticationContext
                });
            }, (error) =>
            {
                if (error.Error==PlayFabErrorCode.LinkedIdentifierAlreadyClaimed)
                {
                    GoogleCustomLogin(emailUserName);
                }
            });
        }, error =>
        {
            signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
            {
                Reason = error.ErrorMessage
            });
            Debug.Log("Login Failed " + error.ErrorMessage);
        });
    }

    void GoogleCustomLogin(string EmailUserName)
    {
        var req = new LoginWithCustomIDRequest();
        req.CustomId = EmailUserName;

        PlayFabClientAPI.LoginWithCustomID(req, OnLoginWithDeviceIDSuccess, OnLoginWithDeviceIDFailed);
    }
    public void LoginWithFaceBook()
    {
        swfb.SingInFacebook();
    }

    void PlayFabFacebookLogin(string accessToken)
    {
        OnFacebookLoginSuccessSignal signal;
        var request = new LoginWithFacebookRequest();
        request.AccessToken = accessToken;
        request.CreateAccount = true;

        PlayFabClientAPI.LoginWithFacebook(request, result =>
        {
            playFabAuthenticationContext = result.AuthenticationContext;
            signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
            {
                PlayerID = result.PlayFabId,
                NewPlayer = result.NewlyCreated,
                AuthenticationContext = result.AuthenticationContext
            });
            PlayerPrefs.SetString("LoginMethod", "Facebook");
        }, error => { Debug.Log("PlayFab Facebook login failed " + error.ErrorMessage); });
    }

    public void LoginWithAppleID()
    {
        if (PlayerPrefs.HasKey("AppleIDToken"))
            OnAppleLoginSuccess(new OnAppleLoginSuccessSignal()
            {
                IdToken = PlayerPrefs.GetString("AppleIDToken")
            });
        else
            _signinWithAppleID.SigninWithApple();
    }

    void OnAppleLoginSuccess(OnAppleLoginSuccessSignal signal)
    {
        var req = new LoginWithAppleRequest();
        req.CreateAccount = true;
        req.IdentityToken = signal.IdToken;
        PlayFabClientAPI.LoginWithApple(req, result =>
        {
            signalBus.Fire<OnPlayFabLoginSuccessesSignal>(new OnPlayFabLoginSuccessesSignal()
            {
                PlayerID = result.PlayFabId,
                NewPlayer = result.NewlyCreated,
                AuthenticationContext = result.AuthenticationContext
            });
            PlayerPrefs.SetString("LoginMethod", "AppleID");
            PlayerPrefs.SetString("AppleIDToken", signal.IdToken);
            Debug.Log("Playfab Apple login success");
        }, error =>
        {
            signalBus.Fire<OnLoginFailedSignal>(new OnLoginFailedSignal()
            {
                Reason = error.ErrorMessage
            });
            PlayerPrefs.DeleteKey("AppleIDToken");
            Debug.Log("Failed to Login with apple id Playfab " + error.ErrorMessage);
        });
    }

    public void LinkToAppleID()
    {
    }
}