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
        headRender.sprite =avatar.headItem.sprite;
        hairRender.sprite =avatar.hairItem.sprite;
        torsoRender.sprite = avatar.torsoItem.TorsoVariants[variantIndex];
        legsRender.sprite = avatar.legsItem.LegsVariants[variantIndex];
        feetRender.sprite =avatar.feetItem.sprite;
    }
    
    

  
    
}
