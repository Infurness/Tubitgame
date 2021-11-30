using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class AssetsDownloader : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] private List<string> labels;
    
    // Start is called before the first frame update
    private void Awake()
    {
        signalBus.Subscribe<OnPlayFabLoginSuccessesSignal>((signal =>
        {
            CheckAssetsUpdates();
        }));
    }

    async  void CheckAssetsUpdates()
    {
      var catalogUpdate=   Addressables.CheckForCatalogUpdates();
      await catalogUpdate.Task;
       var newAssetsCheck=  Addressables.GetDownloadSizeAsync(labels);
       await newAssetsCheck.Task;
       if (newAssetsCheck.Result>0)
       {
          var downloadOp=  Addressables.DownloadDependenciesAsync(labels,Addressables.MergeMode.Union);
          await downloadOp.Task;
          print("Download is done "+newAssetsCheck.Result/1000 +" KB" );
       }
       else
       {
           print("No update avaliable");
       }
       signalBus.Fire<RemoteAssetsCheckSignal>();

    }

   
}
