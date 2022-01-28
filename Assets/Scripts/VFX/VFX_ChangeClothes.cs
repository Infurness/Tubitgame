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
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<ChangeClothesAnimationSignal>(StartAnimation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeClothes()
    {
        signalBus.Fire<ChangeClothesVisualSignal>();
    }
    void StartAnimation(ChangeClothesAnimationSignal signal)
    {
        oldCloth.sprite = newCloth.sprite;
        brightNewCloth.sprite = signal.newCloth;
        newCloth.sprite = signal.newCloth;
        GetComponent<Animator>().Play("Change_Cloth");
    }
}
