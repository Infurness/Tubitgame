using System;
using UnityEngine;
using Zenject;

public class ComputerBehaviour : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    private RoomInventory_VC roomVC;
    private bool canBeUsed;

    private void Start()
    {
        roomVC = FindObjectOfType<RoomInventory_VC>();
        signalBus.Subscribe<CanUseItemsInRoom>((signal) => canBeUsed = signal.canUse);
    }

    private void OnMouseDown()
    {
       
        if (!canBeUsed)
        {
            return;
        }
        if (roomVC.EditModeEnabled==false)
        {
            signalBus.Fire<ShowVideosStatsSignal>();
            GlobalAudioManager.Instance.PlaySound(SoundsHolder.Instance.pushButton, AudioType.Effect);
        }
    }



}
