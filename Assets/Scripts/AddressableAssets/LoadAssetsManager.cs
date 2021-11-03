using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class LoadAssetsManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] ThemesManager themesManager;

    [SerializeField] private string _label;
    [SerializeField] ScriptableTheme cloudThemeData;
    private void Start ()
    {
        cloudThemeData = null;
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
    }
}
