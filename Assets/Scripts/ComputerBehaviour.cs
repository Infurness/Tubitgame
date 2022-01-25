using System;
using UnityEngine;
using Zenject;

public class ComputerBehaviour : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    private void OnMouseDown()
    {
        signalBus.Fire<ShowVideosStatsSignal>();
    }
}
