using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
public class GameAnalyticsManager : MonoBehaviour
{
    public static GameAnalyticsManager Instance=null;


    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        GameAnalytics.Initialize();
    }

    void Update()
    {
        
    }
}
