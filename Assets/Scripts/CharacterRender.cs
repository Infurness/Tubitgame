using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using Zenject;

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
            case  HeadItem headItem:headRender.sprite =headItem.logoSprite;
                break;
            case  FaceItem faceItem:faceRender.sprite = faceItem.logoSprite;
                break;
            case  TorsoItem torsoItem: torsoRender.sprite = torsoItem.torsoSprite;
                break;
            case  LegsItem legsItem:legsRender.sprite = legsItem.logoSprite;
                break;
            case FeetItem feetItem: feetRender.sprite = feetItem.logoSprite;
                break;
        }
    }

  
    
}
