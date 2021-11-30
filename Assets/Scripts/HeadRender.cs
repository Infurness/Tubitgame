using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using Zenject;

public class HeadRender : MonoBehaviour
{
    [Inject] private PlayerInventory playerInventory;

    [SerializeField] private SpriteRenderer headRender, hairRender;
    [SerializeField] private Camera headRenderCamera;
    [SerializeField] private SpriteRenderer TestResult;
    public Sprite GetHeadTexture(string head, string hair)
    {
        var headItem = (HeadItem) playerInventory.CharacterItems.Find((it) => it.name == head);
        var hairItem = (HairItem) playerInventory.CharacterItems.Find((it) => it.name == hair);
        headRender.sprite = headItem.sprite;
        hairRender.sprite = hairItem.sprite;
        var rTex = headRenderCamera.targetTexture;
        RenderTexture.active = rTex;
        Texture2D texture2D = new Texture2D(rTex.width, rTex.height);
        var sprite=Sprite.Create(texture2D, Rect.zero, Vector2.zero);
        TestResult.sprite = sprite;
        return sprite;

    }
    void Start()
    {
        GetHeadTexture("HeadItem", "HairItem");
    }

    void Update()
    {
        
    }
}
