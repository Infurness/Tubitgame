using System;
using UnityEngine;
using Zenject;

public class ComputerBehaviour : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    private bool canBeUsed=true;
    private GameObject roomPanel;


    private void Start()
    {
        signalBus.Subscribe<CanUseItemsInRoom>(ChangeUseState);
        roomPanel = GameObject.Find("RoomPanel");
    }
    void ChangeUseState(CanUseItemsInRoom signal)
    {
        canBeUsed = signal.canUse;
    }

    private void OnMouseDown()
    {
        if (TutorialManager.Instance != null)
            return;
        if (!canBeUsed)
        {
            return;
        }
        if(roomPanel != null)
            roomPanel.SetActive(false);
        signalBus.Fire<ShowVideosStatsSignal>();
        GlobalAudioManager.Instance.PlaySound(SoundsHolder.Instance.pushButton, AudioType.Effect);
        }


    private void OnDestroy()
    {
        signalBus.Unsubscribe<CanUseItemsInRoom>(ChangeUseState);
    }
}
