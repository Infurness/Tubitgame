using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public enum TutorialPhase {Phase0, Phase1, Phase2, Phase3, Phase4, Phase5, Phase6, Phase7, Phase8, Phase9, Phase10, Phase11, Phase12, Phase13 };
public class TutorialManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] private ExperienceManager experienceManager;

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
        currentTutorialPhase = (TutorialPhase)22;
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

    public void Add1Level()
    {
        int level = experienceManager.GetPlayerLevel();
        ulong currentXp = experienceManager.GetPlayerXp();
        ulong maxLvlXp = experienceManager.GetXpThreshold(level);
        ulong experienceToLevelUp = maxLvlXp - currentXp;

        experienceManager.AddExperiencePoints(experienceToLevelUp + 1);
    }
    public void GoToNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex+1);
    }
}
