using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Customizations;
using UnityEngine;
using Zenject;
using  UnityEngine.UI;
public class CharacterRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer headRender;
    [SerializeField] private SpriteRenderer hairRender;
    [SerializeField] private SpriteRenderer torsoRender;
    [SerializeField] private SpriteRenderer legsRender;
    [SerializeField] private SpriteRenderer feetRender;
    [SerializeField] private SpriteRenderer seatedTorsoRender,seatedLegsRender,seatedHairRender,seatedBodyRender;
    
    [SerializeField] private SpriteRenderer idleBodyRender, idleTorsoRender, idleHairRender,idleLegsRender, idleFeetRender;

    [SerializeField] private GameObject seatedCharacter;
    [SerializeField] private GameObject idleCharacter;
    [Inject] private SignalBus signalBus;
    [Inject] private PlayerInventory _playerInventory;

    [Inject] private EnergyManager energyManager;
    private void Start()
    {
        signalBus.Subscribe<OnCharacterAvatarChanged>(OnAvatarChanged);
       SetCharacterSprites(_playerInventory.EquippedAvatar());
       
       signalBus.Subscribe<ChangeCharacterStateSignal>((signal) =>
       {
           switch (signal.state)
           {
               case CharacterState.Idle : 
                   SetIdleCharacterVisibility(true);
                   SetSeatedCharacterVisibility(false);
                   break;
               case CharacterState.Sleeping : 
                   SetIdleCharacterVisibility(false);
                   SetSeatedCharacterVisibility(false);
                   break;
               case  CharacterState.Production :
                   SetIdleCharacterVisibility(false);
                   SetSeatedCharacterVisibility(true);
                   break;
           }
       });
       


    }

    void OnAvatarChanged(OnCharacterAvatarChanged characterAvatarChanged)
    {

        SetCharacterSprites(characterAvatarChanged.NewAvatar);

    }

    void SetCharacterSprites(CharacterAvatar avatar)
    {
      
        int variantIndex = avatar.bodyItem.BodyIndex;
        var body = avatar.bodyItem;
        bodyRenderer.sprite = avatar.bodyItem.sprite;
        seatedBodyRender.sprite = avatar.bodyItem.SeatedBody;
        idleBodyRender.sprite = avatar.bodyItem.IdleBody;
        
        headRender.sprite =  avatar.headItem.sprite;
        headRender.transform.localPosition = body.headPosition;
        
        hairRender.sprite =avatar.hairItem==null ? null: avatar.hairItem.sprite;
        idleHairRender.sprite = avatar.hairItem == null ? null : avatar.hairItem.IdleHair;
        seatedHairRender.sprite = avatar.hairItem == null ? null : avatar.hairItem.SeatedHair;
        
        Sprite newTorso = avatar.torsoItem == null ? null : avatar.torsoItem.TorsoVariants[variantIndex];
        if(TutorialManager.Instance==null && torsoRender.sprite != newTorso)
        {
            signalBus.Fire<ChangeClothesAnimationSignal>(new ChangeClothesAnimationSignal {oldCloth =torsoRender.sprite, newCloth = newTorso });
            UnityEngine.Events.UnityAction changeAction = () => {torsoRender.sprite = newTorso;};
            signalBus.Subscribe<ChangeClothesVisualSignal>(()=> ChangeSpriteVFX(changeAction));
        }
        else
            torsoRender.sprite = newTorso;

        seatedTorsoRender.sprite = avatar.torsoItem == null ? null : avatar.torsoItem.SeatedTorso;
        idleTorsoRender.sprite = avatar.torsoItem == null ? null : avatar.torsoItem.idleTorso;
        torsoRender.transform.localPosition = body.torsoPosition;

        Sprite newLegs = avatar.legsItem == null ? null : avatar.legsItem.LegsVariants[variantIndex];
        if (TutorialManager.Instance == null && legsRender.sprite != newLegs)
        {
            signalBus.Fire<ChangeClothesAnimationSignal>(new ChangeClothesAnimationSignal { oldCloth = legsRender.sprite, newCloth = newLegs });
            UnityEngine.Events.UnityAction changeAction = () => { legsRender.sprite = newLegs; };
            signalBus.Subscribe<ChangeClothesVisualSignal>(() => ChangeSpriteVFX(changeAction));
        }
        else
            legsRender.sprite = newLegs;

        seatedLegsRender.sprite = avatar.legsItem == null ? null : avatar.legsItem.SeatedLegs;
        idleLegsRender.sprite = avatar.legsItem == null ? null : avatar.legsItem.IdleLegs;
        legsRender.transform.localPosition = body.legsPosition;

        Sprite newFeet = avatar.feetItem == null ? null : avatar.feetItem.sprite;
        if (TutorialManager.Instance == null && feetRender.sprite != newFeet)
        {
            signalBus.Fire<ChangeClothesAnimationSignal>(new ChangeClothesAnimationSignal { oldCloth = feetRender.sprite, newCloth = newFeet });
            UnityEngine.Events.UnityAction changeAction = () => { feetRender.sprite = newFeet; };
            signalBus.Subscribe<ChangeClothesVisualSignal>(() => ChangeSpriteVFX(changeAction));
        }
        else
            feetRender.sprite = newFeet;

        idleFeetRender.sprite = avatar.feetItem == null ? null : avatar.feetItem.IdleFeet;
        feetRender.transform.localPosition = body.feetPosition;
        if (avatar.feetItem && avatar.legsItem)
        {
            feetRender.sortingOrder =  avatar.legsItem.FeetSoringLayers;
            idleFeetRender.sortingOrder= avatar.legsItem.FeetSoringLayers+70;
        }

    }

     void SetSeatedCharacterVisibility(bool state)
    {
        seatedCharacter.SetActive(state);
    }

     void SetIdleCharacterVisibility(bool state)
    {
       idleCharacter.SetActive(state);
    }
    
    void ChangeSpriteVFX(UnityEngine.Events.UnityAction changeAction)
    {
        changeAction.Invoke();
        signalBus.TryUnsubscribe<ChangeClothesVisualSignal>(() => ChangeSpriteVFX(changeAction));     
    }
}

public enum CharacterState
{
    Idle,
    Sleeping,
    Production
}
