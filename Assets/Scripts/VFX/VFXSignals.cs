using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_EnergyChangeSignal
{
    public float oldFill;
    public float newFill;
}

public class VFX_LowEnergyBlinkSignal
{
}

public class VFX_NoEnergyParticlesSignal
{
}

public class VFX_CancelVideoAnimationSignal
{
    public Animator anim;
    public Action onEndAnimation;
}

public class VFX_StartMovingCoinsSignal
{
    public Vector3 origin;
}
public class VFX_StartMovingSCBillsSignal
{
    public Vector3 origin;
    public int quantity;
}
public class VFX_ActivateViralAnimation
{
    public string videoName;
}