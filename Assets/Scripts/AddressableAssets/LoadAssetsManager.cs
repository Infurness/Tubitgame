using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class LoadAssetsManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] ThemesManager themesManager;
    [Inject] EnergyManager energyManager;

    [SerializeField] private string _label;
    [SerializeField] ScriptableTheme cloudThemeData;
    [SerializeField] ScriptableEnergyItem cloudEnergyData;
    private void Start ()
    {
        cloudThemeData = null;
        cloudEnergyData = null;
        Get (_label);
    }

    private async Task Get (string label)
    {
        Addressables.ClearDependencyCacheAsync (label);
        var catalogUpdate = Addressables.CheckForCatalogUpdates ();
        await catalogUpdate.Task;
        var downloadSize = Addressables.GetDownloadSizeAsync (label);
        await downloadSize.Task;
        Debug.Log (downloadSize.Result);

        if(downloadSize.Result>0)//Update
        {
            await Addressables.DownloadDependenciesAsync (label).Task;
            var locations = await Addressables.LoadResourceLocationsAsync (label).Task;

            foreach (var location in locations)
            {
                Debug.Log (location);
                var data = Addressables.LoadAssetAsync<ScriptableTheme> (location);
                if(data.IsValid())
                {
                    await data.Task;
                    cloudThemeData = data.Result;
                }     
            }
        }
        themesManager.UpdateThemesData (cloudThemeData);
        GetEnergy ("energyItem");
    }
    private async Task GetEnergy (string label)
    {
        Addressables.ClearDependencyCacheAsync (label);
        var catalogUpdate = Addressables.CheckForCatalogUpdates ();
        await catalogUpdate.Task;
        var downloadSize = Addressables.GetDownloadSizeAsync (label);
        await downloadSize.Task;
        Debug.Log (downloadSize.Result);

        if (downloadSize.Result > 0)//Update
        {
            await Addressables.DownloadDependenciesAsync (label).Task;
            var locations = await Addressables.LoadResourceLocationsAsync (label).Task;

            foreach (var location in locations)
            {
                Debug.Log (location);
                var data = Addressables.LoadAssetAsync<ScriptableEnergyItem> (location);
                if (data.IsValid ())
                {
                    await data.Task;
                    energyManager.SetEnergyItem (data.Result);
                    cloudEnergyData = data.Result;
                }
            }
        }
    }
}
