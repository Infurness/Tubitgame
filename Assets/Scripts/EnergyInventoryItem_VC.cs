using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class EnergyInventoryItem_VC : MonoBehaviour
{
    private SignalBus signalBus;
    private EnergyInventoryManager energyInventoryManager;
    private EnergyManager energyManager;

    [SerializeField] Image selecetedFrame;
    [SerializeField] Color selectedIconColor;
    [SerializeField] Color unselectedIconColor;

    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    private EnergyItemData itemData;
    private EnergyInventory_VC myVc;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener (SelectItem);
    }

    public void SetUpItem (SignalBus signalBusRef, EnergyInventoryManager energyInventoryManagerRef, EnergyItemData item, int quantity, EnergyInventory_VC vc, EnergyManager manager)
    {
        signalBus = signalBusRef;
        itemData = item;
        energyInventoryManager = energyInventoryManagerRef;
        myVc = vc;
        energyManager = manager;
        icon.sprite = energyInventoryManager.GetIcon (item.label);
        countText.text = $"{quantity}";
        DeselectItem ();
    }
    public void SelectItem ()
    {
        Color color = selecetedFrame.color;
        color.a = 1;
        selecetedFrame.color = color;
        icon.color = selectedIconColor;
        myVc.Selectitem (this);
    }
    public void DeselectItem ()
    {
        Color color = selecetedFrame.color;
        color.a = 0;
        selecetedFrame.color = color;
        icon.color = unselectedIconColor;
    }
    public void UseItem ()
    {
        if (energyManager.GetEnergyLeft() < 1)
            return;

        signalBus.Fire<UseEnergyItemSignal> (new UseEnergyItemSignal { label = itemData.label});
        int quantity = int.Parse (countText.text);
        quantity--;
        if (quantity > 0)
            countText.text = quantity.ToString();
        else
            Destroy (gameObject);
    }
}
