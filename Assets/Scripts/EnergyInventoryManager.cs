using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Zenject;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
public struct EnergyInventoryData
{
    public EnergyItemData[] items;
}
public struct EnergyItemData
{
    public string label;
    public float energy;
}
public class EnergyInventoryManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    PlayerDataManager playerDataManager;

    [SerializeField]List<ScriptableEnergyItem> allEnergyItems;
    EnergyInventoryData inventoryData;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<UseEnergyItemSignal> (UseEnergyItem);

        playerDataManager = PlayerDataManager.Instance;
        inventoryData = new EnergyInventoryData { items = new EnergyItemData[0] };
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
            else
            {
                foreach(ScriptableEnergyItem item in allEnergyItems)
                {
                    for(int i=0;i<10;i++)
                    {
                        AddItem (item);
                    }
                }     
            }
        }), (error => { print ("Cant Retrieve Energy Inventory data"); }));

    }
    public EnergyItemData[] GetEnergyItems ()
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
        List<EnergyItemData> energyitems = GetEnergyItems ().ToList ();
        EnergyItemData itemToAdd = new EnergyItemData { label = item.IDLable, energy = item.energyRecover };
        energyitems.Add (itemToAdd);
        inventoryData.items = energyitems.ToArray ();
        SaveEnergyInventoryData ();
    }
    bool RemoveItem (string label)
    {
        List<EnergyItemData> energyitems = GetEnergyItems ().ToList ();
        bool exists = false;
        int index = 0;
        foreach (EnergyItemData item in energyitems)
        {
            if (item.label == label)
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

    public Sprite GetIcon (string label)
    {
        foreach(ScriptableEnergyItem item in allEnergyItems)
        {
            if(item.IDLable == label)
            {
                return item.ObjectIcon;
            }
        }
        Debug.LogError ($"No item with label: {label}");
        return null;

    }

    public void SetEnergyItem (ScriptableEnergyItem itemRecieved)
    {
        Debug.Log ("Label: " + itemRecieved.IDLable + itemRecieved.energyRecover + itemRecieved.ObjectIcon.name);
        ScriptableEnergyItem itemFound = allEnergyItems.Find ((item) => item.IDLable == itemRecieved.IDLable);
        Debug.Log ("Found Label: " + itemFound.IDLable + itemFound.energyRecover);
        itemFound.ObjectIcon = Resources.Load<Sprite>( "EnergySprites/"+ itemRecieved.ObjectIcon.name);
        itemFound.energyRecover = itemRecieved.energyRecover;
    }
}
