using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using Zenject;

public class ButtonSoundTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Inject] private GlobalAudioManager audioManager;
    [Inject] private SoundsHolder soundsHolder;

    bool selectedAsUnique;
    private void Awake()
    {
        ButtonSoundTrigger[] triggers = GetComponents<ButtonSoundTrigger>();
        if (triggers.Length > 1) //In case a button has this script twice
        {
            bool alreadySelected = false;
            foreach(ButtonSoundTrigger trigger in triggers)
            {
                if (trigger.selectedAsUnique)
                    alreadySelected = true;
            }
            if (alreadySelected && !selectedAsUnique)
                Destroy(this);
            else if (!alreadySelected)
                selectedAsUnique = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        audioManager = GlobalAudioManager.Instance;
        soundsHolder = SoundsHolder.Instance;
    }
    public void OnPointerDown (PointerEventData eventData)
    {
        if(!audioManager)
            audioManager = GlobalAudioManager.Instance;
        if (!soundsHolder)
            soundsHolder = SoundsHolder.Instance;
        audioManager.PlaySound(soundsHolder.pushButton, AudioType.Effect);
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        //audioManager.PlaySound(soundsHolder.pushButton, AudioType.Effect);
    }
}
