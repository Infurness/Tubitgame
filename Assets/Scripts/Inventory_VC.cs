using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Inventory_VC : MonoBehaviour
{
    [SerializeField] private Button headBt, faceBt, torsoBt, legsBt, feetBt;
    [Inject] private PlayerInventory playerInventory;
    void Start()
    {
        
    }

    private void OnEnable()
    { 
        headBt.image.sprite = playerInventory.currentHead.logoSprite;
        faceBt.image.sprite = playerInventory.currentFace.logoSprite;
        torsoBt.image.sprite = playerInventory.currentTorso.logoSprite;
        legsBt.image.sprite = playerInventory.currentLegs.logoSprite;
        feetBt.image.sprite = playerInventory.currentFeet.logoSprite;
    }
    

    void Update()
    {
        
    }
}
