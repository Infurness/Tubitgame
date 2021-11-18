using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum PartnershipTiers {NonPartnership, Silver, Gold, Diamond, Ruby, RedDiamond };
[System.Serializable]
public struct RewardsData
{
    public int softCurrency;
    public int hardCurrency;
    //public List<InventoryItem> items; //Dummy: redo this when inventory is finished
}
public class RewardsManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private ulong[] subsForEachPartnershipLevel;
    [SerializeField] private RewardsData[] rewardsForEachPartnershipLevel;
    
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<ChangePlayerSubsSignal> (CheckRankUpdate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckRankUpdate (ChangePlayerSubsSignal signal)
    {
        int lastRank = CheckRank (signal.previousSubs);
        int actualRank = CheckRank (signal.subs);
        if (lastRank < actualRank)
            signalBus.Fire<UpdateRankSignal> (new UpdateRankSignal () { newTier=(PartnershipTiers)actualRank, rewardsData= rewardsForEachPartnershipLevel[actualRank-1]}); //Dummy this is not being called yet, the player should recieve a reward and some VC should make that appear on screen
    }
    int CheckRank(ulong subs)
    {
        int rank = 0;
        foreach (ulong levelXp in subsForEachPartnershipLevel)
        {
            if (subs >= levelXp)
            {
                rank++;
            }
            else
            {
                break;
            }
        }
        return rank;
    }
}
