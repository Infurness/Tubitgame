using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class EnergyInventoryItem_VC : MonoBehaviour
{
    private SignalBus signalBus;

    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    private ScriptableEnergyItem itemData;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener (UseItem);
    }

    public void SetUpItem (SignalBus signalBusRef, ScriptableEnergyItem item)
    {
        signalBus = signalBusRef;
        itemData = item;
        //icon.sprite = item.ObjectIcon;
    }
    void UseItem ()
    {
        signalBus.Fire<UseEnergyItemSignal> (new UseEnergyItemSignal { label = itemData.IDLable, energy=itemData.energyRecover });
        Destroy (gameObject);
    }
}
