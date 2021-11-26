using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EnergyInventory_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] EnergyInventoryManager energyInventoryManager;

    [SerializeField] private GameObject energyItemPrefab;
    [SerializeField] private GameObject itemsHolder;
    [SerializeField] private Button useButton;

    private List<GameObject> energyItemsSpawned = new List<GameObject>();
    private EnergyInventoryItem_VC selectedItem;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<OpenEnergyInventorySignal> (OpenEnergyInventory);

        useButton.onClick.AddListener (UseItem);
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
        EnergyItemData[] items = energyInventoryManager.GetEnergyItems ();
        List<string> itemsAdded = new List<string> ();
        Debug.Log ($"items{items.Length}");
        EnergyInventoryItem_VC firstItemAdded = null;
        foreach (EnergyItemData item in items)
        {
            if (!itemsAdded.Contains (item.label))
            {
                itemsAdded.Add (item.label);
                int quantity = items.Where (c => c.label == item.label).Count ();
                GameObject itemSpawned = Instantiate (energyItemPrefab, itemsHolder.transform);
                if (!firstItemAdded)
                    firstItemAdded = itemSpawned.GetComponent<EnergyInventoryItem_VC> ();
                itemSpawned.GetComponent<EnergyInventoryItem_VC> ().SetUpItem (signalBus, energyInventoryManager, item, quantity, this);
                energyItemsSpawned.Add (itemSpawned);
            } 
        }
        if (firstItemAdded)
            firstItemAdded.SelectItem ();
    }
    public void Selectitem (EnergyInventoryItem_VC item) //Called from a EnergyInventoryItem_VC
    {
        if (selectedItem)
            selectedItem.DeselectItem ();
        selectedItem = item;
    }

    void UseItem ()
    {
        if (selectedItem)
        {
            selectedItem.UseItem ();
        }     
    }
}
