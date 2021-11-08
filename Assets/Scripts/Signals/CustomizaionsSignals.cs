

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

public class OnPlayerRoomThemeItemEquippedSignal
{
    public ThemeCustomizationItem ThemeCustomizationItem;
}

public class TestRoomThemeItemSignal
{
    public ThemeCustomizationItem ThemeCustomizationItem;
}

public class TestRoomVideoQualityITemSignal
{
    public VideoQualityCustomizationItem VideoQualityCustomizationItem;
}
public class OnPlayerRoomVideoQualityItemsEquippedSignal
{
    public List<VideoQualityCustomizationItem> VideoQualityCustomizationItem;
}