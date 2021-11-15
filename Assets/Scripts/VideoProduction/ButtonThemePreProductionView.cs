using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Zenject;

public class ButtonThemePreProductionView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private SignalBus signalBus;
    public ThemeType themeType;
    [SerializeField] private TMP_Text buttonText;
    Button button;

    public void SetSignaBus (SignalBus incommingSignalBus)
    {
        signalBus = incommingSignalBus;
    }
    public void OnPointerDown (PointerEventData eventData)
    {
        if(button.IsInteractable())
            StartCoroutine (WaitForMoving());
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        StopAllCoroutines ();
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        StopAllCoroutines ();
    }

    private void Start ()
    {
        button = GetComponentInChildren<Button> ();
        buttonText.text = string.Concat (Enum.GetName (themeType.GetType (), themeType).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
    }

    IEnumerator WaitForMoving ()
    {
        yield return new WaitForSeconds (0);
        signalBus.Fire<ThemeHeldSignal>( new ThemeHeldSignal(){ themeBox = gameObject, themeType = themeType, buttonText = buttonText.text });
        GetComponent<Button> ().interactable = false;
    }
}
