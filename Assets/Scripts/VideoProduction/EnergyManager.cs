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
        StartCoroutine (UpdateEnergy ());
    }

    IEnumerator UpdateEnergy ()
    {
        Debug.LogError ($"Update{energy}");
        while (energy < 100)
        {
            _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energy });
            yield return new WaitForSeconds (1);
            energy += 3; //Adds 3 per second
           
        }
    }
}
