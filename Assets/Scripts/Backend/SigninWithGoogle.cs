using System;
using System.Threading.Tasks;
using Google;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class SigninWithGoogle
{
    private GoogleSignInConfiguration _configuration;

    public SigninWithGoogle(string clintId ="786436489167-rtuno9jd4smvkstqqmv7kjsj4rvkfdtq.apps.googleusercontent.com")
    {
        _configuration = new GoogleSignInConfiguration()
        {
            RequestEmail = true,
            WebClientId = clintId,
            RequestIdToken = true,
            RequestAuthCode = true,
            RequestProfile = true,
            
        };
        GoogleSignIn.Configuration = _configuration;
    }

    public async void SigninWithGoogleID(SignalBus signalBus)
    {
    
        await GoogleSingIn(signalBus);
    }
    private static async Task GoogleSingIn(SignalBus signalBus)
    {   
        Task<GoogleSignInUser> task;
        if (PlayerPrefs.HasKey("GoogleUser"))
        {
            task = GoogleSignIn.DefaultInstance.SignInSilently();
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
                signalBus.Fire(new OnGoogleSignInSuccessSignal()
                    {
                        AuthCode = task.Result.AuthCode,
                        EmailUserName = task.Result.Email+task.Result.DisplayName
                    });
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
