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
        signalBus.Subscribe<AddExperiencePointsSignal> (ModifyExperience);
        signalBus.Subscribe<AddSoftCurrencyForExperienceSignal> (AddSoftCurrency);       
    }

    public void AddExperiencePoints (ulong experience)
    {
        //Set new experience
        int previousLevel = GetPlayerLevel ();
        playerDataManager.AddExperiencePoints (experience);
        //Check level up
        int currentLevel = GetPlayerLevel ();
        if (currentLevel<xpForEachLevel.Length && previousLevel < currentLevel)
        {
            signalBus.Fire<LevelUpSignal> (new LevelUpSignal () { level = currentLevel, reward=rewardsForEachLevel[currentLevel] });    
        }
        signalBus.Fire<UpdateExperienceSignal> ();
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
            if (playerDataManager.GetExperiencePoints () > levelXp)
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
    private void ModifyExperience(AddExperiencePointsSignal signal)
    {
        AddSubs(signal.subs);
        AddViews(signal.views);
        playerDataManager.UpdateXpData();
    }

    private void AddSubs (ulong subs)
    {
        ulong totalSubs = subs + (ulong)playerDataManager.GetSubsThreshold ();
        long remainder;
        long experiencePoints = Math.DivRem ((long)totalSubs, 100, out remainder);

        playerDataManager.SetSubsThreshold((int)remainder);
        if (TutorialManager.Instance != null)
            return;
        AddExperiencePoints ((ulong)experiencePoints);
    }
    private void AddViews (ulong views)
    {
        ulong totalviews = views + (ulong)playerDataManager.GetViewsThreshold ();
        long remainder;
        long experiencePoints = Math.DivRem ((long)totalviews, 1000, out remainder);
        playerDataManager.SetViewsThreshold ((int)remainder);
        if (TutorialManager.Instance != null)
            return;
        AddExperiencePoints ((ulong)experiencePoints);
    }
    public void AddSoftCurrency (AddSoftCurrencyForExperienceSignal signal)
    {
        ulong totalSoftCurrency = signal.softCurrency + (ulong)playerDataManager.GetSoftCurrencyThreshold ();
        long remainder;
        long experiencePoints = Math.DivRem ((long)totalSoftCurrency, 100, out remainder);

        playerDataManager.SetSoftCurrencyThreshold ((int)remainder);
        if (TutorialManager.Instance != null)
            return;
        AddExperiencePoints ((ulong)experiencePoints);
        playerDataManager.UpdateXpData();
    }

    public ulong GetXpThreshold(int level)
    {
        if (level < xpForEachLevel.Length)
            return xpForEachLevel[level];
        else
            return xpForEachLevel[xpForEachLevel.Length-1];
    }

    public RewardsData GetRewardData (int level)
    {
       return rewardsForEachLevel[level];
    }

    public void CheatResetXp ()
    {
        playerDataManager.CheatResetXp ();
        signalBus.Fire<UpdateExperienceSignal> ();
    }
}
