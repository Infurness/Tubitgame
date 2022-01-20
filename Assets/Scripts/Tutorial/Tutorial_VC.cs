using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class Tutorial_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    [SerializeField] private GameObject speechBubble;
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
        speechBubble.SetActive(false);
        restButton.SetActive(false);
        customButton.SetActive(false);
        videoManagerButton.SetActive(false);
        backButtonsPanel.SetActive(false);
        hideUIButtons.SetActive(false);

        switch (signal.phase)
        {
            case 0: //OpenSettings
                ActivateAndSetSpeechBubble(new string[] { "Hello.", "It's me.", "Lets show you how all this works." });

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

    void ActivateAndSetSpeechBubble(string[] texts)
    {
        speechBubble.SetActive(true);
        StartCoroutine(SetSpeechBubble(texts));
    }
    IEnumerator SetSpeechBubble(string[] texts)
    {
        int i = 0;
        int lengthWritten = 0;
        speechBubble.GetComponentInChildren<TMP_Text>().text = "";
        while (lengthWritten< texts[i].Length)
        {
            speechBubble.GetComponentInChildren<TMP_Text>().text += texts[i].Substring(lengthWritten,1);
            lengthWritten++;
            yield return new WaitForSeconds(0.05f);
        }
        i++;
        while (i<=texts.Length)
        {
            if (Input.anyKeyDown)
            {
                if(i < texts.Length)
                {
                    speechBubble.GetComponentInChildren<TMP_Text>().text = "";
                    lengthWritten = 0;
                    while (lengthWritten < texts[i].Length)
                    {
                        speechBubble.GetComponentInChildren<TMP_Text>().text += texts[i].Substring(lengthWritten, 1);
                        lengthWritten++;
                        yield return new WaitForSeconds(0.05f);
                    }
                }
                i++;
            } 
            yield return null;
        }
        speechBubble.SetActive(false);
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
