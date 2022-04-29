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

    [SerializeField] private GameObject energyCoinsReturnPivot;
    [SerializeField] private GameObject SCBillsReturnPivot;
    [SerializeField] private GameObject[] energyCoinsForVideoCancel;
    [SerializeField] private GameObject[] SCBillsForVideoCancel;
    [SerializeField] private float delayBetweenCoinsMin;
    [SerializeField] private float delayBetweenCoinsMax;

    [SerializeField] private GameObject mainNightColor;
    private float mainNightColorAlpha;
    [SerializeField] private GameObject computerLight;
    private float computerLightAlpha;
    [SerializeField] private GameObject windowsLight;
    private float windowsLightAlpha;
    [SerializeField] private GameObject colorCorrection;
    private float colorCorrectionAlpha;
    [SerializeField] private GameObject sleepParticles;
    [SerializeField] private ParticleSystem changeSleepParticles;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<VFX_EnergyChangeSignal>(ChangeEnergy);
        signalBus.Subscribe<VFX_LowEnergyBlinkSignal>(ActivateLowEnergyAnim);
        signalBus.Subscribe<VFX_NoEnergyParticlesSignal>(ActivateNoEnergyParticles);
        signalBus.Subscribe<VFX_StartMovingCoinsSignal>(MoveCoins);
        signalBus.Subscribe<VFX_StartMovingSCBillsSignal>(MoveSCBills);
        signalBus.Subscribe<VFX_ActivateNightSignal>(ActivateNight);
        signalBus.Subscribe<VFX_GoToSleepSignal>(GoToSleep);
        loseEnergyObject.SetActive(false);
        getEnergyObject.SetActive(false);
        noEnergyParticlesObject.SetActive(false);
        lowEnergyAnimationObject.SetActive(false);

        if(mainNightColor!=null)
        mainNightColorAlpha = mainNightColor.GetComponent<SpriteRenderer>().color.a;
        if (computerLight != null)
            computerLightAlpha = computerLight.GetComponent<SpriteRenderer>().color.a;
        if (windowsLight != null)
            windowsLightAlpha = windowsLight.GetComponent<SpriteRenderer>().color.a;
        if (colorCorrection != null)
            colorCorrectionAlpha = colorCorrection.GetComponent<SpriteRenderer>().color.a;

        //Force day
        Color tempColor;
        if (mainNightColor != null)
        {
            tempColor = mainNightColor.GetComponent<SpriteRenderer>().color;
            tempColor.a = 0;
            mainNightColor.GetComponent<SpriteRenderer>().color = tempColor;
        }

        if (computerLight != null)
        {
            tempColor = computerLight.GetComponent<SpriteRenderer>().color;
            tempColor.a = 0;
            computerLight.GetComponent<SpriteRenderer>().color = tempColor;
        }

        if (windowsLight != null)
        {
            tempColor = windowsLight.GetComponent<SpriteRenderer>().color;
            tempColor.a = 0;
            windowsLight.GetComponent<SpriteRenderer>().color = tempColor;
        }

        if (colorCorrection != null)
        {
            tempColor = colorCorrection.GetComponent<SpriteRenderer>().color;
            tempColor.a = 0;
            colorCorrection.GetComponent<SpriteRenderer>().color = tempColor;
        }
            
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

    void MoveCoins(VFX_StartMovingCoinsSignal signal)
    {
        StartCoroutine(MoveCoinsSteps(signal.origin));
    }
    IEnumerator MoveCoinsSteps(Vector3 origin)
    {
        foreach (GameObject coin in energyCoinsForVideoCancel)
        {  
            coin.GetComponent<VFX_EnergyCoinVC>().SetCoinMovementValues(origin, energyCoinsReturnPivot.GetComponent<RectTransform>().position);
            yield return new WaitForSeconds(Random.Range(delayBetweenCoinsMin,delayBetweenCoinsMax));
        }
    }
    void MoveSCBills(VFX_StartMovingSCBillsSignal signal)
    {
        StartCoroutine(MoveSCBillsSteps(signal.origin, signal.quantity));
    }
    IEnumerator MoveSCBillsSteps(Vector3 origin, int quantity)
    {
        int i = 0;
        foreach (GameObject bill in SCBillsForVideoCancel)
        {
            if (i > quantity)
                break;
            bill.GetComponent<VFX_EnergyCoinVC>().SetCoinMovementValues(origin, SCBillsReturnPivot.GetComponent<RectTransform>().position);
            yield return new WaitForSeconds(Random.Range(delayBetweenCoinsMin, delayBetweenCoinsMax));
            i++;
        }
    }

    void ActivateNight(VFX_ActivateNightSignal signal)
    {
        if(computerLight)
            computerLight.SetActive(signal.activate);
        if (windowsLight)
            windowsLight.SetActive(signal.activate);
        if (mainNightColor)
            mainNightColor.SetActive(signal.activate);
    }
    void GoToSleep(VFX_GoToSleepSignal signal)
    {
        StopAllCoroutines();
        StartCoroutine(GoToSleepSmooth(signal.goToSleep));
        sleepParticles.SetActive(signal.goToSleep);
        if(signal.goToSleep)
        {
            sleepParticles.GetComponent<ParticleSystem>().Play();
            changeSleepParticles.Play();
        }
            
    }
    IEnumerator GoToSleepSmooth(bool goToSleep)
    {
        float nightColorAlpha = 0;
        float computerAlpha = 0;
        float windowsAlpha = 0;
        float correctionAlpha = 0;

        if (!goToSleep)
        {
            nightColorAlpha = mainNightColorAlpha;
            computerAlpha = computerLightAlpha;
            windowsAlpha = windowsLightAlpha;
            correctionAlpha = colorCorrectionAlpha;
        }
        Color tempColor;

        if(goToSleep)
        {
            while (
                nightColorAlpha < mainNightColorAlpha ||
                computerAlpha < computerLightAlpha ||
                windowsAlpha < windowsLightAlpha ||
                correctionAlpha < colorCorrectionAlpha
            )
            {
                nightColorAlpha += Time.deltaTime;
                computerAlpha += Time.deltaTime;
                windowsAlpha += Time.deltaTime;
                correctionAlpha += Time.deltaTime;

                tempColor = mainNightColor.GetComponent<SpriteRenderer>().color;
                tempColor.a = nightColorAlpha;
                mainNightColor.GetComponent<SpriteRenderer>().color = tempColor;

                tempColor = computerLight.GetComponent<SpriteRenderer>().color;
                tempColor.a = computerAlpha;
                computerLight.GetComponent<SpriteRenderer>().color = tempColor;

                tempColor = windowsLight.GetComponent<SpriteRenderer>().color;
                tempColor.a = windowsAlpha;
                windowsLight.GetComponent<SpriteRenderer>().color = tempColor;

                tempColor = colorCorrection.GetComponent<SpriteRenderer>().color;
                tempColor.a = correctionAlpha;
                colorCorrection.GetComponent<SpriteRenderer>().color = tempColor;

                if (nightColorAlpha > mainNightColorAlpha)
                    nightColorAlpha = mainNightColorAlpha;

                if (computerAlpha > computerLightAlpha)
                    computerAlpha = computerLightAlpha;

                if (windowsAlpha > windowsLightAlpha)
                    windowsAlpha = windowsLightAlpha;

                if (correctionAlpha > colorCorrectionAlpha)
                    correctionAlpha = colorCorrectionAlpha;

                yield return null;
            }
        }
        else
        {
            while (
                nightColorAlpha > 0 ||
                computerAlpha > 0 ||
                windowsAlpha > 0 ||
                correctionAlpha > 0
            )
            {
                nightColorAlpha -= Time.deltaTime;
                computerAlpha -= Time.deltaTime;
                windowsAlpha -= Time.deltaTime;
                correctionAlpha -= Time.deltaTime;

                tempColor = mainNightColor.GetComponent<SpriteRenderer>().color;
                tempColor.a = nightColorAlpha;
                mainNightColor.GetComponent<SpriteRenderer>().color = tempColor;

                tempColor = computerLight.GetComponent<SpriteRenderer>().color;
                tempColor.a = computerAlpha;
                computerLight.GetComponent<SpriteRenderer>().color = tempColor;

                tempColor = windowsLight.GetComponent<SpriteRenderer>().color;
                tempColor.a = windowsAlpha;
                windowsLight.GetComponent<SpriteRenderer>().color = tempColor;

                tempColor = colorCorrection.GetComponent<SpriteRenderer>().color;
                tempColor.a = correctionAlpha;
                colorCorrection.GetComponent<SpriteRenderer>().color = tempColor;

                if (nightColorAlpha < 0)
                    nightColorAlpha = 0;

                if (computerAlpha < 0)
                    computerAlpha = 0;

                if (windowsAlpha < 0)
                    windowsAlpha = 0;

                if (correctionAlpha < 0)
                    correctionAlpha = 0;

                yield return null;
            }
        }
        
    }
}
