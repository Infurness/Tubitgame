using System;
using System.Collections;
using System.Collections.Generic;
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
        yield return new WaitForSecondsRealtime(3);
        
      yield return  UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);

    }
}
