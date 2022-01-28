using System;
using UnityEngine;
using Zenject;

public class ComputerBehaviour : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    private bool canBeUsed;

    private void Start()
    {
        signalBus.Subscribe<CanUseItemsInRoom>(ChangeUseState);
    }
    void ChangeUseState(CanUseItemsInRoom signal)
    {
        canBeUsed = signal.canUse;
    }

    private void OnMouseDown()
    {
       
        if (!canBeUsed)
        {
            return;
        }
        
        
            signalBus.Fire<ShowVideosStatsSignal>();
            GlobalAudioManager.Instance.PlaySound(SoundsHolder.Instance.pushButton, AudioType.Effect);
        }


    private void OnDestroy()
    {
        signalBus.Unsubscribe<CanUseItemsInRoom>(ChangeUseState);
    }
}
