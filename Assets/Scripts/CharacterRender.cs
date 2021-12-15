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
    [SerializeField] private Sprite defaultMaleHead, defaultFemaleHead,defaultMaleHair,defaultFemaleHair;
    
    
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
        var gender = avatar.bodyItem.GenderItemType; 
        Sprite defaultHeadSprite= avatar.bodyItem.GenderItemType ==GenderItemType.Male ? defaultMaleHead : defaultFemaleHead ;
        Sprite defaultHairSprite= avatar.bodyItem.GenderItemType ==GenderItemType.Male ? defaultMaleHair : defaultFemaleHair ;
        bodyRenderer.sprite = avatar.bodyItem.sprite;
        headRender.sprite = avatar.headItem.GenderItemType==gender ? avatar.headItem.sprite:defaultHeadSprite;
        headRender.transform.localPosition = body.headPosition;
        hairRender.sprite =avatar.hairItem.GenderItemType==gender ? avatar.hairItem.sprite:defaultHairSprite;
        torsoRender.sprite =avatar.torsoItem.GenderItemType==gender ? avatar.torsoItem.TorsoVariants[variantIndex]:null;
        torsoRender.transform.localPosition = body.torsoPosition;
        legsRender.sprite =avatar.legsItem.GenderItemType==gender ? avatar.legsItem.LegsVariants[variantIndex]:null;
        legsRender.transform.localPosition = body.legsPosition;
        feetRender.sprite =avatar.legsItem.GenderItemType==gender ? avatar.feetItem.sprite:null;
        feetRender.transform.localPosition = body.feetPosition;

    }
    
    

  
    
}
