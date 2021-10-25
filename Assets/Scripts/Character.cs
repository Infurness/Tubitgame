using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;

public class Character : MonoBehaviour
{
    private HeadItem characterHead;
    private FaceItem characterFace;
    private TorsoItem characterTorso;
    private LegsItem characterLegs;
    private FeetItem characterFeet;


    public void SetCharacterHead(HeadItem headItem)
    {
        characterHead = headItem;
        
    }

    public void SetCharacterTorso(TorsoItem torsoItem)
    {
        characterTorso = torsoItem;
    }

    public void SetCharacterFace(FaceItem faceItem)
    {
        characterFace = faceItem;
    }

    public void SetCharacterLegs(LegsItem legsItem)
    {
        characterLegs = legsItem;
    }
    public void SetCharacterFeet(FeetItem feetItem)
    {
        characterFeet = feetItem;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
