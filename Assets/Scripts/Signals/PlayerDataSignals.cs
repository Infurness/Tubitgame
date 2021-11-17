using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUsernameSignal
{
}
public class ChangePlayerSubsSignal
{
    public ulong previousSubs;
    public ulong subs;
}
public class UpdateRankSignal
{
    public PartnershipTiers newTier;
}

