




using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class RoomLayout
{
    public List<string> equippedThemeITems=new List<string>();
    public List<string> equippedVCITems=new List<string>();


    public RoomLayout(RoomLayout roomLayout)
    {
        equippedThemeITems = roomLayout.equippedThemeITems.ToList();
        equippedVCITems = roomLayout.equippedVCITems.ToList();
    }

    public RoomLayout()
    {
    }
}