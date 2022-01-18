using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;

public class SnapToNeighborhoodViewSignal
{

}
public class OpenRealEstateShopSignal
{
    public string houseName;
}

public class HouseChangedSignal
{
    public RealEstateCustomizationItem realEstateCustomizationItem;
}

public class HousePopUp
{
    public RealEstateCustomizationItem realEstateCustomizationItem;
}

public class SetCarsCanvasButtonVisibility
{
    public bool visibility = false;
}

public class EquipCarSignal
{
    public Car car;
}
