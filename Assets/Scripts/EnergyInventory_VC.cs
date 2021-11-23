using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnergyInventory_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] EnergyInventoryManager energyInventoryManager;

    [SerializeField] private GameObject energyItemPrefab;
    [SerializeField] private GameObject itemsHolder;

    private List<GameObject> energyItemsSpawned = new List<GameObject>(); 
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenEnergyInventorySignal> (OpenEnergyInventory);
    }

    void OpenEnergyInventory ()
    {
        if(energyItemsSpawned.Count>0)
        {
            foreach(GameObject item in energyItemsSpawned)
            {
                Destroy (item);
            }
        }
        energyItemsSpawned.Clear ();
        ScriptableEnergyItem[] items = energyInventoryManager.GetEnergyItems ();
        Debug.Log ($"items{items.Length}");
        foreach(ScriptableEnergyItem item in items)
        {
            GameObject itemSpawned = Instantiate (energyItemPrefab, itemsHolder.transform);
            itemSpawned.GetComponent<EnergyInventoryItem_VC> ().SetUpItem (signalBus, item);
            energyItemsSpawned.Add (itemSpawned);
        }
    }
}
