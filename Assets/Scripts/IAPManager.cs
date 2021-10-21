using System;
using System.Collections;
using System.Collections.Generic;
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
       signalBus.Subscribe<OnPurchaseProductSignal>(PurchaseProduct);
       signalBus.Subscribe<ConfirmPendingPurchaseSignal>(ConfirmPendingPurchase);
    }

     void  ConfirmPendingPurchase(ConfirmPendingPurchaseSignal confirmPendingPurchaseSignal)
    {
        controller.ConfirmPendingPurchase(confirmPendingPurchaseSignal.product);
    }

     void PurchaseProduct(OnPurchaseProductSignal purchaseProductSignal)
    {
     controller.InitiatePurchase(purchaseProductSignal.product.definition.id);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        
        
        signalBus.Fire<ProcessPurchaseSignal>(new ProcessPurchaseSignal()
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

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {

        this.controller = controller;
        this.extensions = extensions;
        print("Purchasing init");
    }
}
