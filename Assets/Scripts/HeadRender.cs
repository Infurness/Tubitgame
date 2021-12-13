using System;
using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class HeadRender : MonoBehaviour
{
    [Inject] private PlayerInventory playerInventory;

    [SerializeField] private SpriteRenderer headRender, hairRender;
    [SerializeField] private Camera headRenderCamera;
    [SerializeField] private SpriteRenderer TestResult;

    private List<HeadItem> headItems;
    private List<HairItem> hairItems;
    public Sprite GetHeadTexture(string head, string hair)
    {
        
        var headItem = headItems.Find((it) => it.name == head);
        var hairItem = hairItems.Find((it) => it.name == hair);
        headRender.sprite = headItem.sprite;
        hairRender.sprite = hairItem.sprite;
        var rTex = headRenderCamera.targetTexture;
        RenderTexture.active = rTex;
        Texture2D texture2D = new Texture2D(rTex.width, rTex.height);
        var sprite=Sprite.Create(texture2D,Rect.zero, Vector2.zero);
        TestResult.sprite = sprite;
        return sprite;

    }

    private async void Awake()
    {
        var loadHeads = Addressables.LoadAssetsAsync<HeadItem>("character", null);
        await loadHeads.Task;
        var loadHairs = Addressables.LoadAssetsAsync<HairItem>("character", null);
        await loadHairs.Task;

        headItems = (List<HeadItem>) loadHeads.Result;
        hairItems=( List<HairItem>) loadHairs.Result;
        TestResults();
    }


    void TestResults()
    {
      var sprite=  GetHeadTexture("HeadItem", "HairItem");
      TestResult.sprite = sprite;
    }

    void Update()
    {
        
    }
}
