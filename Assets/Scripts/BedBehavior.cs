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
    private bool canBeUsed;
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

            if(!canBeUsed)
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
