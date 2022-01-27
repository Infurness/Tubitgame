

using System.Collections.Generic;
using Customizations;

public class OnCharacterAvatarChanged
{
    public CharacterAvatar NewAvatar;
}

public class OnPlayerInventoryFetchedSignal
{
    public PlayerInventoryAddressedData PlayerInventoryAddressedData;
    public CharacterAvatarAddressedData CharacterAvatarAddressedData;
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

public class RemoteAssetsCheckSignal
{
    
}

public class RoomCustomizationVisibilityChanged
{
    public bool Visibility;
}
public class CharacterCustomizationVisibilityChanged
{
    public bool Visibility;
}

public class ChangeCharacterStateSignal
{
    public CharacterState state;
}

