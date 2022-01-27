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
    private void Awake()
    {
       
        
    }

    void Start()
    {
        energyManager = FindObjectOfType<EnergyManager>();
        roomVC = FindObjectOfType<RoomInventory_VC>();
        signalBus.Subscribe<ChangeRestStateSignal>(OnRestChanged);

        if (energyManager.GetPlayerIsResting().Equals(true))
        {
            SwitchToSleepingBed();
        }
        else
        {
            SwitchToNormalBed();
        }

    }

    void OnRestChanged(ChangeRestStateSignal signal)
    {
        if (energyManager.GetPlayerIsResting().Equals(true))
        {
            SwitchToSleepingBed();
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
            if (roomVC.EditModeEnabled==false)
            {
                print("Bed Clicked");
                signalBus.Fire<ChangeRestStateSignal>();
                OnRestChanged(null);
            }
           
        
        }

        private void OnDestroy()
        {
            signalBus.Unsubscribe<ChangeRestStateSignal>(OnRestChanged);

        }

        public void Dispose()
        {
            signalBus.Unsubscribe<ChangeRestStateSignal>(OnRestChanged);
        }

        public void Initialize()
        {
            
        }
}
