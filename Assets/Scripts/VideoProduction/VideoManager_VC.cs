using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Zenject;

enum VideoManagerPanels {MakeAVideo, ManageVideos }
public class VideoManager_VC : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private YouTubeVideoManager _youTubeVideoManager;
    [Inject] private ThemesManager _themesManager;
    [Inject] private EnergyManager _energyManager;
    [Inject] private AlgorithmManager algorithmManager;
    [Inject] private AdsRewardsManager adsRewardsManager;
    [Inject] private EnergyManager energyManager;
    [Inject] private ThemesManager themesManager;

    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private GameObject makeAVideoPanel;
    [SerializeField] private GameObject manageVideosPanel;
    [SerializeField] private Button makeAVideoButton;
    [SerializeField] private Button manageVideosButton;

    [SerializeField] private Button recordVideoButton;

    private string selectedVideoName;
    [SerializeField] private TMP_InputField videoNameinputField;
    [SerializeField] private Button generateVideoName;

    [SerializeField] private Button[] themeSelectionButtons;
    [SerializeField] private Image[] themeSelectionImage;

    private ThemeType lastThemePressed;
    private ThemeType[] selectedThemes;
    private VideoQuality selectedQuality;

    [SerializeField] private Transform videoInfoHolder;
    [SerializeField] private GameObject videoInfoPrefab;
    private Dictionary<string, GameObject> videosShown = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject skipRecodingPanelPopUp;
    [SerializeField] private TMP_Text skipCurrencyText;
    [SerializeField] private Button skipRecodingPopUpConfirmButton;
    [SerializeField] private Button skipRecodingPopUpCancelButton;

    [SerializeField] private TMP_Text subsText;
    [SerializeField] private TMP_Text uploadedVideosText;

    [SerializeField] private GameObject[] qualitiesTags;
    [SerializeField] private GameObject blinkerVFX;
    private float blinkerXPos;
    [SerializeField] private Slider qualitySelector;
    private float oldValue;
    private bool energyHasBeenOfferedThisSesion; //Dummy value for Vertical Slice, not for real release to the market
    [SerializeField] private Color qualitySelectedColor;
    [SerializeField] private Sprite qualitySelectedImage;
    [SerializeField] private TMP_FontAsset qualitySelectedFont;
    [SerializeField] private Color qualityNonSelectedColor;
    [SerializeField] private Sprite qualityNonSelectedImage;
    [SerializeField] private TMP_FontAsset qualityNonSelectedFont;

    [SerializeField] private TMP_Text[] graphHourTexts;
    float minGraphX;
    float maxGraphX;

    private int videoCreationEnergyCost;
    [SerializeField] private GameObject energyCostPanel;
    [SerializeField] private TMP_Text energyCostText;

    private List<VideoInfo_VC> unpublishedVideosVC = new List<VideoInfo_VC>();
    private List<GameObject> unpublishedVideosShown = new List<GameObject>();

    [SerializeField] private int videosPerPage = 5;
    [SerializeField] private Button pageLeft;
    [SerializeField] private Button pageRight;
    [SerializeField] private TMP_Text pagesCount;
    private int currentPageNumber = 1;

    [SerializeField] private GameObject themeGraphSelectionPrefab;
    [SerializeField] private GameObject themeGraphSelectionHolder;

    [Inject] private GameAnalyticsManager gameAnalyticsManager;
    // Start is called before the first frame update
    void Start ()
    {
        minGraphX = graphHourTexts[0].gameObject.GetComponent<RectTransform> ().anchoredPosition.x;
        maxGraphX = graphHourTexts[graphHourTexts.Length - 1].gameObject.GetComponent<RectTransform> ().anchoredPosition.x;

        _signalBus.Subscribe<ShowVideosStatsSignal> (OpenManageVideosPanel);
        _signalBus.Subscribe<OpenVideoManagerSignal> (InitialState);
        _signalBus.Subscribe<ConfirmThemesSignal> (SetConfirmedThemes);
        _signalBus.Subscribe<EndPublishVideoSignal> (ResetVideoCreationInfo);
        _signalBus.Subscribe<EndPublishVideoSignal>(StartSortNextFrame);
        _signalBus.Subscribe<CancelVideoRecordingSignal> (ResetVideoCreationInfo);
        _signalBus.Subscribe<CancelVideoRecordingSignal> (CancelVideoRecording);
        _signalBus.Subscribe<ChangePlayerSubsSignal> (UpdateGlobalSubsFromSignal);
        _signalBus.Subscribe<UpdateThemesGraphSignal> (SetGraphHourTexts);
        _signalBus.Subscribe<ChangeUsernameSignal> (UpdateUsername);
        _signalBus.Subscribe<VFX_CancelVideoAnimationSignal>(WaitCancelVideo);
        _signalBus.Subscribe<RecieveSkipCuantitySignal>(SetVideoSkipMoney);

        makeAVideoButton.onClick.AddListener (OpenMakeAVideoPanel);
        manageVideosButton.onClick.AddListener (OpenManageVideosPanel);
        recordVideoButton.onClick.AddListener (OnRecordButtonPressed);
        generateVideoName.onClick.AddListener (ChangeVideoName);
        skipRecodingPopUpConfirmButton.onClick.AddListener(SkipVideo);

        qualitySelector.onValueChanged.AddListener (SetQualityTag);

        foreach(Button button in themeSelectionButtons)
        {
            button.onClick.AddListener (OpenThemeSelectorPopUp);
        }
        skipRecodingPopUpCancelButton.onClick.AddListener (() =>  OpenSkipRecordginPopUp (false));
        pageLeft.onClick.AddListener(GoToPreviousVideosPage);
        pageRight.onClick.AddListener(GoToNextVideosPage);

        energyHasBeenOfferedThisSesion = false;
        blinkerXPos = blinkerVFX.GetComponent<RectTransform>().anchoredPosition.x;

        videoNameinputField.onDeselect.AddListener(OnConfirmVideoName);
        videoNameinputField.onSubmit.AddListener(OnConfirmVideoName);

        UpdateVideoList();
        StartGraphThemesSelection();
    }
    void CheckEnergyForVideo ()
    {
        if (oldValue == qualitySelector.value || energyHasBeenOfferedThisSesion)
            return;
        
        oldValue = qualitySelector.value;
        float qualityStep = 1f / Enum.GetValues (typeof (VideoQuality)).Length;
        int qualityTagIndex = (int)(qualitySelector.value / qualityStep);
        selectedQuality = (VideoQuality)qualityTagIndex + 1;
        float energycost = _energyManager.GetVideoEnergyCost (selectedQuality);
        if(energycost> energyManager.GetEnergy ())
        {
            energyHasBeenOfferedThisSesion = true;
            _signalBus.Fire<OpenEnergyAdSignal> ();
        }
    }
    void InitialState ()
    {
        OpenManageVideosPanel ();
        recordVideoButton.interactable = false;
        videoNameinputField.text = "This is the name of my video";
        energyCostPanel.SetActive (false);
        skipRecodingPanelPopUp.SetActive (false);
        SetQualityTagVisual (0);
        UpdateUsername ();
        OpenVideosPage(currentPageNumber);
    }
    void UpdateUsername ()
    {
        if (PlayerDataManager.Instance != null)
        {
            playerNameText.text = PlayerDataManager.Instance.GetPlayerName ().ToUpper ();
        }
    }
    void ResetVideoCreationInfo ()
    {
        if(selectedThemes!=null)
            Array.Clear (selectedThemes, 0, selectedThemes.Length);
        foreach (Button button in themeSelectionButtons)
        {
            button.GetComponentInChildren<TMP_Text> ().text = "";
            button.transform.GetChild(1).GetComponentInChildren<Image> ().enabled = true;
        }

        Color transparent = Color.white;
        transparent.a = 0;
        
        foreach (Image image in themeSelectionImage)
        {
            image.sprite = null;
            image.color = transparent;
        }

        recordVideoButton.interactable = false;
        videoNameinputField.text = "This is the name of my video";
        energyCostPanel.SetActive (false);
    }
    void UpdateGlobalSubsFromSignal (ChangePlayerSubsSignal signal)
    {
        subsText.text = $"{signal.subs}";
    }
    void UpdateGlobalSubs ()
    {
        subsText.text = $"{PlayerDataManager.Instance.GetSubscribers()}";
    }
    void UpdateGlobalVideos ()
    {
        uploadedVideosText.text = $"{PlayerDataManager.Instance.GetPlayerTotalVideos()}";
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp (0) && makeAVideoPanel.activeSelf)
        {
            CheckEnergyForVideo ();
        }
    }
    void OpenMakeAVideoPanel ()
    {
        if (_youTubeVideoManager.IsRecording ())
            OpenSkipRecordginPopUp (true);
        else if (!_energyManager.GetPlayerIsResting ())
        {
            OpenPanel (VideoManagerPanels.MakeAVideo);
        }
        else
        {
            _signalBus.Fire<OpenDefaultMessagePopUpSignal> (new OpenDefaultMessagePopUpSignal { message = "Video creation is no available when resting" });
        }
           
    }
    void OpenSkipRecordginPopUp (bool open)
    {
        skipRecodingPanelPopUp.SetActive (open);
        if(open == true)
        {
            _signalBus.Fire<AskForSkipCuantitySignal>();
        }
    }
    void SetVideoSkipMoney(RecieveSkipCuantitySignal signal)
    {
        skipCurrencyText.text = signal.skipMoney;
    }
    void SkipVideo()
    {
        _signalBus.Fire<SkipRecordingVideo>();
        skipRecodingPanelPopUp.SetActive(false);
    }
    void OpenManageVideosPanel ()
    {
        OpenPanel (VideoManagerPanels.ManageVideos);
        UpdateGlobalSubs ();
        UpdateGlobalVideos ();
        SetGraphHourTexts ();
    }
    void OpenPanel (VideoManagerPanels _panel)
    {
        if (_panel == VideoManagerPanels.MakeAVideo)
        {
            makeAVideoPanel.SetActive (true);
            _signalBus.Fire<OpenVideoCreationSignal> ();
            _signalBus.Fire<ChangeBackButtonSignal> (new ChangeBackButtonSignal { changeToHome = false,
                buttonAction = () =>
                {
                    OpenPanel (VideoManagerPanels.ManageVideos);
                }
            });
            ForceQualityTagSelectionSliderPosition (0);
            ResetVideoCreationInfo();
        }
        else
        {
            makeAVideoPanel.SetActive (false);
            makeAVideoButton.GetComponentInChildren<Image> ().color = Color.white;
            _signalBus.Fire<CloseVideoCreationSignal> ();
        }

        if (_panel == VideoManagerPanels.ManageVideos)
        {
            manageVideosPanel.SetActive (true);
            UpdateVideoList ();
            _signalBus.TryUnsubscribe<OnVideosStatsUpdatedSignal> (UpdateVideoList);
            _signalBus.Subscribe<OnVideosStatsUpdatedSignal> (UpdateVideoList);
            _signalBus.Fire<ChangeBackButtonSignal> (new ChangeBackButtonSignal { changeToHome = true });
        }
        else
        {
            manageVideosPanel.SetActive (false);
            _signalBus.TryUnsubscribe<OnVideosStatsUpdatedSignal> (UpdateVideoList);
            manageVideosButton.GetComponentInChildren<Image> ().color = Color.white;

        }
    }

    void OnRecordButtonPressed ()
    {
        if (_energyManager.GetEnergy() <= videoCreationEnergyCost || selectedThemes.Length == 0)
        {
                gameAnalyticsManager.SendCustomEvent("OutOfEnergy");
            return;

        }

        _signalBus.Fire<StartRecordingSignal> (new StartRecordingSignal ()
        {
            recordedThemes = selectedThemes,
            videoName = selectedVideoName
        });
        gameAnalyticsManager.SendCustomEvent("record_start",new object[]{selectedThemes,selectedQuality});
        _signalBus.Fire<AddEnergySignal> (new AddEnergySignal () { energyAddition = -videoCreationEnergyCost });
        StartRecordingVideo ();
        recordVideoButton.GetComponent<Animator>().Play("Start_Recording");
        StartCoroutine(WaitAnimToOpenVideoManager());
    }
    IEnumerator WaitAnimToOpenVideoManager()
    {
        yield return new WaitForSeconds(0.1f);
        Animator anim = recordVideoButton.GetComponent<Animator>();
        while (anim.GetCurrentAnimatorStateInfo(0).length +0.5f > anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            yield return null;
        }
        OpenManageVideosPanel();
    }
    
    void StartRecordingVideo ()
    {
        //Create video info in the video manager screen
        CreateVideoToRecord ();        
    }
    void CreateVideoToRecord ()
    {
        GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);
        string newVideoName = selectedVideoName;
        VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
        vc.SetReferences (_signalBus, _youTubeVideoManager, _energyManager, adsRewardsManager);
        float qualityNumber = (float)selectedQuality / (float)Enum.GetValues (typeof (VideoQuality)).Length * 2;
        int secondsToProduce = algorithmManager.GetVideoSecondsToBeProduced (qualityNumber, selectedThemes.Length);
        vc.SetVideoInfoUp (newVideoName,
                            secondsToProduce,
                            selectedThemes,
                            selectedQuality
                            );
        vc.SetTimeLeftToPublish (GameClock.Instance.Now);
        unpublishedVideosVC.Add (vc);
        unpublishedVideosShown.Add(videoInfoObject);
        UnpublishedVideo unpublishedVideo = new UnpublishedVideo (newVideoName,selectedThemes,selectedQuality, secondsToProduce, GameClock.Instance.Now);
        PlayerDataManager.Instance.SetUnpublishedVideo (unpublishedVideo);
        _signalBus.Fire<OpenTimeShortenAdSignal> (new OpenTimeShortenAdSignal {video = unpublishedVideo });
        OpenVideosPage(currentPageNumber);
    }
    void CreateUnpublishedVideos ()
    {
        UnpublishedVideo[] videos = PlayerDataManager.Instance.GetUnpublishedVideos ().ToArray ();
        foreach(UnpublishedVideo video in videos)
        {
            GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);          
            VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
            unpublishedVideosVC.Add (vc);
            unpublishedVideosShown.Add(videoInfoObject);
            vc.SetReferences (_signalBus, _youTubeVideoManager, _energyManager, adsRewardsManager);
            vc.SetVideoInfoUp (video.videoName,
                                video.secondsToBeProduced,
                                video.videoThemes,
                                video.videoQuality
                                );
            vc.SetTimeLeftToPublish (video.createdTime);

            if (video.GetSecondsLeftToPublish() > 0)
                _signalBus.Fire<VideoStartedSignal>(new VideoStartedSignal { startedVideo = video });
        }
    }
    void CreateVideo (Video video)
    {
        GameObject videoInfoObject = Instantiate (videoInfoPrefab, videoInfoHolder);
        videoInfoObject.transform.SetAsFirstSibling();
        VideoInfo_VC vc = videoInfoObject.GetComponent<VideoInfo_VC> ();
        vc.SetReferences (_signalBus, _youTubeVideoManager,_energyManager, adsRewardsManager);
        vc.SetVideoInfoUp (video);
        videosShown.Add (video.name, videoInfoObject);
        vc.UpdateVideoInfo ();
        vc.SetViralCheck();
    }

    void UpdateVideoList ()
    {
        Video[] playerVideos = PlayerDataManager.Instance.GetVideos ().ToArray ();
        foreach (Video video in playerVideos)
        {
            if (videosShown.ContainsKey (video.name))
            {
                videosShown[video.name].GetComponent<VideoInfo_VC> ().UpdateVideoInfo ();
            }
            else
            {
                CreateVideo (video);
            }
        }

        if (unpublishedVideosVC.Count==0)
        {
            CreateUnpublishedVideos ();             
        }
        else
        {
            List<int> indexToDelete = new List<int>();
            int index=0;
            unpublishedVideosVC.RemoveAll (item => item == null);
            foreach (VideoInfo_VC infoVC in unpublishedVideosVC)
            {
                if(videosShown.ContainsKey (infoVC.GetVideoName ()))
                {
                    indexToDelete.Add (index);
                }
                else 
                { 
                    infoVC.RestartProductionBar (); 
                }
                index++;
            }
            foreach(int videoindex in indexToDelete)
            {
                Destroy(unpublishedVideosVC[videoindex].gameObject);
            }
            unpublishedVideosVC.RemoveAll(item => item == null);
        }
    }

    void OpenThemeSelectorPopUp()
    {
        _signalBus.Fire<OpenThemeSelectorPopUpSignal> ();
    }

    void SetConfirmedThemes (ConfirmThemesSignal signal)
    {
        for(int i=0; i < themeSelectionButtons.Length; i++)
        {
            if (signal.selectedThemesSlots.ContainsKey (i))
            {
                ThemeType themeType = signal.selectedThemesSlots[i];
                themeSelectionButtons[i].GetComponentInChildren<TMP_Text>().text = string.Concat (Enum.GetName (themeType.GetType (), themeType).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
                themeSelectionButtons[i].transform.GetChild (1).GetComponentInChildren<Image> ().enabled = false;
                themeSelectionImage[i].sprite = _themesManager.GetThemeSprite(themeType);
                themeSelectionImage[i].color = Color.white;
            }
            else
            {
                themeSelectionButtons[i].GetComponentInChildren<TMP_Text> ().text = "";
                themeSelectionButtons[i].transform.GetChild (1).GetComponentInChildren<Image> ().enabled = true;
                Color transparent = Color.white;
                transparent.a = 0;
                themeSelectionImage[i].sprite = null;
                themeSelectionImage[i].color = transparent;
            }
        }
        selectedThemes = signal.selectedThemesSlots.Values.ToArray();

        if(selectedThemes.Length > 0)
        {
            recordVideoButton.interactable = true;
            energyCostPanel.SetActive (true);
            ChangeVideoName();
        } 
        else
        {
            recordVideoButton.interactable = false;
            videoNameinputField.text = "This is the name of my video";
            energyCostPanel.SetActive (false);
        }
            
    }
    void ChangeVideoName()
    {
        if (selectedThemes==null || selectedThemes.Length <= 0)
            return;

        string oldName = selectedVideoName;
        do 
        {
            selectedVideoName = _youTubeVideoManager.GetVideoNameByTheme(selectedThemes);
        } while (selectedVideoName == oldName);

        videoNameinputField.text = selectedVideoName;
    }
    void CancelVideoRecording (CancelVideoRecordingSignal signal)
    {
        videosShown.Remove (signal.name);
    }
    void ForceQualityTagSelectionSliderPosition (float value)
    {
        SetQualityTag (value);
        qualitySelector.value = value;
    }
    void SetQualityTag (float value)
    {
        float qualityStep = 1f / Enum.GetValues (typeof(VideoQuality)).Length;
        int qualityTagIndex = (int)(value / qualityStep);
        selectedQuality = (VideoQuality)qualityTagIndex+1;
        SetQualityTagVisual (qualityTagIndex);
        float blinkerSpeed = Math.Max(0.1f, qualitySelector.value * 2);
        blinkerVFX.GetComponent<Animator>().speed = blinkerSpeed;
        videoCreationEnergyCost = _energyManager.GetVideoEnergyCost (selectedQuality);
        energyCostText.text = videoCreationEnergyCost.ToString();
    }
    void SetQualityTagVisual (int index)
    {
        int i = 0;
        foreach(GameObject qualityTag in qualitiesTags)
        {
            if(i == index)
            {
                qualityTag.GetComponentInChildren<Image> ().sprite = qualitySelectedImage;
                qualityTag.GetComponentInChildren<TMP_Text> ().color = qualitySelectedColor;
                qualityTag.GetComponentInChildren<TMP_Text> ().font = qualitySelectedFont;
                blinkerVFX.transform.parent = qualityTag.transform;
                Vector3 pos = blinkerVFX.GetComponent<RectTransform>().anchoredPosition;
                pos.x = blinkerXPos;
                blinkerVFX.GetComponent<RectTransform>().anchoredPosition = pos;
            }       
            else
            {
                if (qualityTag.GetComponentInChildren<Image>().sprite.name == "BG_theme") //Dummy: until implementation
                    continue;
                qualityTag.GetComponentInChildren<Image> ().sprite = qualityNonSelectedImage;
                qualityTag.GetComponentInChildren<TMP_Text> ().color = qualityNonSelectedColor;
                qualityTag.GetComponentInChildren<TMP_Text> ().font = qualityNonSelectedFont;
            }
            i++;
        }
    }

    void SetGraphHourTexts ()
    {
        int hourPeriod = Math.DivRem (GameClock.Instance.Now.Hour, 6, out int remaindesr);
        int i = 0;
        
        int hour = GameClock.Instance.Now.Hour % 6;
        float minutes = GameClock.Instance.Now.Minute * 100f / 60f;
        float timeNowDecimals = (hour + (minutes / 100));

        float maxDistance = maxGraphX - minGraphX;
        float distanceBetweenHours = (maxDistance * (1 / timeNowDecimals));

        foreach (TMP_Text text in graphHourTexts)
        {
            //Change hour
            text.text = $"{i + hourPeriod*6}:00";
            //Change position
            
            if (i > 0)
            {
                RectTransform rectTransform = text.gameObject.GetComponent<RectTransform> ();
                Vector3 newPos = rectTransform.anchoredPosition;
                newPos.x = minGraphX + (distanceBetweenHours * i);
                rectTransform.anchoredPosition = newPos;
            }
            i++;
        }
    }

    void GoToNextVideosPage()
    {
        OpenVideosPage(currentPageNumber + 1);
    }
    void GoToPreviousVideosPage()
    {
        OpenVideosPage(currentPageNumber - 1);
    }
    void OpenVideosPage(int pageNumber)
    {
        if (pageNumber < 1)
            return;
        unpublishedVideosShown.RemoveAll(video => video == null);
        int numberOfTotalVideos = videosShown.Count + unpublishedVideosShown.Count;
        if (numberOfTotalVideos <= 0)
        {
            pageRight.transform.parent.gameObject.SetActive(false);
            pageLeft.transform.parent.gameObject.SetActive(false);
            pagesCount.text = "1/1";
            return;
        }
        float maxPageFloat = (float)numberOfTotalVideos / (float)videosPerPage;
        if (maxPageFloat % 1 != 0)
            maxPageFloat += 1;
        int maxPage = (int)maxPageFloat;
        
        if (pageNumber > maxPage)
            return;

        
        SortVideos(pageNumber);

        currentPageNumber = pageNumber;
        pagesCount.text = $"{currentPageNumber}/{maxPage}";

        bool canGoRight = currentPageNumber < maxPage;
        bool canGoLeft = currentPageNumber > 1;
        pageRight.transform.parent.gameObject.SetActive(canGoRight);
        pageLeft.transform.parent.gameObject.SetActive(canGoLeft);
    }

    void SortVideos(int pageNumber)
    {
        int startShownIndex = videosPerPage * (pageNumber - 1);
        List<GameObject> videoObjects = videosShown.Values.ToList();
        videoObjects.AddRange(unpublishedVideosShown);
        videoObjects.Reverse();
        videoObjects.RemoveAll(video => video == null);
        for (int i = videoObjects.Count - 1; i >= 0; i--)
        {
            if (i < startShownIndex || i > (startShownIndex + videosPerPage) - 1)
            {
                if (videoObjects[i].activeSelf)
                {
                    videoObjects[i].SetActive(false);
                }
            }
            else
            {
                if (!videoObjects[i].activeSelf)
                {
                    videoObjects[i].SetActive(true);
                }
            }
            videoObjects[i].transform.SetSiblingIndex(i);
        }
    }
    void StartSortNextFrame()
    {
        StartCoroutine(SortNextFrame());
    }
    IEnumerator SortNextFrame()
    {
        yield return null;
        SortVideos(currentPageNumber);
    }

    void StartGraphThemesSelection()
    {
        foreach (ThemeType themeType in themesManager.GetThemes())
        {
            GameObject themeSelector = Instantiate(themeGraphSelectionPrefab, themeGraphSelectionHolder.transform);
            themeSelector.GetComponentInChildren<TMP_Text>().text = string.Concat(Enum.GetName(themeType.GetType(), themeType).Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            themeSelector.GetComponentInChildren<Button>().onClick.AddListener(()=> _signalBus.Fire<SelectThemeInGraphSignal>(new SelectThemeInGraphSignal { themeType = themeType }));
            themeSelector.GetComponent<ThemeGraphSelectorButton_VC>().SetUpReferences(_signalBus, themeType);
        }
    }

    void WaitCancelVideo(VFX_CancelVideoAnimationSignal signal)
    {
        StartCoroutine(CancelVideoRoutine(signal));
    }
    IEnumerator CancelVideoRoutine(VFX_CancelVideoAnimationSignal signal)
    {
        yield return new WaitForSeconds(0.5f);

        while (signal.anim.GetCurrentAnimatorStateInfo(0).length > signal.anim.GetCurrentAnimatorStateInfo(0).normalizedTime /*&& signal.anim.gameObject.activeSelf*/) //Why id first statement if false and second is true, it does enter loop anyway????!!!! Is this a joke?
        {
            if (!signal.anim.gameObject.activeSelf)
                break;
            yield return null;
        }
        signal.onEndAnimation.Invoke();
    }
    void OnConfirmVideoName(string value)
    {
        selectedVideoName = $"{value}";
        int videoNumber = _youTubeVideoManager.GetNumberOfVideoByName(value);
        if (videoNumber > 0)
            selectedVideoName += $" {videoNumber}";
    }
}
