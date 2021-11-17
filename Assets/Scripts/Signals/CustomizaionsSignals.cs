

using System.Collections.Generic;
using Customizations;

public class OnCharacterItemEquippedSignal
{
    public ThemeCustomizationItem ThemeCustomizationItem;
}

public class OnPlayerInventoryFetchedSignal
{
    public PlayerInventoryAddressedData PlayerInventoryAddressedData;
}

public class OnPlayerEquippedThemeItemChangedSignal
{
    public List<ThemeCustomizationItem> CustomizationItems;
}


public class TestRoomThemeItemSignal
{
    public ThemeCustomizationItem ThemeCustomizationItem;
}

public class TestRoomVideoQualityITemSignal
{
    public VideoQualityCustomizationItem VideoQualityCustomizationItem;
}

public class SaveRoomLayoutSignal
{

}

public class DiscardRoomLayoutSignal
{
    
}

public class RoomZoomStateChangedSignal
{
    public bool ZoomIn = false;
}

public class AssetsLoadedSignal
{
    
}