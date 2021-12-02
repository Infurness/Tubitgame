using System.Collections;
using System.Collections.Generic;
using Customizations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class Shop : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] private List<AssetReferenceT<ThemeCustomizationItem>> clothes;
    [SerializeField] private List<AssetReferenceT<ThemeCustomizationItem>> furniture;
    [SerializeField] private List<AssetReferenceT<VideoQualityCustomizationItem>> equipments;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
