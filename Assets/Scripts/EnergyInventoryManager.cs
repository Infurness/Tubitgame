using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
public struct EnergyInventoryData
{
    public ScriptableEnergyItem[] items;
}
public class EnergyInventoryManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    PlayerDataManager playerDataManager;

    EnergyInventoryData inventoryData;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<UseEnergyItemSignal> (UseEnergyItem);

        playerDataManager = PlayerDataManager.Instance;
        inventoryData = new EnergyInventoryData { items = new ScriptableEnergyItem[0] };
        GetItemsData ();
    }

    private void SaveEnergyInventoryData ()
    {
        playerDataManager.UpdateEnergyInventoryData (inventoryData);
    }
    private void GetItemsData ()
    {
        GetUserDataRequest dataRequest = new GetUserDataRequest ();
        dataRequest.Keys = new List<string> () { };
        PlayFabClientAPI.GetUserData (dataRequest, (result =>
        {
            UserDataRecord datarecord;

            if (result.Data.TryGetValue ("EnergyItems", out datarecord))
            {
                inventoryData = JsonConvert.DeserializeObject<EnergyInventoryData> (datarecord.Value);
                Debug.Log ($"Items Gotten: { inventoryData.items.Length}");
            }
        }), (error => { print ("Cant Retrieve Energy Inventory data"); }));

    }
    public ScriptableEnergyItem[] GetEnergyItems ()
    {
        return inventoryData.items;
    }
    public void UseEnergyItem (UseEnergyItemSignal signal)
    {
        if(RemoveItem (signal.label))
            signalBus.Fire<AddEnergySignal> (new AddEnergySignal { energyAddition = signal.energy });   
    }

    public void AddItem(ScriptableEnergyItem item)
    {
        List<ScriptableEnergyItem> energyitems = GetEnergyItems ().ToList ();
        energyitems.Add (item);
        inventoryData.items = energyitems.ToArray ();
        SaveEnergyInventoryData ();
    }
    bool RemoveItem (string label)
    {
        List<ScriptableEnergyItem> energyitems = GetEnergyItems ().ToList ();
        bool exists = false;
        int index = 0;
        foreach (ScriptableEnergyItem item in energyitems)
        {
            if (item.IDLable == label)
            {
                exists = true;
                break;
            }
            index++;
        }
        if (!exists)
            return false;

        energyitems.RemoveAt (index);

        inventoryData.items = energyitems.ToArray ();

        SaveEnergyInventoryData ();

        return true;
    }
}
