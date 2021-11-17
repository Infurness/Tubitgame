using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ExperienceManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    private PlayerDataManager playerDataManager;

    [SerializeField] private ulong[] xpForEachLevel;
    [SerializeField] private RewardsData[] rewardsForEachLevel;
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
        if (Input.GetKeyDown (KeyCode.L))
        {
            Add1LevelCheat ();
        }
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
            signalBus.Fire<LevelUpSignal> (new LevelUpSignal () { level = currentLevel, reward=rewardsForEachLevel[currentLevel-1] });
        }
        signalBus.Fire<UpdateExperienceSignal> ();
    }

    public void Add1LevelCheat ()
    {
        int currentLevel = GetPlayerLevel ();
        if (currentLevel < rewardsForEachLevel.Length-1)
        {
            signalBus.Fire<LevelUpSignal> (new LevelUpSignal () { level = currentLevel + 1, reward = rewardsForEachLevel[currentLevel] });
        }
        else
        {
            signalBus.Fire<LevelUpSignal> (new LevelUpSignal () { level = currentLevel, reward = rewardsForEachLevel[currentLevel - 1] });
        }
    }
    public ulong GetPlayerXp ()
    {
        return playerDataManager.GetExperiencePoints ();
    }
    public int GetPlayerLevel ()
    {
        if (playerDataManager == null)
            playerDataManager = PlayerDataManager.Instance;
        int level = 0;
        foreach (ulong levelXp in xpForEachLevel)
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

    public ulong GetXpThreshold(int level)
    {
        return xpForEachLevel[level];
    }

    public RewardsData GetRewardData (int level)
    {
       return rewardsForEachLevel[level];
    }
}
