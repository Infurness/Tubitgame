using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class Tutorial_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    [SerializeField] private RectTransform floatingHand;

    [SerializeField] private GameObject openSettingsButton;
    [SerializeField] private GameObject inputNameField;
    [SerializeField] private GameObject confirmNameField;
    [SerializeField] private GameObject settingsPopUp;

    [SerializeField] private GameObject[] shopButtons;
    [SerializeField] private GameObject restButton;
    [SerializeField] private GameObject customButton;
    [SerializeField] private GameObject videoManagerButton;
    [SerializeField] private GameObject backButtonsPanel;
    [SerializeField] private GameObject hideUIButtons;

    [Header("Hand positions")]
    [SerializeField] private RectTransform openSettingsHandPos;
    [SerializeField] private RectTransform inputNameHandPos;
    [SerializeField] private RectTransform confirmNameHandPos;
    [SerializeField] private RectTransform videoManagerButtonHandPos;
    [SerializeField] private RectTransform recordVideoButtonHandPos;
    // Start is called before the first frame update
    private void Awake()
    {
        signalBus.Subscribe<StartTutorialPhaseSignal>(ActivatePhase);
        floatingHand.gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ActivatePhase(StartTutorialPhaseSignal signal)
    {

        foreach (GameObject shopButton in shopButtons)
        {
            shopButton.SetActive(false);
        }
        restButton.SetActive(false);
        customButton.SetActive(false);
        videoManagerButton.SetActive(false);
        backButtonsPanel.SetActive(false);
        hideUIButtons.SetActive(false);

        switch (signal.phase)
        {
            case 0: //OpenSettings

                SendHandTo(openSettingsHandPos.localPosition);
                openSettingsButton.GetComponentInChildren<Button>().onClick.AddListener(()=>TutorialManager.Instance.GoToNextPhase(1));
                openSettingsButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(openSettingsButton.GetComponentInChildren<Button>(), 1));
               break;

            case (TutorialPhase)1: //SetName

                SendHandTo(inputNameHandPos.localPosition);
                TMP_InputField inputField = inputNameField.GetComponentInChildren<TMP_InputField>();
                inputField.onDeselect.AddListener((inputString) => TutorialManager.Instance.GoToNextPhase(2));
                inputField.onSubmit.AddListener((inputString) => TutorialManager.Instance.GoToNextPhase(2));
                inputField.onDeselect.AddListener((inputString) => InputFieldDeleteGoToNextPhase(inputField));
                inputField.onSubmit.AddListener((inputString) => InputFieldDeleteGoToNextPhase(inputField));

                break;
            case (TutorialPhase)2: //Confirm name

                SendHandTo(confirmNameHandPos.localPosition);
                confirmNameField.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(3));
                confirmNameField.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(confirmNameField.GetComponentInChildren<Button>(), 3));

                break;
            case (TutorialPhase)3: //Open video manager
                signalBus.Fire<CloseSettingPanelSignal>();
                videoManagerButton.SetActive(true);

                SendHandTo(videoManagerButtonHandPos.localPosition);
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(4));
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(videoManagerButton.GetComponentInChildren<Button>(), 4));

                break;
            case (TutorialPhase)4:
                SendHandTo(recordVideoButtonHandPos.localPosition);
                break;
        }
    }
    void DeleteGoToNextPhaseListener(Button button)
    {
        button.onClick.RemoveListener(TutorialManager.Instance.GoToNextPhase);
    }
    void DeleteGoToNextPhaseListener(Button button, int phase)
    {
        button.onClick.RemoveListener(()=> TutorialManager.Instance.GoToNextPhase(phase));
    }


    void SendHandTo(Vector3 position)
    {
        floatingHand.gameObject.SetActive(true);
        floatingHand.localPosition = position;
    }
    void InputFieldDeleteGoToNextPhase(TMP_InputField input)
    {
        input.onDeselect.RemoveListener((inputString) => TutorialManager.Instance.GoToNextPhase(2));
        input.onSubmit.RemoveListener((inputString) => TutorialManager.Instance.GoToNextPhase(2));
    }
    void InputFieldDeleteGoToNextPhase(TMP_InputField input, int phase)
    {
        input.onDeselect.RemoveListener((inputString) => TutorialManager.Instance.GoToNextPhase(phase));
        input.onSubmit.RemoveListener((inputString) => TutorialManager.Instance.GoToNextPhase(phase));
    }
}
