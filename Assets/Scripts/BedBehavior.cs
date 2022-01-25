using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class BedBehavior : MonoBehaviour
{
    [SerializeField] private GameObject sleepBed;
    [SerializeField] private GameObject normalBed;
    [Inject] SignalBus signalBus;
    private EnergyManager energyManager;

    private void Awake()
    {
       
        
    }

    void Start()
    {
        energyManager = FindObjectOfType<EnergyManager>();
   
        signalBus.Subscribe<ChangeRestStateSignal>((signal) =>
        { 
            if (energyManager.GetPlayerIsResting())
                {
                    SwitchToNormalBed();
                }
                else
                {
                    SwitchToSleepingBed();
                }
            
        });
        
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
            print("Bed Clicked");
            signalBus.Fire<ChangeRestStateSignal>();
        
        }

}
