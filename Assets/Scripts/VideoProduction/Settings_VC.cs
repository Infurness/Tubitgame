using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class Settings_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private Button editButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button closeButton;
    // Start is called before the first frame update
    void Start ()
    {
        inputField.onDeselect.AddListener (OnConfirm);
        inputField.onSubmit.AddListener (OnConfirm);
        editButton.onClick.AddListener (EditName);
        closeButton.onClick.AddListener (ClosePanel);

        inputField.interactable = false;
        RefreshPlayerName ();
    }
    void EditName ()
    {
        inputField.interactable = true;
        inputField.Select ();
    }
    void OnConfirm (string value)
    {
        Debug.Log (value);
        UpdatePlayerName (value);
        inputField.interactable = false;
    }
    public void UpdatePlayerName (string value)
    {
        PlayerDataManager.Instance.SetPLayerName (value);
        StartCoroutine (RefreshName());
    }
    void RefreshPlayerName ()
    {
        inputField.text = PlayerDataManager.Instance.GetPlayerName ();
    }
    void ClosePanel ()
    {
        signalBus.Fire<CloseSettingPanelSignal> ();
    }
    IEnumerator RefreshName ()
    {
        yield return new  WaitForSecondsRealtime (1);
        signalBus.Fire<ChangeUsernameSignal> ();
    }
}

