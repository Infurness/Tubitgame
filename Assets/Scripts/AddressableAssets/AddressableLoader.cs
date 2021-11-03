using System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class AddressableLoader
{
    public static async Task LoadAssetsByLabel(string label)
    {
        await Addressables.LoadResourceLocationsAsync (label).Task;
        MonoBehaviour.print ("load");
    }

    public static async Task GetAll (string label, IList<IResourceLocation> loadedLocations)
    {
        var unloadedLocations = await Addressables.LoadResourceLocationsAsync (label).Task;

        foreach (var location in unloadedLocations)
            loadedLocations.Add (location);
    }
}
