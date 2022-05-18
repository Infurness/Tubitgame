using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    private void Start()
    {
        signalBus.Subscribe<AssetsLoadedSignal>((signal =>
        {
            StartCoroutine(LoadSceneAsync(1));
        } ));
    }

    IEnumerator LoadSceneAsync(int index)
    {
        yield return new WaitForSecondsRealtime(0.5f);

        string playerName = PlayerDataManager.Instance.GetPlayerName();
        if (playerName == "No Data" || playerName == "Error: NoData")
            yield return  UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);//Tutorial
        else
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
    }
}
