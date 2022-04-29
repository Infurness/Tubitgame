using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VFX_ChangeClothes : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    [SerializeField] SpriteRenderer oldCloth;
    [SerializeField] SpriteRenderer brightNewCloth;
    [SerializeField] SpriteRenderer newCloth;
    [SerializeField] GameObject lightEffect;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<ChangeClothesAnimationSignal>(StartAnimation);
    }

    public void ChangeClothes()
    {
        signalBus.Fire<ChangeClothesVisualSignal>();
    }
    void StartAnimation(ChangeClothesAnimationSignal signal)
    {
        oldCloth.sprite = signal.newCloth;
        oldCloth.sortingOrder = signal.layerOrder;
        oldCloth.transform.position = signal.worldPos;
        oldCloth.transform.rotation = signal.rotation;

        brightNewCloth.sprite = signal.newCloth;
        brightNewCloth.sortingOrder = signal.layerOrder;
        brightNewCloth.transform.position = signal.worldPos;
        brightNewCloth.transform.rotation = signal.rotation;

        newCloth.sprite = signal.newCloth;
        newCloth.sortingOrder = signal.layerOrder;
        newCloth.transform.position = signal.worldPos;
        newCloth.transform.rotation = signal.rotation;

        lightEffect.transform.position = signal.worldPos;

        GetComponent<Animator>().Play("Change_Cloth");
    }
}
