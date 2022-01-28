using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BedBehavior : MonoBehaviour ,IDisposable,IInitializable
{
    [SerializeField] private GameObject sleepBed;
    [SerializeField] private GameObject normalBed;
    [Inject] SignalBus signalBus;
    private EnergyManager energyManager;
    private RoomInventory_VC roomVC;
    private bool canBeUsed;
    private void Awake()
    {
       
        
    }

    void Start()
    {
        energyManager = FindObjectOfType<EnergyManager>();
        roomVC = FindObjectOfType<RoomInventory_VC>();
        signalBus.Subscribe<RestStateChangedSignal>(OnRestChanged);
        signalBus.Subscribe<CanUseItemsInRoom>((signal)=> canBeUsed = signal.canUse);
        if (energyManager.GetPlayerIsResting().Equals(true))
        {
            SwitchToSleepingBed();
        }
        else
        {
            SwitchToNormalBed();
        }

    }

    void OnRestChanged(RestStateChangedSignal signal)
    {

        if (signal.isResting)
        {
            SwitchToSleepingBed();
            signalBus.Fire<ChangeCharacterStateSignal>(new ChangeCharacterStateSignal()
            {
                state = CharacterState.Sleeping
            });
        }
        else
        {
            SwitchToNormalBed();
        }
    }
        void SwitchToSleepingBed()
        {
         sleepBed.gameObject.SetActive(true);
         normalBed.gameObject.SetActive(false);
        }

        void SwitchToNormalBed()
        {
            sleepBed.gameObject.SetActive(false);
            normalBed.gameObject.SetActive(true);
        }

    


        private void OnMouseDown()
        {
            if (TutorialManager.Instance != null)
                return;
            if (!canBeUsed)
            {
                return;
            }
            if (roomVC.EditModeEnabled==false)
            {
                print("Bed Clicked");
                signalBus.Fire<ChangeRestStateSignal>();
            }
           
        
        }

        private void OnDestroy()
        {
            signalBus.Unsubscribe<RestStateChangedSignal>(OnRestChanged);

        }

        public void Dispose()
        {
            signalBus.Unsubscribe<RestStateChangedSignal>(OnRestChanged);
        }

        public void Initialize()
        {
            
        }
}
