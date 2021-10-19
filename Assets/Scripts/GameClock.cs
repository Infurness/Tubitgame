using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Zenject;
/*
 * Todo Tested in background on phone
 */
public class GameClock : MonoBehaviour
{
    public static GameClock Instance;
    [Inject] private SignalBus signalBus;
    
    public DateTime Now
    {
        get;
        private set;
    }
    private bool timeSeted;

    private void Awake()
    {
        
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>(signal =>
        {
            var timeReq = new GetTimeRequest();
            timeReq.AuthenticationContext = signal.AuthenticationContext;
            PlayFabClientAPI.GetTime(timeReq, (result =>
            {
                Now = result.Time;
                timeSeted = true;

            }), (error =>
            {
                print("Failed To Update DateTime");
            }));
        });

    }

 
   
// Update is called once per frame
    void Update()
    {
        if (timeSeted)
        {
          Now= Now.AddSeconds(Time.unscaledDeltaTime);
        //print(Now);
        }
    }
    
}
