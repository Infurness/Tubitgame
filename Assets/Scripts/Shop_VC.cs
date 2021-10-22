using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

public class Shop_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    
    void Start()
    {
        
    }

    public void OnBuyProductPressed(string id)
    {
        
        _signalBus.Fire<OnPurchaseProductSignal>(new OnPurchaseProductSignal()
        {
            productID = id
        });
    }
    void Update()
    {
        
    }
}
