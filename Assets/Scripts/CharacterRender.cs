using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using Zenject;

public class CharacterRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer headSprite;
    [SerializeField] private SpriteRenderer faceSprite;
    [SerializeField] private SpriteRenderer torsoSprite;
    [SerializeField] private SpriteRenderer legsSprite;
    [SerializeField] private SpriteRenderer feetSprite;
    [Inject] private SignalBus signalBus;
    private void Start()
    {
        signalBus.Subscribe<OnHeadEquippedSignal>(SetCharacterHead);
        signalBus.Subscribe<OnFaceEquippedSignal>(SetCharacterFace);
        signalBus.Subscribe<OnTorsoEquippedSignal>(SetCharacterTorso);
        signalBus.Subscribe<OnLegsEquippedSignal>(SetCharacterLegs);
        signalBus.Subscribe<OnFeetEquippedSignal>(SetCharacterFeet);
    }

    public void SetCharacterHead(OnHeadEquippedSignal headEquipped)
    {

        headSprite.sprite =  headEquipped.HeadItem.headSprite;
    }

    public void SetCharacterTorso(OnTorsoEquippedSignal torsoEquipped)
    {
        torsoSprite.sprite = torsoEquipped.TorsoItem.torsoSprite;
    }

    public void SetCharacterFace(OnFaceEquippedSignal faceEquipped)
    {
        faceSprite.sprite = faceEquipped.FaceItem.faceSprite;
    }

    public void SetCharacterLegs(OnLegsEquippedSignal legsEquipped)
    {
        legsSprite.sprite = legsEquipped.LegsItem.legSprite;
    }
    public void SetCharacterFeet(OnFeetEquippedSignal feetEquipped)
    {
        feetSprite.sprite = feetEquipped.FeetItem.feetSprite;
    }
    
}
