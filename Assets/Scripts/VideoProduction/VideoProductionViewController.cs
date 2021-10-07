using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VideoProductionViewController : MonoBehaviour
{
    [Inject] private SignalBus _SignalBus;
    [SerializeField] private GameObject  productionPanel;
    [SerializeField] private Image recording_image;
    [SerializeField] private Button publishButton;
    
    void Start()
    {
        _SignalBus.Subscribe<StartRecordingSignal>(StartRecording);
        publishButton.interactable = false;
        publishButton.onClick.AddListener (OnPublishButtonPressed);
    }


    void StartRecording(StartRecordingSignal recordingSignal)
    {
        productionPanel.SetActive(true);
        StartCoroutine(FillTheRecordImage(recordingSignal.RecordingTime));
    }
    
    IEnumerator FillTheRecordImage(float time)
    {

        float tACC = 0;
        while (tACC<time)
        {
            yield return new WaitForEndOfFrame();
            tACC += Time.deltaTime;
            recording_image.fillAmount=tACC/time;
        }
        publishButton.interactable = true;
        
    }

    private void OnPublishButtonPressed ()
    {
        publishButton.interactable = false;
        productionPanel.SetActive (false);
        _SignalBus.Fire<StartPublishSignal> (new StartPublishSignal());
    }

    void Update()
    {
        
    }
}
