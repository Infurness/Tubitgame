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
    [Inject] private PlayerDataManager playerDataManager;
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
    [Inject] private IPushNotificationsManager pushNotifications;

    // Start is called before the first frame update
    void Start()
    {
        isResting = false;
        isResting = playerDataManager.GetPlayerRestState();
        _signalBus.Fire<RestStateChangedSignal>(new RestStateChangedSignal()
        {
            IsResting = isResting
        });
        gameClock = GameClock.Instance;
        _signalBus.Subscribe<AddEnergySignal> (AddEnergy);

        GetEnergyData ();
        StartChargingEnergy ();
        InvokeRepeating ("SaveEnergyData", 30, 30);
        _signalBus.Subscribe<ChangeRestStateSignal>((() =>
        {
            ChangePlayerRestingState ();

        }));
      
    }
    public void GetEnergyData ()
    {
        GetUserDataRequest dataRequest = new GetUserDataRequest ();
        dataRequest.Keys = new List<string> () { };
        PlayFabClientAPI.GetUserData (dataRequest, (result =>
        {
            UserDataRecord datarecord;


            if (result.Data.TryGetValue ("Energy", out datarecord))
            {
                energyData = JsonConvert.DeserializeObject<EnergyData> (datarecord.Value);
                UpdateEnergyOnStart ();
            }
            else
            {
                energyData = new EnergyData { energy = maxEnergyByLevel[0], lastTimeUpdated = gameClock.Now };
                SaveEnergyData();
            }
            _signalBus.Fire<HUDStartingEnergySignal>(new HUDStartingEnergySignal { energy = energyData.energy });
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
            energyData.energy += energyGainPerSecond * seconds;
        }   
    }
    public float GetEnergy ()
    {
        return energyData.energy;
    }
    public int GetMaxEnergy ()
    {
        return maxEnergyByLevel[Mathf.Max(0, xpManager.GetPlayerLevel () - 1)];
    }
    public float GetEnergyLeft()
    {
        return GetMaxEnergy() - GetEnergy();
    }
    void AddEnergy (AddEnergySignal _signal)
    {
        float previousEnergy = energyData.energy;
        energyData.energy += _signal.energyAddition;
        StartChargingEnergy ();

        _signalBus.Fire<VFX_EnergyChangeSignal>(new VFX_EnergyChangeSignal() { oldFill = previousEnergy / GetMaxEnergy(), newFill = energyData.energy / GetMaxEnergy() });
    }

    void StartChargingEnergy ()
    {
        StopAllCoroutines ();
        StartCoroutine (UpdateEnergy ());
    }
   
    IEnumerator UpdateEnergy ()
    {
        int energyMultiplier = PlayerDataManager.Instance.GetDoubleEnergyState() ? 2 : 1;
        _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energyData.energy });
        while (energyData.energy < maxEnergyByLevel[Mathf.Max (0, xpManager.GetPlayerLevel()-1)])
        {
            if(!youTubeVideoManager.IsRecording()) //Dont charge energy if there is a video recording
            {
                float energyGainPerSecond = GetEnergyGainedPerSecond ();
                energyData.energy += energyGainPerSecond * Time.deltaTime*energyMultiplier;
                _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energyData.energy });
            }
            yield return null;
        }
        if (energyData.energy > maxEnergyByLevel[Mathf.Max (0, xpManager.GetPlayerLevel () - 1)])
            energyData.energy = maxEnergyByLevel[Mathf.Max (0, xpManager.GetPlayerLevel () - 1)];
        _signalBus.Fire<EnergyValueSignal> (new EnergyValueSignal () { energy = energyData.energy });
    }
    public float GetEnergyGainedPerSecond ()
    {
        float energyGainPerSecond = GetMaxEnergy () / SecondsToFillEnergy ();

        return energyGainPerSecond;
    }
    float SecondsToFillEnergy ()
    {
        float divisor = baseRegenerationValue;
        if (isResting)
            divisor *= restFactorValue;
        float secondsToFillAllTheEnergy = (GetMaxEnergy () / divisor); //Parenthesis for readability

        return secondsToFillAllTheEnergy;
    }
    public int GetVideoEnergyCost(VideoQuality quality)
    {
        return energyCostForEachQuality.Single (x => x.quality == quality).energyCost;
    }
    public void ChangePlayerRestingState ()
    {

        if (youTubeVideoManager.IsRecording())
            return;


        isResting = !isResting;
        playerDataManager.UpdatePlayerRestState(isResting);
        if (isResting)
        {
            _signalBus.Fire(new ChangeCharacterStateSignal()
            {
                state = CharacterState.Sleeping
            });
            _signalBus.Fire(new RestStateChangedSignal()
            {
                IsResting = true
            });
        }
        else
        {
            _signalBus.Fire(new ChangeCharacterStateSignal()
            {
                state = CharacterState.Idle
            });  
            _signalBus.Fire(new RestStateChangedSignal()
            {
                IsResting = false
            });
        }
    }
    public bool GetPlayerIsResting ()
    {
        return isResting;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus)
        {
            SaveEnergyData ();
            var title = "Energy Full";
            var subtitle = "Login to create a video.";
            var text = new string[]{"Ready to create another masterpiece?", "You're ready to create another video.","You did a good job resting, it's time to get back to work again!"};
            var timeTrigger = SecondsToFillEnergy();
            var id = 1;
            pushNotifications.ScheduleNotification(title, subtitle, text[UnityEngine.Random.Range(0, text.Length)], timeTrigger, id);
            PlayerPrefs.SetInt("EnergyNotification", id);
        }
        else
        {
            if(PlayerPrefs.HasKey("EnergyNotification"))
            {
                var id = PlayerPrefs.GetInt("EnergyNotification");
                pushNotifications.UnScheduleNotification(id);
                PlayerPrefs.DeleteKey("EnergyNotification");
            }
        }
    }
}
