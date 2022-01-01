using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

public class IAPManager : MonoBehaviour,IStoreListener
{
    [Inject] private SignalBus signalBus;
    private IStoreController controller;
    private IExtensionProvider extensions;
    [SerializeField] private List<string> productCatalog;
    private void Awake()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        
        foreach (var product in productCatalog)
        {
            builder.AddProduct(product, ProductType.Consumable, new IDs()
                {
                    {product, GooglePlay.Name},
                    {product,MacAppStore.Name}
                }
            );
        }
       UnityPurchasing.Initialize(this,builder);
       signalBus.Subscribe<ConfirmPendingPurchaseSignal>(ConfirmPendingPurchase);
    }



    void  ConfirmPendingPurchase(ConfirmPendingPurchaseSignal confirmPendingPurchaseSignal)
    {
        controller.ConfirmPendingPurchase(confirmPendingPurchaseSignal.product);
        
    }

    public void PurchaseProduct(string productID)
    {
     controller.InitiatePurchase(productID);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        print("Init IAP Failed " + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        print("Attempting to buy "+purchaseEvent.purchasedProduct.definition.id);
        
        signalBus.Fire(new ProcessPurchaseSignal()
        {
            product = purchaseEvent.purchasedProduct
        });

        return PurchaseProcessingResult.Pending;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        print("Purchase Failed");
        switch (reason)
        {
            case PurchaseFailureReason.PurchasingUnavailable:
                print("Product is not available");
                break;
            case PurchaseFailureReason.ExistingPurchasePending:
                print("Exiting Process pending");
                break;
            case PurchaseFailureReason.ProductUnavailable:
                print("Product is not available");
                break;
            case PurchaseFailureReason.SignatureInvalid:
                print("Invalid Signature");
                break;
            case PurchaseFailureReason.UserCancelled:
                print("User Cancelled");
                break;
            case PurchaseFailureReason.PaymentDeclined:
                print("Payment Declined");
                break;
            case PurchaseFailureReason.DuplicateTransaction:
                print("Duplicated Transaction ");
                break;
            case PurchaseFailureReason.Unknown:
                print("Purchase Failed for unknown Reason");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
        }
    }

    public void OnInitialized(IStoreController cont, IExtensionProvider ext)
    {

        this.controller = cont;
        this.extensions = ext;
        print("Purchasing init");
    }

    public string GetPrice(string productID)
    {
        var product = controller.products.all.ToList().Find((product => product.definition.id == productID));
        return product.metadata.localizedPriceString;
    }
}
