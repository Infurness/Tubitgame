using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CheatsManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private ExperienceManager experienceManager;
    [Inject] private EnergyInventoryManager energyInventoryManager;

    [SerializeField] ScriptableEnergyItem energyItemToAdd;
    public void Add1Level()
    {
        int level = experienceManager.GetPlayerLevel ();
        ulong currentXp = experienceManager.GetPlayerXp ();
        ulong maxLvlXp = experienceManager.GetXpThreshold (level);
        ulong experienceToLevelUp = maxLvlXp - currentXp;

        experienceManager.AddExperiencePoints (experienceToLevelUp + 1);
    }
    public void ResetExperience ()
    {
        experienceManager.CheatResetXp ();
    }
    public void Add100Energy ()
    {
        signalBus.Fire<AddEnergySignal> (new AddEnergySignal { energyAddition = 100 });
    }
    public void AddEnergyItem ()
    {
        energyInventoryManager.AddItem (energyItemToAdd);
    }
}
