using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum TutorialPhase { Phase1, Phase2, Phase3, Phase4, Phase5};
public class TutorialManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    public static TutorialManager Instance;

    TutorialPhase currentTutorialPhase;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartTutorial()
    {
        Debug.Log("Tutorial");
        currentTutorialPhase = 0;
        signalBus.Fire<StartTutorialPhaseSignal>( new StartTutorialPhaseSignal { phase = currentTutorialPhase});
    }
    public void GoToNextPhase()
    {
        currentTutorialPhase += 1;
        signalBus.Fire<StartTutorialPhaseSignal>(new StartTutorialPhaseSignal { phase = currentTutorialPhase });
    }
    public void GoToNextPhase(int forcephase)
    {
        currentTutorialPhase = (TutorialPhase)forcephase;
        signalBus.Fire<StartTutorialPhaseSignal>(new StartTutorialPhaseSignal { phase = currentTutorialPhase });
    }
}
