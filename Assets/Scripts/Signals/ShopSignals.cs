using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;


public class OnPurchaseProductSignal
{
    public string productID;

}


public class ConfirmPendingPurchaseSignal
{
    public Product product;
}

public class ProcessPurchaseSignal
{
    public Product product;
}

public class BuyHouseSignal
{
    public string houseName;
}

public class ShopPanelOpened
{
    
}

public class OpenSCCurrenciesPanelSignal
{

}
public class OpenHCCurrenciesPanelSignal
{

}