using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VFX_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    [SerializeField] private GameObject loseEnergyObject;
    [SerializeField] private Image fillBarLimiterLoseEnergy;
    [SerializeField] private Image fillBarInverseLoseEnergy;

    [SerializeField] private GameObject getEnergyObject;
    [SerializeField] private Image fillBarLimiterGetEnergy;
    [SerializeField] private Image fillBarInverseGetEnergy;
    [SerializeField] private GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<VFX_EnergyChangeSignal>(ChangeEnergy);
        loseEnergyObject.SetActive(false);
        getEnergyObject.SetActive(false);
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
        StopAllCoroutines();
        StartCoroutine(WaitAnimationToFinish(loseEnergyObject, "Energy_Bar_Spend"));
    }
    void RecieveEnergy(float oldEnergyFill, float newEnergyFill)
    {
        getEnergyObject.SetActive(true);
        fillBarLimiterGetEnergy.fillAmount = newEnergyFill;
        fillBarInverseGetEnergy.fillAmount = 1 - oldEnergyFill;
        float midFill = (newEnergyFill + oldEnergyFill) / 2;
        particle.GetComponent<RectTransform>().pivot = new Vector2(midFill, 0.5f);
        StopAllCoroutines();
        StartCoroutine(WaitAnimationToFinish(getEnergyObject, "Fill_Energy_Bar"));
    }

    IEnumerator WaitAnimationToFinish(GameObject animObject, string animationName)
    {
        Animator anim = animObject.GetComponent<Animator>();
        anim.Play(animationName);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        animObject.SetActive(false);
    }
}
