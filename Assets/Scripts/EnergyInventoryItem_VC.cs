using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EnergyInventoryItem_VC : MonoBehaviour
{
    private SignalBus signalBus;
    private EnergyInventoryManager energyInventoryManager;

    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    private EnergyItemData itemData;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener (UseItem);
    }

    public void SetUpItem (SignalBus signalBusRef, EnergyInventoryManager energyInventoryManagerRef, EnergyItemData item)
    {
        signalBus = signalBusRef;
        itemData = item;
        energyInventoryManager = energyInventoryManagerRef;
        icon.sprite = energyInventoryManager.GetIcon (item.label);
    }
    void UseItem ()
    {
        signalBus.Fire<UseEnergyItemSignal> (new UseEnergyItemSignal { label = itemData.label, energy=itemData.energy });
        Destroy (gameObject);
    }
}
