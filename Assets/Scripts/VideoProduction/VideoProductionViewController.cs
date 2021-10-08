using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VideoProductionViewController : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private PlayerDataManager playerDataManager;

    [SerializeField] private GameObject  productionPanel;
    [SerializeField] private Image recording_image;
    [SerializeField] private Button publishButton;
    private ThemeType[] videoThemes;
    
    void Start()
    {
        _signalBus.Subscribe<StartRecordingSignal>(StartRecording);
        publishButton.interactable = false;
        publishButton.onClick.AddListener (OnPublishButtonPressed);
    }


    void StartRecording(StartRecordingSignal recordingSignal)
    {
        productionPanel.SetActive(true);
        StartCoroutine(FillTheRecordImage(recordingSignal.recordingTime));
        videoThemes = recordingSignal.recordedThemes;
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

        //Dummy name Creation
        string videoName = $"{Enum.GetName (videoThemes[0].GetType (), videoThemes[0])}_{playerDataManager.GetPlayerTotalVideos()}" ;

        _signalBus.Fire<PublishVideoSignal> (new PublishVideoSignal ()
        {
             videoName = videoName, 
             videoThemes = videoThemes
        });
        _signalBus.Fire<StartPublishSignal> (new StartPublishSignal());
    }

    void Update()
    {
        
    }
}
