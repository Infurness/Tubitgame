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

    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnPointerDown (PointerEventData eventData)
    {

    }

    public void OnPointerUp (PointerEventData eventData)
    {
        audioManager.PlaySound(soundsHolder.pushButton, AudioType.Effect);
    }
}
