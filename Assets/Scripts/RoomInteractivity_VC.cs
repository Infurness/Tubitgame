using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class RoomInteractivity_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] RoomInteractivityManager roomInteractivityManager;
    [Inject] EnergyManager energyManager;

    [SerializeField] private GameObject bed;
    [SerializeField] private Button bedButton;
    [SerializeField] private GameObject computer;
    [SerializeField] private Button computerButton;
    // Start is called before the first frame update
    void Start()
    {
        bedButton.GetComponent<Image> ().color = Color.clear;
        bedButton.GetComponentInChildren<TMP_Text> ().color = Color.clear;
        bedButton.transform.position = roomInteractivityManager.GetItemPositionInScreen (bed);

        computerButton.GetComponent<Image> ().color = Color.clear;
        computerButton.GetComponentInChildren<TMP_Text> ().color = Color.clear;
        computerButton.transform.position = roomInteractivityManager.GetItemPositionInScreen (computer);

        bedButton.onClick.AddListener (StartResting);
        computerButton.onClick.AddListener (OpenVideoManager);
    }

    void OpenVideoManager ()
    {
        signalBus.Fire<ShowVideosStatsSignal> ();
    }

    void StartResting ()
    {
        signalBus.Fire<ChangeRestStateSignal> ();
    }
}
