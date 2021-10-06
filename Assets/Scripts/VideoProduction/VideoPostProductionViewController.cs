using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VideoPostProductionViewController : MonoBehaviour
{
    [Inject] private SignalBus _SignalBus;
    [SerializeField] private GameObject postProductionPanel;
    void Start()
    {
        _SignalBus.Subscribe<StartPublishSignal> (PublishVideo);
    }

    void Update()
    {
        
    }

    void PublishVideo ()
    {
        postProductionPanel.SetActive (true);
        Debug.Log ("Video published");
    }
}
