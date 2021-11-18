using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Cheats_VC : MonoBehaviour
{
    [Inject] CheatsManager cheatsManager;

    [SerializeField] private Button add100EnergyButton;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Button resetXp;
    // Start is called before the first frame update
    void Start ()
    {
        add100EnergyButton.onClick.AddListener (cheatsManager.Add100Energy);
        levelUpButton.onClick.AddListener (cheatsManager.Add1Level);
        resetXp.onClick.AddListener (cheatsManager.ResetExperience);
    }
}
