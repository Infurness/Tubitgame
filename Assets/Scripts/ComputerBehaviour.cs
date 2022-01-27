using System;
using UnityEngine;
using Zenject;

public class ComputerBehaviour : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    private RoomInventory_VC roomVC;


    private void Start()
    {
        roomVC = FindObjectOfType<RoomInventory_VC>();
    }

    private void OnMouseDown()
    {
        if (roomVC.EditModeEnabled==false)
        {
            signalBus.Fire<ShowVideosStatsSignal>();

        }
    }



}
