using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class HeadAssets : MonoBehaviour
{
    public static HeadAssets Instance;
    private List<HeadItem> headItems;
    private List<HairItem> hairItems;
   

    private async void Awake()
    {
        Instance = this;
        
        var loadHeads = Addressables.LoadAssetsAsync<HeadItem>("character", null);
        await loadHeads.Task;
        var loadHairs = Addressables.LoadAssetsAsync<HairItem>("character", null);
        await loadHairs.Task;

        headItems = (List<HeadItem>) loadHeads.Result;
        hairItems=( List<HairItem>) loadHairs.Result;
     

    }

    public Sprite GetHeadSprite(string headName)
    {
        HeadItem headFound = headItems.Find((it) => it.name == headName);
        if (headFound == null)
        {
            Debug.LogError($"Hair style called: { headName } was not found");
            return null;
        }

        return headFound.sprite;
    }

    public Sprite GetHairSprite(string hairName)
    {
        HairItem hairFound = hairItems.Find((it) => it.name == hairName);
        if (hairFound == null)
        {
            Debug.LogError($"Hair style called: { hairName } was not found");
            return null;
        }
            
        return hairFound.sprite;
    }

 

 
}
