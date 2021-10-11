using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

enum VideoManagerPanels {MakeAVideo, ManageVideos }
public class VideoManager_VC : MonoBehaviour
{

    [Inject] private SignalBus _signalBus;

    [SerializeField] private GameObject makeAVideoPanel;
    [SerializeField] private GameObject manageVideosPanel;
    [SerializeField] private Button makeAVideoButton;
    [SerializeField] private Button manageVideosButton;
    [SerializeField] private Button recordVideoButton;

    // Start is called before the first frame update
    void Start ()
    {
        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenManageVideosPanel);

        OpenMakeAVideoPanel ();
        makeAVideoButton.onClick.AddListener (OpenMakeAVideoPanel);
        manageVideosButton.onClick.AddListener (OpenManageVideosPanel);
        recordVideoButton.onClick.AddListener (OnRecordButtonPressed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OpenMakeAVideoPanel ()
    {
        OpenPanel (VideoManagerPanels.MakeAVideo);
    }
    void OpenManageVideosPanel ()
    {
        OpenPanel (VideoManagerPanels.ManageVideos);
    }
    void OpenPanel (VideoManagerPanels panel)
    {
        if (panel == VideoManagerPanels.MakeAVideo)
            makeAVideoPanel.SetActive (true);
        else
            makeAVideoPanel.SetActive (false);

        if (panel == VideoManagerPanels.ManageVideos)
            manageVideosPanel.SetActive (true);
        else
            manageVideosPanel.SetActive (false);
    }

    void OnRecordButtonPressed ()
    {
        //if (energyManager.GetEnergy () < 30)
        //    return;


        _signalBus.Fire<StartRecordingSignal> (new StartRecordingSignal ()
        {
            recordingTime = 3f
            //recordedThemes = selectedThemes.ToArray ()
        });
        _signalBus.Fire<AddEnergySignal> (new AddEnergySignal () { energyAddition = -30 });
    }
}
