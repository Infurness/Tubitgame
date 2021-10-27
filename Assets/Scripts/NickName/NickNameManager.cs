using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class NickNameManager : MonoBehaviour
{
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
        StartCoroutine (LoadSceneAsync (2));
    }
    public void UpdatePlayerName ()
    {
        PlayerDataManager.Instance.SetPLayerName (nickNameText.text);
    }
    IEnumerator LoadSceneAsync (int index)
    {
        yield return new WaitForSecondsRealtime (3);
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (index);

    }

}

