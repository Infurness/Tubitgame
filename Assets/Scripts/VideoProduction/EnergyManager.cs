using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

[System.Serializable]
public struct EnergyData
{
    public DateTime lastTimeUpdated;
    public float energy;
}
public class EnergyManager : MonoBehaviour
{
    [Inject] SignalBus _signalBus;
    [Inject] private YouTubeVideoManager youTubeVideoManager;
    [Inject] private ExperienceManager xpManager;
    private GameClock gameClock;

    private EnergyData energyData;
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
    bool energyChargeBlocked = false;
    // Start is called before the first frame update
    void Start()
    {
        isResting = false;
        gameClock = GameClock.Instance;
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
                energyData = JsonConvert.DeserializeObject<EnergyData> (datarecord.Value);
                UpdateEnergyOnStart ();
            }
            else
            {
                energyData = new EnergyData { energy = 0, lastTimeUpdated = gameClock.Now };
            }
        }), (error => { print ("Cant Retrieve User energy data"); }));
    }
    void SaveEnergyData ()
    {
        energyData.lastTimeUpdated = gameClock.Now;
        PlayerDataManager.Instance.UpdateEnergyData (energyData);
    }
    void UpdateEnergyOnStart ()
    {
        if (!youTubeVideoManager.IsRecording ())
        {
            float energyGainPerSecond = GetEnergyGainedPerSecond ();
            float seconds = (float)(gameClock.Now - energyData.lastTimeUpdated).TotalSeconds;
            Debug.Log ($"ADDITIONAL ENERGY: {energyGainPerSecond * seconds}");
            energyData.energy += energyGainPerSecond * seconds;
        }   
    }
    public float GetEnergy ()
    {
        return energyData.energy;
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
        energyData.energy += _signal.energyAddition;
        StartChargingEnergy ();
    }

    IEnumerator UpdateEnergy ()
    {
        while (energyData.energy < maxEnergyByLevel[xpManager.GetPlayerLevel()-1])
        {
            if(!youTubeVideoManager.IsRecording()) //Dont charge energy if there is a video recording
            {
                float energyGainPerSecond = GetEnergyGainedPerSecond ();
                energyData.energy += energyGainPerSecond * Time.deltaTime;
                _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energyData.energy });
            }
            yield return null;
        }
        if (energyData.energy > maxEnergyByLevel[xpManager.GetPlayerLevel () - 1])
            energyData.energy = maxEnergyByLevel[xpManager.GetPlayerLevel () - 1];
        _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energyData.energy });
    }
    float GetEnergyGainedPerSecond ()
    {
        //Algorithm = maxEnergy / baseRegenerationValue
        //Resting algoritm = maxEnergy / (baseRegenerationValue * restFactorValue)

        float divisor = baseRegenerationValue;
        if (isResting)
            divisor *= restFactorValue;
        float secondsToFillAllTheEnergy = (GetMaxEnergy () / divisor); //Parenthesis for readability
        float energyGainPerSecond = GetMaxEnergy () / secondsToFillAllTheEnergy;

        return energyGainPerSecond;
    }
    public int GetVideoEnergyCost(VideoQuality quality)
    {
        return energyCostForEachQuality.Single (x => x.quality == quality).energyCost;
    }
    public void ChangePlayerRestingState ()
    {
        isResting = !isResting;
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
