﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using Zenject;

public class SigninWithGoogle
{
    private readonly SignalBus signalBus;
    private GoogleSignInConfiguration configuration;

        public SigninWithGoogle(SignalBus sb=null,string webclientId="786436489167-rtuno9jd4smvkstqqmv7kjsj4rvkfdtq.apps.googleusercontent.com")
        {
            signalBus = sb;
            configuration = new GoogleSignInConfiguration()
            {
                WebClientId = webclientId,
                RequestIdToken = true,
                RequestAuthCode = true ,
                UseGameSignIn = false
                
            };
            GoogleSignIn.Configuration = configuration;

        }


        public async Task<GoogleSignInUser> SingInWithGoogleID()
        {
            var loginTask = GoogleSignIn.DefaultInstance.SignIn();
            await loginTask;
            
            OnAuthenticationFinished(loginTask);
            return loginTask.Result;
        }
        
        
         void OnAuthenticationFinished(Task<GoogleSignInUser> task) {
            if (task.IsFaulted) {
                using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator()) {
                    if (enumerator.MoveNext()) {
                        GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                        Debug.Log("Got Error: " + error.Status + " " + error.Message);
                        signalBus.Fire<OnGoogleSignInFailed>(new OnGoogleSignInFailed()
                        {
                            reason = error.Message
                        });
                        
                    } else {
                        Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                    }
                }
            } else if(task.IsCanceled) {
                Debug.Log("Canceled");
            } else  {
                Debug.Log("Welcome: " + task.Result.AuthCode+ "!");

                signalBus.Fire(new OnGoogleSignInSuccessSignal()
                {
                    authCode =task.Result.AuthCode
                    
                });
               
            }
        }
         
         
         public void SignOut() {
             Debug.Log("Calling SignOut");
             GoogleSignIn.DefaultInstance.SignOut();
         }

         public void Disconnect() {
             Debug.Log("Calling Disconnect");
             GoogleSignIn.DefaultInstance.Disconnect();
         }
        
    }
