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
public class EnergyItemData
{
    public string label;
    public int quantity;
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
            }
            else
            {
                foreach(ScriptableEnergyItem item in allEnergyItems)
                {
                    for(int i=0;i<10;i++)
                    {
                        AddItem (item.IDLable, 1);
                    }
                }     
            }
        }), (error => { print ("Cant Retrieve Energy Inventory data"); }));

    }
    public float GetItemEnergy(string IDLabel)
    {
        ScriptableEnergyItem item = allEnergyItems.Find(item => item.IDLable == IDLabel);
        float itemEnergy = 0;
        if (item)
            itemEnergy = item.energyRecover;

        return itemEnergy;
    }
    public EnergyItemData[] GetEnergyItems ()
    {
        return inventoryData.items;
    }
    public void UseEnergyItem (UseEnergyItemSignal signal)
    {
        if(RemoveItem (signal.label))
        {
            signalBus.Fire<AddEnergySignal>(new AddEnergySignal { energyAddition = GetItemEnergy(signal.label)});
        }        
    }

    public void AddItem(string ID, int quantity)
    {
        List<EnergyItemData> energyitems = GetEnergyItems ().ToList ();

        bool itemFound = false;
        foreach(EnergyItemData item in energyitems)
        {
            if (item.label == ID)
            {
                item.quantity += quantity;
                itemFound = true;
                break;
            }        
        }

        if (!itemFound)
        {
            energyitems.Add(new EnergyItemData { label = ID, quantity = quantity });
        }

        inventoryData.items = energyitems.ToArray ();
        SaveEnergyInventoryData ();
    }
    bool RemoveItem (string ID)
    {
        List<EnergyItemData> energyitems = GetEnergyItems ().ToList ();

        bool itemFound = false;
        foreach (EnergyItemData item in energyitems)
        {
            if (item.label == ID)
            {
                item.quantity -= 1;
                itemFound = true;
                break;
            }
        }

        if (itemFound)
        {
            inventoryData.items = energyitems.ToArray();
            SaveEnergyInventoryData();
        }
        return itemFound;

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
        ScriptableEnergyItem itemFound = allEnergyItems.Find ((item) => item.IDLable == itemRecieved.IDLable);
        itemFound.ObjectIcon = Resources.Load<Sprite>( "EnergySprites/"+ itemRecieved.ObjectIcon.name);
        itemFound.energyRecover = itemRecieved.energyRecover;
    }
}
