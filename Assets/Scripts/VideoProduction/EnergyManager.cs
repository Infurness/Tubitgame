using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class EnergyManager : MonoBehaviour
{
    [Inject] SignalBus _signalBus;
    [Inject] private ExperienceManager xpManager;

    private float energy;
    [SerializeField] private QualityEnergy[] energyCostForEachQuality;
    [System.Serializable]
    struct QualityEnergy
    {
        public VideoQuality quality;
        public int energyCost;
    }
    [SerializeField] private int[] maxEnergyByLevel;
    [SerializeField] float baseRegenerationValue = 0.0018f;
    [SerializeField] float restFactorValue = 6f;
    bool isResting;
    // Start is called before the first frame update
    void Start()
    {
        isResting = false;
        _signalBus.Subscribe<AddEnergySignal> (AddEnergy);
        GetEnergyData ();
        StartChargingEnergy ();
        InvokeRepeating ("SaveEnergyData", 30, 30);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetEnergyData ()
    {
        GetUserDataRequest dataRequest = new GetUserDataRequest ();
        dataRequest.Keys = new List<string> () { };
        PlayFabClientAPI.GetUserData (dataRequest, (result =>
        {
            UserDataRecord datarecord;

            foreach (var pair in result.Data.ToList ())
            {
                print ("Data pair " + pair.Key + " " + pair.Value.Value);
            }

            if (result.Data.TryGetValue ("Energy", out datarecord))
            {
                energy = JsonConvert.DeserializeObject<ulong> (datarecord.Value);
            }
            else
            {
                energy = 0;
            }
        }), (error => { print ("Cant Retrieve User energy data"); }));
    }
    void SaveEnergyData ()
    {
        PlayerDataManager.Instance.UpdateEnergyData (energy);
    }
    public float GetEnergy ()
    {
        return energy;
    }
    public int GetMaxEnergy ()
    {
        return maxEnergyByLevel[xpManager.GetPlayerLevel () - 1];
    }
    void StartChargingEnergy ()
    {
        StopAllCoroutines ();
        StartCoroutine (UpdateEnergy ());
    }
    void AddEnergy (AddEnergySignal _signal)
    {
        energy += _signal.energyAddition;
        StartChargingEnergy ();
    }

    IEnumerator UpdateEnergy ()
    {
        while (energy < maxEnergyByLevel[xpManager.GetPlayerLevel()-1])
        {
            //Algorithm = maxEnergy / baseRegenerationValue
            //Resting algoritm = maxEnergy / (baseRegenerationValue * restFactorValue)

            float divisor = baseRegenerationValue;
            if (isResting)
                divisor *= restFactorValue;
            float secondsToFillAllTheEnergy = (GetMaxEnergy () / divisor); //Parenthesis for readability
            float energyGainPerSecond = GetMaxEnergy () / secondsToFillAllTheEnergy;
            energy += energyGainPerSecond * Time.deltaTime;
            _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energy });
            yield return null;
        }
        if (energy > maxEnergyByLevel[xpManager.GetPlayerLevel () - 1])
            energy = maxEnergyByLevel[xpManager.GetPlayerLevel () - 1];
        _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energy });
    }
    public int GetVideoEnergyCost(VideoQuality quality)
    {
        return energyCostForEachQuality.Single (x => x.quality == quality).energyCost;
    }
    public void SetPlayerIsResting (bool resting)
    {
        isResting = resting;
    }
    public bool GetPlayerIsResting ()
    {
        return isResting;
    }
    private void OnApplicationQuit ()
    {
        SaveEnergyData ();
    }
}
