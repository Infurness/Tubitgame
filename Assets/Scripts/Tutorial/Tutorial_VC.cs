using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class Tutorial_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    private Camera cam;
    [SerializeField] private GameObject tutorialMainPanel;
    [SerializeField] private GameObject inputBlocker;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private RectTransform floatingHand;

    [SerializeField] private GameObject openSettingsButton;
    [SerializeField] private GameObject inputNameField;
    [SerializeField] private GameObject confirmNameField;
    [SerializeField] private GameObject settingsPopUp;
    [SerializeField] private GameObject openVideoCreatorButton;
    [SerializeField] private GameObject[] themeButtons;
    [SerializeField] private GameObject confirmThemesButton; 
    [SerializeField] private GameObject recordVideoButton;
    [SerializeField] private GameObject doubleViewsButton;

    //Buttons to show/hide
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
    [SerializeField] private RectTransform themeSelectionHandPos;
    [SerializeField] private RectTransform confirmThemesHandPos;
    [SerializeField] private RectTransform themeDropHandPos;
    [SerializeField] private RectTransform skipVideoHandPos;
    [SerializeField] private RectTransform publishVideoHandPos;
    [SerializeField] private RectTransform doubleViewsHandPos;
    

    // Start is called before the first frame update
    private void Awake()
    {
        signalBus.Subscribe<StartTutorialPhaseSignal>(ActivatePhase);
        floatingHand.gameObject.SetActive(false);
    }
    void Start()
    {
        cam = Camera.main;
        tutorialMainPanel.SetActive(true);
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
        inputBlocker.SetActive(false);
        restButton.SetActive(false);
        customButton.SetActive(false);
        videoManagerButton.SetActive(false);
        backButtonsPanel.SetActive(false);
        hideUIButtons.SetActive(false);

        switch (signal.phase)
        {
            case 0: //OpenSettings
                ActivateAndSetSpeechBubble(new string[] { "Hello.", "It's me.", "Lets show you how all this works.", "Okay, start by clicking on your profile picture, top left corner." });

                SendHandTo(openSettingsHandPos.localPosition);
                openSettingsButton.GetComponentInChildren<Button>().onClick.AddListener(()=>TutorialManager.Instance.GoToNextPhase(1));
                openSettingsButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(openSettingsButton.GetComponentInChildren<Button>(), 1));
               break;

            case (TutorialPhase)1: //SetName
                ActivateAndSetSpeechBubble(new string[] { "This is the settings menu.", "Lets change your player name.", "Touch the current name, and then write your own."});
                SendHandTo(inputNameHandPos.localPosition);
                TMP_InputField inputField = inputNameField.GetComponentInChildren<TMP_InputField>();
                inputField.onDeselect.AddListener((inputString) => TutorialManager.Instance.GoToNextPhase(2));
                inputField.onSubmit.AddListener((inputString) => TutorialManager.Instance.GoToNextPhase(2));
                inputField.onDeselect.AddListener((inputString) => InputFieldDeleteGoToNextPhase(inputField));
                inputField.onSubmit.AddListener((inputString) => InputFieldDeleteGoToNextPhase(inputField));

                break;
            case (TutorialPhase)2: //Confirm name
                ActivateAndSetSpeechBubble(new string[] { "Now click on the confirm button."});
                SendHandTo(confirmNameHandPos.localPosition);
                confirmNameField.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(3));
                confirmNameField.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(confirmNameField.GetComponentInChildren<Button>(), 3));

                break;
            case (TutorialPhase)3: //Open video manager
                ActivateAndSetSpeechBubble(new string[] { "Cool", "Lets start managing your new YouChannel.", "Click on the Video Manager button to open the Video Manager." });

                signalBus.Fire<CloseSettingPanelSignal>();
                videoManagerButton.SetActive(true);

                SendHandTo(videoManagerButtonHandPos.localPosition);
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(4));
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(videoManagerButton.GetComponentInChildren<Button>(), 4));

                break;
            case (TutorialPhase)4:
                ActivateAndSetSpeechBubble(new string[] { "We are going to create your first video.", "First of all lets open the video creation window"});
                openVideoCreatorButton.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(5));
                openVideoCreatorButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(openVideoCreatorButton.GetComponentInChildren<Button>(), 5));
                SendHandTo(recordVideoButtonHandPos.localPosition);
                break;
            case (TutorialPhase)5:
                ActivateAndSetSpeechBubble(new string[] { "Select the themes your video is about", "Click the + icon" });
                SetThemesButtonsNextPhase();
                SendHandTo(themeSelectionHandPos.localPosition);
                break;
            case (TutorialPhase)6:
                ActivateAndSetSpeechBubble(new string[] { "Now drag one of the themes to the empty boxes above it"});
                signalBus.Subscribe<DropThemeSignal>(ThemeDropped);
                SendHandTo(themeDropHandPos.localPosition);
                break;
            case (TutorialPhase)7:
                ActivateAndSetSpeechBubble(new string[] { "Hit the confirm button to set the themes as selected" });
                confirmThemesButton.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(8));
                confirmThemesButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(confirmThemesButton.GetComponentInChildren<Button>(), 8));
                SendHandTo(confirmThemesHandPos.localPosition);
                break;
            case (TutorialPhase)8:
                ActivateAndSetSpeechBubble(new string[] { "Lets not worry about the quality of the video right now, just hit Record Video" });
                recordVideoButton.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(9));
                recordVideoButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(recordVideoButton.GetComponentInChildren<Button>(), 9));
                SendHandTo(recordVideoButtonHandPos.localPosition);
                break;
            case (TutorialPhase)9:
                ActivateAndSetSpeechBubble(new string[] { "Waiting is boring, hit the skip button.", "This time will be free."});
                signalBus.Subscribe<VideoSkippedSignal>(SkippedVideo);
                SendHandTo(skipVideoHandPos.localPosition);
                break;
            case (TutorialPhase)10:
                ActivateAndSetSpeechBubble(new string[] { "Now that the video is done producing, lets publish it."});
                signalBus.Subscribe<OnHitPublishButtonSignal>(PublishedVideo);
                SendHandTo(publishVideoHandPos.localPosition);
                break;
            case (TutorialPhase)11:
                ActivateAndSetSpeechBubble(new string[] { "We dont have time to see ads, so just for you this time will be free, if you want." });
                doubleViewsButton.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(12));
                doubleViewsButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(doubleViewsButton.GetComponentInChildren<Button>(), 12));
                SendHandTo(doubleViewsHandPos.localPosition);
                break;
            case (TutorialPhase)12:
                ActivateAndSetSpeechBubble(new string[] { "Congratulations!!! You have uploaded your first video!" });
                break;
        }
    }

    void ActivateAndSetSpeechBubble(string[] texts)
    {
        speechBubble.SetActive(true);
        inputBlocker.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(SetSpeechBubble(texts));
    }
    IEnumerator SetSpeechBubble(string[] texts)
    {
        int i = 0;
        int lengthWritten = 0;
        speechBubble.GetComponentInChildren<TMP_Text>().text = "";
        //while (lengthWritten< texts[i].Length)
        //{
        //    speechBubble.GetComponentInChildren<TMP_Text>().text += texts[i].Substring(lengthWritten, 1);
        //    lengthWritten++;
        //    float secondsPassed = 0;
        //    bool keyDown = false;
        //    while (secondsPassed < 0.03f && !keyDown)
        //    {
        //        if (Input.anyKeyDown)
        //        {
        //            keyDown = true;
        //        }
        //        secondsPassed += Time.deltaTime;
        //        yield return null;
        //    }
        //    if (keyDown)
        //    {
        //        speechBubble.GetComponentInChildren<TMP_Text>().text = texts[i];
        //        lengthWritten = texts[i].Length;
        //    }
        //}
        //yield return new WaitForSeconds(0.05f);
        //i++;
        while (i<=texts.Length)
        {
            if (Input.anyKeyDown ||i==0)
            {
                if(i < texts.Length)
                {
                    speechBubble.GetComponentInChildren<TMP_Text>().text = "";
                    lengthWritten = 0;
                    yield return null;
                    while (lengthWritten < texts[i].Length)
                    {
                        speechBubble.GetComponentInChildren<TMP_Text>().text += texts[i].Substring(lengthWritten, 1);
                        lengthWritten++;
                        float secondsPassed = 0;
                        bool keyDown = false;
                        while(secondsPassed < 0.03f && !keyDown)
                        {
                            if (Input.anyKeyDown)
                            {
                                keyDown = true;
                            }
                            secondsPassed += Time.deltaTime;
                            yield return null;
                        }
                        if(keyDown)
                        {
                            speechBubble.GetComponentInChildren<TMP_Text>().text = texts[i];
                            lengthWritten = texts[i].Length;
                        }
                    }
                    yield return new WaitForSeconds(0.05f);
                }
                i++;
            } 
            yield return null;
        }
        speechBubble.SetActive(false);
        inputBlocker.SetActive(false);
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
        if (!cam)
            cam = Camera.main;
        floatingHand.gameObject.SetActive(true);
        floatingHand.localPosition = position;

        Vector2 viewportCoordinates = cam.ScreenToViewportPoint(floatingHand.localPosition);
        Debug.Log(viewportCoordinates);
        Vector2 handRotation = new Vector2( Mathf.Abs( floatingHand.localScale.x), Mathf.Abs(floatingHand.localScale.y));
        if (viewportCoordinates.x < 0)
            handRotation.x *= -1;
        if (viewportCoordinates.y > 0)
            handRotation.y *= -1;
        floatingHand.localScale = handRotation;
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

    void SetThemesButtonsNextPhase()
    {
        foreach(GameObject themeButton in themeButtons)
        {
            themeButton.GetComponentInChildren<Button>().onClick.AddListener(() => TutorialManager.Instance.GoToNextPhase(6));
            themeButton.GetComponentInChildren<Button>().onClick.AddListener(() => UnSetThemesButtonsNextPhase());
        }
        
    }
    void UnSetThemesButtonsNextPhase()
    {
        foreach (GameObject themeButton in themeButtons)
        {
            DeleteGoToNextPhaseListener(themeButton.GetComponentInChildren<Button>(), 6);
        }
    }
    void ThemeDropped()
    {
        signalBus.TryUnsubscribe<DropThemeSignal>(ThemeDropped);
        TutorialManager.Instance.GoToNextPhase(7);
    }
    void SkippedVideo()
    {
        signalBus.TryUnsubscribe<VideoSkippedSignal>(SkippedVideo);
        TutorialManager.Instance.GoToNextPhase(10);
    }
    void PublishedVideo()
    {
        signalBus.TryUnsubscribe<OnHitPublishButtonSignal>(PublishedVideo);
        TutorialManager.Instance.GoToNextPhase(11);
    }
}
