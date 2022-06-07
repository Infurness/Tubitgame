using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

[Serializable]
public class TokenClassName
{
    public string access_token;
}


public class SigninWithGoogle
{
    private GoogleSignInConfiguration _configuration;

    private static void GetAccessToken(string authToken, Action<string> result)
    {
        var content = new Dictionary<string, string>();

        content.Add("client_id", "786436489167-rtuno9jd4smvkstqqmv7kjsj4rvkfdtq.apps.googleusercontent.com");
        content.Add("client_secret", "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");
        content.Add("code", authToken);
        content.Add("grant_type", "authorization_code");
        content.Add("redirect_uri", "https://oauth.playfab.com/oauth2/google");

        var www = UnityWebRequest.Post("https://oauth2.googleapis.com/token", content);

        www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.ConnectionError)
        {
            string resultContent = www.downloadHandler.text;
            TokenClassName json = JsonUtility.FromJson<TokenClassName>(resultContent);

            result(json.access_token);
        }
        else
        {
            result("");
        }
    }


    public SigninWithGoogle(string clientID = "786436489167-rtuno9jd4smvkstqqmv7kjsj4rvkfdtq.apps.googleusercontent.com")
    {
#if UNITY_IOS
clientID = "786436489167-vi6acu8rehq7ug9ghj2k22oa43q2sb7b.apps.googleusercontent.com";
#endif
        _configuration = new GoogleSignInConfiguration()
        {
            RequestEmail = true,
            WebClientId = clientID,
            RequestAuthCode = true,
            RequestIdToken = true,
            ForceTokenRefresh = true, 
            UseGameSignIn = false,
        };
        GoogleSignIn.Configuration = _configuration;
    }

    public async void SigninWithGoogleID(SignalBus signalBus)
    {
    
        await GoogleSingIn(signalBus);
    }
    private static async Task GoogleSingIn(SignalBus signalBus)
    {   
        var isSilently = false;
        Task<GoogleSignInUser> task;
        if (PlayerPrefs.HasKey("GoogleUser"))
        {
            task = GoogleSignIn.DefaultInstance.SignInSilently();
            isSilently = true;
        }
        else
        {
            task = GoogleSignIn.DefaultInstance.SignIn();
        }

        try
        {
            await task;
            if (!task.IsFaulted)
            {
                if(isSilently)
                {
                    GetAccessToken(task.Result.AuthCode, (token)=>
                    {
                        if(!string.IsNullOrEmpty(token))
                        {
                            signalBus.Fire(new OnGoogleSignInSuccessSignal()
                            {
                                AuthCode = token,
                            });
                        }
                    });
                }
                else
                {
                    signalBus.Fire(new OnGoogleSignInSuccessSignal()
                    {
                        AuthCode = task.Result.AuthCode,
                    });
                }

                Debug.Log("Login with Google Success");
                PlayerPrefs.SetString("GoogleUser", "googleUser");
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogError("Failed to login with google ");
                signalBus.Fire<OnGoogleSignInFailed>(new OnGoogleSignInFailed());
            }
        }
        catch (Exception e)
        {
            Debug.LogError(task.Exception);
            Debug.LogException(e);

            signalBus.Fire<OnGoogleSignInFailed>(new OnGoogleSignInFailed()
            {
                Reason = e.Message
            });
            throw;
        }
    }

    public void LogOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }
}