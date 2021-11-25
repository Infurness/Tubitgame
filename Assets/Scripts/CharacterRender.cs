using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using Zenject;
using  UnityEngine.UI;
public class CharacterRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer headRender;
    [SerializeField] private SpriteRenderer faceRender;
    [SerializeField] private SpriteRenderer torsoRender;
    [SerializeField] private SpriteRenderer legsRender;
    [SerializeField] private SpriteRenderer feetRender;
    [Inject] private SignalBus signalBus;
    private void Start()
    {
        signalBus.Subscribe<OnCharacterItemEquippedSignal>(OnCharacterItemEquipped);
       
    }

    void OnCharacterItemEquipped(OnCharacterItemEquippedSignal characterItemEquippedSignal)
    {
        switch (characterItemEquippedSignal.ThemeCustomizationItem)
        {
            case  HeadItem headItem:headRender.sprite =headItem.sprite;
                break;
            case  FaceItem faceItem:faceRender.sprite = faceItem.sprite;
                break;
            case  TorsoItem torsoItem: torsoRender.sprite = torsoItem.sprite;
                break;
            case  LegsItem legsItem:legsRender.sprite = legsItem.sprite;
                break;
            case FeetItem feetItem: feetRender.sprite = feetItem.sprite;
                break;
        }
    }

  
    
}
