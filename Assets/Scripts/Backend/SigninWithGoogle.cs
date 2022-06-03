using System;
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
        var hasUser = FileManager.LoadFromFile("GoogleUser.json", out string result);
        if (hasUser)
        {
            var savedUser = new GoogleSignInUser();
            savedUser = JsonConvert.DeserializeObject<GoogleSignInUser>(result);

            if (!string.IsNullOrEmpty(savedUser.AuthCode))
            {
                signalBus.Fire(new OnGoogleSignInSuccessSignal());
            }
            else
            {
                var task = GoogleSignIn.DefaultInstance.SignIn();

                try
                {
                    await task;
                    if (!task.IsFaulted)
                    {
                        Debug.Log("Login with Google Success");
                        var googleUser = JsonConvert.SerializeObject(task.Result);
                        FileManager.WriteToFile("GoogleUser.json", googleUser);
                        //OnGoogleSingedIn(task.Result);
                        signalBus.Fire(new OnGoogleSignInSuccessSignal()
                        {
                            AuthCode = task.Result.AuthCode,
                            IdToken = task.Result.IdToken
                        });
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
        }
        else
        {
            var task = GoogleSignIn.DefaultInstance.SignIn();
            try
            {
                await task;
                if (!task.IsFaulted)
                {
                    Debug.Log("Login with Google Success");
                    var googleUser = JsonConvert.SerializeObject(task.Result);
                    FileManager.WriteToFile("GoogleUser.json", googleUser);
                    
                    signalBus.Fire(new OnGoogleSignInSuccessSignal()
                    {
                        AuthCode = task.Result.AuthCode,
                        IdToken = task.Result.IdToken
                    });
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
    }

    public void LogOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }
}
