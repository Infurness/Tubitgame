using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class YouTubeVideoManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private PlayerDataManger playerDataManger;
     private List<Video> videos;
    void Start()
    {
       _signalBus.Subscribe<PublishNewVideoSignal>(OnPublish);
       playerDataManger.gameObject.SetActive(false);
    }


    void OnPublish(PublishNewVideoSignal ps)
    {
        videos.Add(ps.Video);
    }
    void Update()
    {
        
    }
}
