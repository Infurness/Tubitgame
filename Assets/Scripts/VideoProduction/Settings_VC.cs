using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class Settings_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private GlobalAudioManager audioManager;

    [SerializeField] private Button editButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button deleteAccountButton;

    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;
    // Start is called before the first frame update
    void Start ()
    {
        inputField.onDeselect.AddListener (OnConfirm);
        inputField.onSubmit.AddListener (OnConfirm);
        editButton.onClick.AddListener (EditName);
        deleteAccountButton.onClick.AddListener (OpenDeleteAccount);

        musicVolumeSlider.onValueChanged.AddListener ((value) =>  audioManager.SetMusicVolumeModifier (value));
        effectsVolumeSlider.onValueChanged.AddListener ((value) => audioManager.SetEffectsVolumeModifier (value));

        audioManager.SetMusicVolumeModifier (musicVolumeSlider.value);
        audioManager.SetEffectsVolumeModifier (effectsVolumeSlider.value);

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
    IEnumerator RefreshName ()
    {
        yield return new  WaitForSecondsRealtime (1);
        signalBus.Fire<ChangeUsernameSignal> ();
    }
    void OpenDeleteAccount ()
    {
        signalBus.Fire<OpenDeleteAccountSignal> ();
    }
}

