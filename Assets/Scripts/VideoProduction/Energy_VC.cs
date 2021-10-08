using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class Energy_VC : MonoBehaviour
{
    [Inject] SignalBus _signalBus;

    [SerializeField] private TMP_Text energyText;
    // Start is called before the first frame update
    void Start()
    {
        _signalBus.Subscribe<EnergyValueSignal> (SetEnergy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetEnergy ( EnergyValueSignal signal )
    {
        energyText.text = $"{signal.energy}%";
    }
}
