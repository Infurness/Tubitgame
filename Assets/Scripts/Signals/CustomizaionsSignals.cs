

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
public class OnPlayerRoomVideoQualityItemEquippedSignal
{
    public VideoQualityCustomizationItem VideoQualityCustomizationItem;
}