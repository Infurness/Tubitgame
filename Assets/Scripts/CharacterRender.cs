using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Camera idleRenderCam;
    [Inject] private SignalBus signalBus;
    [Inject] private PlayerInventory _playerInventory;
    
    
    private void Start()
    {
        signalBus.Subscribe<OnCharacterAvatarChanged>(OnAvatarChanged);
       SetCharacterSprites(_playerInventory.EquippedAvatar());
       
       signalBus.Subscribe<ChangeSeatedCharacterVisibilitySignal>((signal) =>
       {
           SetSeatedCharacterVisibility(signal.Visibility);
       });
       
       signalBus.Subscribe<ChangeIdleCharacterVisibilitySignal>((signal) =>
       {
           SetIdleCharacterVisibility(signal.Visibility);
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
        
        torsoRender.sprite =avatar.torsoItem==null ? null:avatar.torsoItem.TorsoVariants[variantIndex];
        seatedTorsoRender.sprite = avatar.torsoItem == null ? null : avatar.torsoItem.SeatedTorso;
        idleTorsoRender.sprite = avatar.torsoItem == null ? null : avatar.torsoItem.idleTorso;
        torsoRender.transform.localPosition = body.torsoPosition;
        
        legsRender.sprite =avatar.legsItem==null?  null:avatar.legsItem.LegsVariants[variantIndex];
        seatedLegsRender.sprite = avatar.legsItem == null ? null : avatar.legsItem.SeatedLegs;
        idleLegsRender.sprite = avatar.legsItem == null ? null : avatar.legsItem.IdleLegs;
        legsRender.transform.localPosition = body.legsPosition;
        
        feetRender.sprite = avatar.feetItem == null ? null:avatar.feetItem.sprite;
        idleFeetRender.sprite = avatar.feetItem == null ? null : avatar.feetItem.IdleFeet;
        feetRender.transform.localPosition = body.feetPosition;
        if (avatar.feetItem && avatar.legsItem)
        {
            feetRender.sortingOrder =  avatar.legsItem.FeetSoringLayers;
            idleFeetRender.sortingOrder= avatar.legsItem.FeetSoringLayers;
        }

    }

     void SetSeatedCharacterVisibility(bool state)
    {
        seatedBodyRender.gameObject.SetActive(state);
    }

     void SetIdleCharacterVisibility(bool state)
    {
        idleBodyRender.gameObject.SetActive(state);
     //   idleRenderCam.gameObject.SetActive(state);
        idleRenderCam.targetTexture.Release();
    }
    
}
