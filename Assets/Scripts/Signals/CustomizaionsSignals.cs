

using System.Collections.Generic;
using Customizations;

public class OnHeadEquippedSignal
{
    public HeadItem HeadItem;
}

public class OnFaceEquippedSignal
{
    public FaceItem FaceItem;
}

public class OnTorsoEquippedSignal
{
    public TorsoItem TorsoItem;
}

public class OnLegsEquippedSignal
{
    public LegsItem LegsItem;
}

public class OnFeetEquippedSignal
{
    public FeetItem FeetItem;
}

public class OnPlayerInventoryFetchedSignal
{
    public PlayerInventoryAddressedData PlayerInventoryAddressedData;
}

public class OnPlayerEquippedItemChangedSignal
{
    public List<CustomizationItem> CustomizationItems;
}
