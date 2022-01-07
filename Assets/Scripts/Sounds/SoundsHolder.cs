using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsHolder : MonoBehaviour
{
    public static SoundsHolder Instance;

    [Header("Music")]
    public AudioClip shopMusic;
    public AudioClip generalMusic;

    [Header("Effects")]
    public AudioClip pushButton;
    public AudioClip popUp;
    public AudioClip rankUp;
    public AudioClip enterShop;
    public AudioClip buyItem;
    public AudioClip streetSounds;
    public AudioClip viralPopUp;
    private void Start()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }
}
