using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VFX_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] EnergyManager energyManager;

    [SerializeField] private GameObject loseEnergyObject;
    [SerializeField] private Image fillBarLimiterLoseEnergy;
    [SerializeField] private Image fillBarInverseLoseEnergy;

    [SerializeField] private GameObject getEnergyObject;
    [SerializeField] private Image fillBarLimiterGetEnergy;
    [SerializeField] private Image fillBarInverseGetEnergy;
    [SerializeField] private GameObject particle;

    [SerializeField] private GameObject noEnergyParticlesObject;
    [SerializeField] private GameObject lowEnergyAnimationObject;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<VFX_EnergyChangeSignal>(ChangeEnergy);
        signalBus.Subscribe<VFX_LowEnergyBlinkSignal>(ActivateLowEnergyAnim);
        signalBus.Subscribe<VFX_NoEnergyParticlesSignal>(ActivateNoEnergyParticles);
        loseEnergyObject.SetActive(false);
        getEnergyObject.SetActive(false);
        noEnergyParticlesObject.SetActive(false);
        lowEnergyAnimationObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeEnergy(VFX_EnergyChangeSignal signal)
    {
        if (signal.oldFill > signal.newFill)
            LoseEnergy(signal.oldFill, signal.newFill);
        else
            RecieveEnergy(signal.oldFill, signal.newFill);

    }
    void LoseEnergy(float oldEnergyFill, float newEnergyFill)
    {
        loseEnergyObject.SetActive(true);
        fillBarLimiterLoseEnergy.fillAmount = oldEnergyFill;
        fillBarLimiterLoseEnergy.GetComponent<RectTransform>().pivot = new Vector2(newEnergyFill, 0.5f);
        fillBarInverseLoseEnergy.fillAmount = 1 - newEnergyFill;
        StopCoroutine(WaitAnimationToFinish(loseEnergyObject, "Energy_Bar_Spend"));
        StartCoroutine(WaitAnimationToFinish(loseEnergyObject, "Energy_Bar_Spend"));
    }
    void RecieveEnergy(float oldEnergyFill, float newEnergyFill)
    {
        getEnergyObject.SetActive(true);
        fillBarLimiterGetEnergy.fillAmount = newEnergyFill;
        fillBarInverseGetEnergy.fillAmount = 1 - oldEnergyFill;
        float midFill = (newEnergyFill + oldEnergyFill) / 2;
        particle.GetComponent<RectTransform>().pivot = new Vector2(midFill, 0.5f);
        StopCoroutine(WaitAnimationToFinish(getEnergyObject, "Fill_Energy_Bar"));
        StartCoroutine(WaitAnimationToFinish(getEnergyObject, "Fill_Energy_Bar"));
    }

    IEnumerator WaitAnimationToFinish(GameObject animObject, string animationName)
    {
        Animator anim = animObject.GetComponent<Animator>();
        anim.Play(animationName);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        animObject.SetActive(false);
    }

    void ActivateLowEnergyAnim(VFX_LowEnergyBlinkSignal signal)
    {
        if (!lowEnergyAnimationObject.activeSelf)
        {
            lowEnergyAnimationObject.SetActive(true);
            lowEnergyAnimationObject.GetComponent<Animator>().Play("Energy_Blink");

            StartCoroutine(CancelLowEnergy());
        }            
    }

    IEnumerator CancelLowEnergy()
    {
        while (energyManager.GetEnergy() / energyManager.GetMaxEnergy() < 0.4f)
            yield return null;

        lowEnergyAnimationObject.SetActive(false);
    }

    void ActivateNoEnergyParticles(VFX_NoEnergyParticlesSignal signal)
    {
        if(!noEnergyParticlesObject.activeSelf)
        {
            noEnergyParticlesObject.SetActive(true);
            StartCoroutine(CancelNoEnergy());
        }
    }

    IEnumerator CancelNoEnergy()
    {
        while (energyManager.GetEnergy() <1)
            yield return null;

        noEnergyParticlesObject.SetActive(false);
    }
}
