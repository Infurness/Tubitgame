using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BedBehavior : MonoBehaviour 
{
    [SerializeField] private GameObject sleepBed;
    [SerializeField] private GameObject normalBed;
    [Inject] SignalBus signalBus;
    private EnergyManager energyManager;
    private bool canBeUsed=true;
    private void Awake()
    {
       
        
    }

    void Start()
    {
        energyManager = FindObjectOfType<EnergyManager>();
        signalBus.Subscribe<RestStateChangedSignal>(OnRestChanged);
        signalBus.Subscribe<CanUseItemsInRoom>(ChangeUseState);
        if (energyManager.GetPlayerIsResting().Equals(true))
        {
            SwitchToSleepingBed();
        }
        else
        {
            SwitchToNormalBed();
        }

    }

    void ChangeUseState(CanUseItemsInRoom signal)
    {
        canBeUsed = signal.canUse;
    }

    void OnRestChanged(RestStateChangedSignal signal)
    {

        if (signal.isResting)
        {
            signalBus.Fire<VFX_GoToSleepSignal>(new VFX_GoToSleepSignal { goToSleep = true });
            SwitchToSleepingBed();
            signalBus.Fire<ChangeCharacterStateSignal>(new ChangeCharacterStateSignal()
            {
                state = CharacterState.Sleeping
            });
        }
        else
        {
            signalBus.Fire<VFX_GoToSleepSignal>(new VFX_GoToSleepSignal { goToSleep = false });
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
           
                print("Bed Clicked");
                signalBus.Fire<ChangeRestStateSignal>();
            
           
        
        }

        private void OnDestroy()
        {
            signalBus.Unsubscribe<RestStateChangedSignal>(OnRestChanged);
            signalBus.Unsubscribe<CanUseItemsInRoom>(ChangeUseState);

        }

      

      
}
