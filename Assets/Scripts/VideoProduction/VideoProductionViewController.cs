using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VideoProductionViewController : MonoBehaviour
{
    [Inject] SignalBus _signalBus;

   

    [SerializeField] private TMP_Text selectedTheme;
    [SerializeField] private GameObject preProductionPanel, productionPanel, postProduction;
    [SerializeField] private Button recordButton;
    [SerializeField] private Image recording_image;
    public void OnSelectTheme(string themeName)
    {
        _signalBus.TryFire<SelectThemeSignal>(new SelectThemeSignal()
        {
            ThemeName = themeName
            
            
        });
        selectedTheme.text = "Theme : " + themeName;
        recordButton.interactable = true;
    }

    public void OnStartRecordingPressed()
    {
        preProductionPanel.SetActive(false);
        productionPanel.SetActive(true);
        StartCoroutine(FillTheRecordImage(10));
    }

    IEnumerator FillTheRecordImage(float time)
    {

        float tACC = 0;
        while (tACC<time)
        {
            yield return new WaitForEndOfFrame();
            tACC += Time.deltaTime;
            recording_image.fillAmount=tACC/time;
        }
        
    }

   
    void Start()
    {
        recordButton.onClick.AddListener(OnStartRecordingPressed);
    }
    

    void Update()
    {
        
    }
}
