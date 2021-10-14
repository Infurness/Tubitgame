using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnergyManager : MonoBehaviour
{
    [Inject] SignalBus _signalBus;

    [SerializeField] float energy;
    // Start is called before the first frame update
    void Start()
    {
        _signalBus.Subscribe<AddEnergySignal> (AddEnergy);
        energy = 100; //Dummy value
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetEnergy ()
    {
        return energy;
    }
    void AddEnergy (AddEnergySignal _signal)
    {
        energy += _signal.energyAddition;
        StopAllCoroutines ();
        StartCoroutine (UpdateEnergy ());
    }

    IEnumerator UpdateEnergy ()
    {
        _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energy });
        while (energy < 100)
        {
            energy += 3*Time.deltaTime; //Adds 3 per second
            _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energy });
            yield return null;
        }
    }
}
