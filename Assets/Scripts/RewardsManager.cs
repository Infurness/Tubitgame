using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum PartnershipTiers {NonPartnership, Silver, Gold, Diamond, Ruby, RedDiamond };
public class RewardsManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
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
            signalBus.Fire<UpdateRankSignal> (new UpdateRankSignal () { newTier=(PartnershipTiers)actualRank}); //Dummy this is not being called yet, the player should recieve a reward and some VC should make that appear on screen
    }
    int CheckRank(ulong subs)
    {
        ulong[] partnershipLevels = 
                                {
                                0,
                                100000,
                                1000000,
                                10000000,
                                50000000,
                                100000000,
                                };

        int rank = 0;
        foreach (ulong levelXp in partnershipLevels)
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
