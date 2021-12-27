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
    [Inject] private SignalBus signalBus;
    [Inject] private PlayerInventory _playerInventory;
    
    
    private void Start()
    {
        signalBus.Subscribe<OnCharacterAvatarChanged>(OnAvatarChanged);
       SetCharacterSprites(_playerInventory.EquippedAvatar());
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
        headRender.sprite =  avatar.headItem.sprite;
        headRender.transform.localPosition = body.headPosition;
        hairRender.sprite =avatar.hairItem==null ? null: avatar.hairItem.sprite;
        torsoRender.sprite =avatar.torsoItem==null ? null:avatar.torsoItem.TorsoVariants[variantIndex];
        torsoRender.transform.localPosition = body.torsoPosition;
        legsRender.sprite =avatar.legsItem==null?  null:avatar.legsItem.LegsVariants[variantIndex];
        legsRender.transform.localPosition = body.legsPosition;
        feetRender.sprite = avatar.feetItem == null ? null:avatar.feetItem.sprite;
        feetRender.transform.localPosition = body.feetPosition;

    }
    
    

  
    
}
