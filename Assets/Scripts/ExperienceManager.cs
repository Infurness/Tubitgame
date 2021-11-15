using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ExperienceManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    private PlayerDataManager playerDataManager;
    // Start is called before the first frame update
    void Start()
    {
        playerDataManager = PlayerDataManager.Instance;
        signalBus.Subscribe<AddSubsForExperienceSignal> (AddSubs);
        signalBus.Subscribe<AddViewsForExperienceSignal> (AddViews);
        signalBus.Subscribe<AddSoftCurrencyForExperienceSignal> (AddSoftCurrency);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddExperiencePoints (ulong experience)
    {
        //Set new experience
        int previousLevel = GetPlayerLevel ();
        playerDataManager.AddExperiencePoints (experience);
        //Check level up
        int currentLevel = GetPlayerLevel ();
        if (previousLevel < currentLevel)
        {
            signalBus.Fire<LevelUpSignal> (new LevelUpSignal () { level = currentLevel });
        }
    }
    public int GetPlayerLevel ()
    {
        ulong[] xpLevels =  {
                            0,
                            10,
                            50,
                            250,
                            1250,
                            6000,
                            30510,
                            152550,
                            762750,
                            3813000 };
        int level = 0;
        foreach (ulong levelXp in xpLevels)
        {
            if (playerDataManager.GetExperiencePoints () >= levelXp)
            {
                level++;
            }
            else
            {
                break;
            }
        }
        return level;
    }

    public void AddSubs (AddSubsForExperienceSignal signal)
    {
        Debug.Log ("CheckSubs Threshold");
        ulong totalSubs = signal.subs + (ulong)playerDataManager.GetSubsThreshold ();
        long remainder;
        long experiencePoints = Math.DivRem ((long)totalSubs, 100, out remainder);

        playerDataManager.SetSubsThreshold((int)remainder);
        AddExperiencePoints ((ulong)experiencePoints);
    }
    public void AddViews (AddViewsForExperienceSignal signal)
    {
        Debug.Log ("CheckViews Threshold");
        ulong totalviews = signal.views + (ulong)playerDataManager.GetViewsThreshold ();
        long remainder;
        long experiencePoints = Math.DivRem ((long)totalviews, 1000, out remainder);
        playerDataManager.SetViewsThreshold ((int)remainder);
        AddExperiencePoints ((ulong)experiencePoints);
    }
    public void AddSoftCurrency (AddSoftCurrencyForExperienceSignal signal)
    {
        Debug.Log ("CheckSoftCurrency Threshold");
        ulong totalSoftCurrency = signal.softCurrency + (ulong)playerDataManager.GetSoftCurrencyThreshold ();
        long remainder;
        long experiencePoints = Math.DivRem ((long)totalSoftCurrency, 100, out remainder);

        playerDataManager.SetSoftCurrencyThreshold ((int)remainder);
        AddExperiencePoints ((ulong)experiencePoints);
    }
}