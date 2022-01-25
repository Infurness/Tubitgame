using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using UnityEngine;
public class GameAnalyticsManager : MonoBehaviour,IGameAnalyticsATTListener
{
    public static GameAnalyticsManager Instance=null;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            GameAnalytics.Initialize();
        }
    }

    public void SendCustomEvent(string eventName, object[] parameters=null)
    {
        if (parameters==null)
        {
            GameAnalytics.NewDesignEvent(eventName);
            return;
        }
        Dictionary<string, object> fields = new Dictionary<string, object>();
        foreach (var parameter in parameters)
        {
            fields.Add(parameter.ToString(),parameter);
        }

        GA_Design.NewEvent(eventName,fields);
    }
    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }
}