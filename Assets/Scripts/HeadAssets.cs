using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class HeadAssets : MonoBehaviour
{
    private List<HeadItem> headItems;
    private List<HairItem> hairItems;
   

    private async void Awake()
    {
        var loadHeads = Addressables.LoadAssetsAsync<HeadItem>("character", null);
        await loadHeads.Task;
        var loadHairs = Addressables.LoadAssetsAsync<HairItem>("character", null);
        await loadHairs.Task;

        headItems = (List<HeadItem>) loadHeads.Result;
        hairItems=( List<HairItem>) loadHairs.Result;
     

    }

    public Sprite GetHeadSprite(string headName)
    {
        return headItems.Find((it) => it.name == headName).sprite;
    }

    public Sprite GetHairSprite(string hairName)
    {
        return hairItems.Find((it) => it.name == hairName).sprite;
    }

 

 
}
