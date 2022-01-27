using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChairBehaviour : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    void Start()
    {
        signalBus.Subscribe<ChangeCharacterStateSignal>(OnCharacterPoseChanged);
        
    }

    void OnCharacterPoseChanged(ChangeCharacterStateSignal signal)
    {
        switch (signal.state)
        {
            case  CharacterState.Sleeping : ChangeMasksVisibility(false);break;
            case  CharacterState.Idle : ChangeMasksVisibility(false);break;          
            case  CharacterState.Production : ChangeMasksVisibility(true);break;
        }   
    }

    void ChangeMasksVisibility(bool state)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(state);
        }
    }

    private void OnDestroy()
    {
        signalBus.Unsubscribe<ChangeCharacterStateSignal>(OnCharacterPoseChanged);
    }
}
