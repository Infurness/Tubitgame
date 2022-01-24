using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class Tutorial_VC : MonoBehaviour
{
    [Inject] SignalBus signalBus;

    [Header("Phase 1")]
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

    [Space]
    [Header("Phase 2")]

    //Hide/Show
    [SerializeField] private GameObject shopButton;
    [SerializeField] private GameObject clothingTab;
    [SerializeField] private GameObject furnitureTab;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject customizeButton;
    [SerializeField] private GameObject playerCustomizationButton;
    [SerializeField] private GameObject roomCustomizationButton;
    [SerializeField] private GameObject levelOkButton;

    [Header("Hand positions")]
    [SerializeField] private RectTransform shopHandPosition;
    [SerializeField] private RectTransform clothingTabHandPosition;
    [SerializeField] private RectTransform furnitureTabHandPosition;
    [SerializeField] private RectTransform backButtonHandPosition;
    [SerializeField] private RectTransform customizeButtonHandPosition;
    [SerializeField] private RectTransform playerCustomizationButtonHandPosition;
    [SerializeField] private RectTransform roomCustomizationButtonHandPosition;

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
        UnityEngine.Events.UnityAction btnAction = null;
        UnityEngine.Events.UnityAction<string> fieldAction = null;
        switch (signal.phase)
        {
            case 0: //OpenSettings
                ActivateAndSetSpeechBubble(new string[] { "Hello.", "It's me.", "Lets show you how all this works.", "Okay, start by clicking on your profile picture, top left corner." });

                SendHandTo(openSettingsHandPos.position);
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(1); };
                openSettingsButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                openSettingsButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(openSettingsButton.GetComponentInChildren<Button>(), btnAction));
                break;

            case (TutorialPhase)1: //SetName
                ActivateAndSetSpeechBubble(new string[] { "This is the settings menu.", "Lets change your player name.", "Touch the current name, and then write your own." });
                SendHandTo(inputNameHandPos.position);
                TMP_InputField inputField = inputNameField.GetComponentInChildren<TMP_InputField>();
                fieldAction = (inputString) => { TutorialManager.Instance.GoToNextPhase(2); };
                inputField.onDeselect.AddListener(fieldAction);
                inputField.onSubmit.AddListener(fieldAction);
                inputField.onDeselect.AddListener((inputString) => InputFieldDeleteGoToNextPhase(inputField, fieldAction));
                inputField.onSubmit.AddListener((inputString) => InputFieldDeleteGoToNextPhase(inputField, fieldAction));

                break;
            case (TutorialPhase)2: //Confirm name
                ActivateAndSetSpeechBubble(new string[] { "Now click on the confirm button." });
                SendHandTo(confirmNameHandPos.position);
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(3); };
                confirmNameField.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                confirmNameField.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(confirmNameField.GetComponentInChildren<Button>(), btnAction));

                break;
            case (TutorialPhase)3: //Open video manager
                ActivateAndSetSpeechBubble(new string[] { "Cool", "Lets start managing your new YouChannel.", "Click on the Video Manager button to open the Video Manager." });

                signalBus.Fire<CloseSettingPanelSignal>();
                videoManagerButton.SetActive(true);

                SendHandTo(videoManagerButtonHandPos.position);
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(4); };
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(videoManagerButton.GetComponentInChildren<Button>(), btnAction));

                break;
            case (TutorialPhase)4:
                ActivateAndSetSpeechBubble(new string[] { "We are going to create your first video.", "First of all lets open the video creation window." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(5); };
                openVideoCreatorButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                openVideoCreatorButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(openVideoCreatorButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(recordVideoButtonHandPos.localPosition);
                break;
            case (TutorialPhase)5:
                ActivateAndSetSpeechBubble(new string[] { "Select the themes your video is about", "Click the + icon." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(6); };
                SetThemesButtonsNextPhase(btnAction);
                SendHandTo(themeSelectionHandPos.position);
                break;
            case (TutorialPhase)6:
                ActivateAndSetSpeechBubble(new string[] { "Now drag one of the themes to the empty boxes above it." });
                signalBus.Subscribe<DropThemeSignal>(ThemeDropped);
                SendHandTo(themeDropHandPos.position);
                break;
            case (TutorialPhase)7:
                ActivateAndSetSpeechBubble(new string[] { "Hit the confirm button to set the themes as selected." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(8); };
                confirmThemesButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                confirmThemesButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(confirmThemesButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(confirmThemesHandPos.position);
                break;
            case (TutorialPhase)8:
                ActivateAndSetSpeechBubble(new string[] { "Lets not worry about the quality of the video right now, just hit Record Video." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(9); };
                recordVideoButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                recordVideoButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(recordVideoButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(recordVideoButtonHandPos.position);
                break;
            case (TutorialPhase)9:
                ActivateAndSetSpeechBubble(new string[] { "Waiting is boring, hit the skip button.", "This time will be free." });
                signalBus.Subscribe<VideoSkippedSignal>(SkippedVideo);
                SendHandTo(skipVideoHandPos.position);
                break;
            case (TutorialPhase)10:
                ActivateAndSetSpeechBubble(new string[] { "Now that the video is done producing, lets publish it." });
                signalBus.Subscribe<OnHitPublishButtonSignal>(PublishedVideo);
                SendHandTo(publishVideoHandPos.position);
                break;
            case (TutorialPhase)11:
                ActivateAndSetSpeechBubble(new string[] { "We dont have time to see ads, so just for you this time will be free, if you want." });
                signalBus.Subscribe<OnHitConfirmAdButtonSignal>(ConfirmDoubleAd);
                SendHandTo(doubleViewsHandPos.position);
                break;
            case (TutorialPhase)12:
                ActivateAndSetSpeechBubble(new string[] { "Congratulations!!! You have uploaded your first video!", "In the mean time, why don’t we go shopping? " });
                shopButton.SetActive(true);
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(13); };
                shopButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                shopButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(shopButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(shopHandPosition.position);
                signalBus.Fire<OpenHomeScreenSignal>();
                break;
            case (TutorialPhase)13:
                ActivateAndSetSpeechBubble(new string[] { "I have been holding a free item here just for you!" });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(14); };
                clothingTab.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                clothingTab.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(clothingTab.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(clothingTabHandPosition.position);
                break;
            case (TutorialPhase)14:
                ActivateAndSetSpeechBubble(new string[] { "Another one here also, so you have an easier start." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(15); };
                furnitureTab.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                furnitureTab.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(furnitureTab.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(furnitureTabHandPosition.position);
                break;
            case (TutorialPhase)15:
                backButtonsPanel.SetActive(true);
                ActivateAndSetSpeechBubble(new string[] { "Lets get back to the room whenever you are finished." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(16); };
                signalBus.Subscribe<BackButtonClickedSignal>(() => ClickBackButton(btnAction));
                SendHandTo(backButtonHandPosition.position);
                break;
            case (TutorialPhase)16:
                ActivateAndSetSpeechBubble(new string[] { "Wonderful! I believe you will look great with the new Hair Style.", "Try opening the customization menu." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(17); };
                customButton.SetActive(true);
                customizeButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                customizeButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(customizeButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(customizeButtonHandPosition.position);
                break;
            case (TutorialPhase)17:
                ActivateAndSetSpeechBubble(new string[] { "Here you will be able to change your appearance." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(18); };
                customButton.SetActive(true);
                playerCustomizationButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                playerCustomizationButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(playerCustomizationButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(playerCustomizationButtonHandPosition.position);
                break;
            case (TutorialPhase)18:
                backButtonsPanel.SetActive(true);
                ActivateAndSetSpeechBubble(new string[] { "Lets get back to the room whenever you are finished." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(19); };
                signalBus.Subscribe<BackButtonClickedSignal>(() => ClickBackButton(btnAction));
                SendHandTo(backButtonHandPosition.position);
                break;
            case (TutorialPhase)19:
                signalBus.Fire<OpenHomeScreenSignal>();
                ActivateAndSetSpeechBubble(new string[] { "Okay, let me now show you how to customize your room.", "Lets open the customization menu again." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(20); };
                customButton.SetActive(true);
                customizeButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                customizeButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(customizeButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(customizeButtonHandPosition.position);
                break;
            case (TutorialPhase)20:
                ActivateAndSetSpeechBubble(new string[] { "Now hit this button to open the room customization." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(21); };
                customButton.SetActive(true);
                roomCustomizationButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                roomCustomizationButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(roomCustomizationButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(roomCustomizationButtonHandPosition.position);
                break;
            case (TutorialPhase)21:
                backButtonsPanel.SetActive(true);
                ActivateAndSetSpeechBubble(new string[] { "Cool!", "If you have any bought items for the room, you will find them here, ready to place them in the room.",
                                                          "If you got the free bin from before, it will be under the floor tab, since it is a item that goes on the floor.", "You will be able to place it in the room from there!",
                                                          "And once more, lets get back whenever you are ready." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(22); };
                signalBus.Subscribe<BackButtonClickedSignal>(() => ClickBackButton(btnAction));
                SendHandTo(backButtonHandPosition.position);
                break;
            case (TutorialPhase)22:
                ActivateAndSetSpeechBubble(new string[] { "Lets check how is the video going." });
                btnAction = () => { TutorialManager.Instance.GoToNextPhase(23); };
                videoManagerButton.SetActive(true);
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
                videoManagerButton.GetComponentInChildren<Button>().onClick.AddListener(() => DeleteGoToNextPhaseListener(videoManagerButton.GetComponentInChildren<Button>(), btnAction));
                SendHandTo(videoManagerButtonHandPos.position);
                break;
            case (TutorialPhase)23:
                DeactivateHand();
                ActivateAndSetSpeechBubble(new string[] { "Here you can see how your videos are performing.", "Views, likes, comments, new subscribers.", "And then claim the money that video has generated." });
                signalBus.Subscribe<SpeechEndedSignal>(ForceLevelUp);
                break;
            case (TutorialPhase)24:
                ActivateAndSetSpeechBubble(new string[] { "Yay! You have just been promoted.", "Ranking up will reward you with some currency and items, use this to keep improving!", "That is all from now, it has been a pleasure.", "See you around!" });
                levelOkButton.GetComponentInChildren<Button>().onClick.AddListener(()=>TutorialManager.Instance.GoToNextScene());
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
        while (i <= texts.Length)
        {
            if (Input.anyKeyDown || i == 0)
            {
                if (i < texts.Length)
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
                        while (secondsPassed < 0.03f && !keyDown)
                        {
                            if (Input.anyKeyDown)
                            {
                                keyDown = true;
                            }
                            secondsPassed += Time.deltaTime;
                            yield return null;
                        }
                        if (keyDown)
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
        signalBus.Fire<SpeechEndedSignal>();
    }
    void DeleteGoToNextPhaseListener(Button button)
    {
        button.onClick.RemoveListener(TutorialManager.Instance.GoToNextPhase);
    }
    void DeleteGoToNextPhaseListener(Button button, UnityEngine.Events.UnityAction btnAction)
    {
        button.onClick.RemoveListener(btnAction);
    }

    void DeactivateHand()
    {
        floatingHand.gameObject.SetActive(false);
    }
    void SendHandTo(Vector3 position)
    {
        if (!cam)
            cam = Camera.main;
        floatingHand.gameObject.SetActive(true);
        floatingHand.position = position;

        Vector2 viewportCoordinates = cam.ScreenToViewportPoint(floatingHand.localPosition);
        Debug.Log(viewportCoordinates);
        Vector2 handRotation = new Vector2(Mathf.Abs(floatingHand.localScale.x), Mathf.Abs(floatingHand.localScale.y));
        if (viewportCoordinates.x < 0)
            handRotation.x *= -1;
        if (viewportCoordinates.y > 0)
            handRotation.y *= -1;
        floatingHand.localScale = handRotation;
    }
    void InputFieldDeleteGoToNextPhase(TMP_InputField input, UnityEngine.Events.UnityAction<string> fieldAction)
    {
        input.onDeselect.RemoveListener(fieldAction);
        input.onSubmit.RemoveListener(fieldAction);
    }

    void SetThemesButtonsNextPhase(UnityEngine.Events.UnityAction btnAction)
    {
        foreach (GameObject themeButton in themeButtons)
        {
            themeButton.GetComponentInChildren<Button>().onClick.AddListener(btnAction);
            themeButton.GetComponentInChildren<Button>().onClick.AddListener(() => UnSetThemesButtonsNextPhase(btnAction));
        }

    }
    void UnSetThemesButtonsNextPhase(UnityEngine.Events.UnityAction btnAction)
    {
        foreach (GameObject themeButton in themeButtons)
        {
            DeleteGoToNextPhaseListener(themeButton.GetComponentInChildren<Button>(), btnAction);
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
    void ConfirmDoubleAd()
    {
        signalBus.TryUnsubscribe<OnHitConfirmAdButtonSignal>(ConfirmDoubleAd);
        TutorialManager.Instance.GoToNextPhase(12);

    }
    void ClickBackButton(UnityEngine.Events.UnityAction btnAction)
    {
        signalBus.TryUnsubscribe<BackButtonClickedSignal>(() => ClickBackButton(btnAction));
        StopAllCoroutines();
        StartCoroutine(WaitOneFrameToUseBackButton(btnAction));
    }
    IEnumerator WaitOneFrameToUseBackButton(UnityEngine.Events.UnityAction btnAction)
    {
        yield return null;
        btnAction.Invoke();
    }

    void ForceLevelUp()
    {
        signalBus.TryUnsubscribe<SpeechEndedSignal>(ForceLevelUp);
        StartCoroutine(WaitToLevelUp());
    }
    IEnumerator WaitToLevelUp()
    {
        yield return new WaitForSeconds(3);
        TutorialManager.Instance.Add1Level();
        TutorialManager.Instance.GoToNextPhase(24);
    }
}
