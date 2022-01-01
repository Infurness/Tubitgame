using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class IAPButton : MonoBehaviour
{
   [Inject] private IAPManager iAPManager;

   [SerializeField] private string productID;
   [SerializeField] private TMP_Text PriceText;

   private void OnEnable()
   {
      PriceText.text = iAPManager.GetPrice(productID);
   }

   public void BuyProduct()
   {
      iAPManager.PurchaseProduct(productID);
   }
}
