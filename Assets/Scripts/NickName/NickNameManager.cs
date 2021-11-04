using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Zenject;

public class NickNameManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text nickNameText;
    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener (OnConfirm);
    }
    void OnConfirm ()
    {
        UpdatePlayerName ();
    }
    public void UpdatePlayerName ()
    {
        PlayerDataManager.Instance.SetPLayerName (nickNameText.text);
        signalBus.Fire<ChangeUsernameSignal> ();
    }
}
