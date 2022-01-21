using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class VideoInfo_VC : MonoBehaviour
{
    private SignalBus signalBus;
    private YouTubeVideoManager youTubeVideoManager;
    private EnergyManager energyManger;
    private AdsRewardsManager adsRewardsManager;

    private bool hasBeenInitialized;

    private Video videoRef;
    private string videoName;
    private ThemeType[] themeTypes;
    private VideoQuality selectedQuality;
    private float maxInternalRecordTime;
    private float internalRecordTime;
    private DateTime createdTime;
    
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text viewsText;
    [SerializeField] private TMP_Text likesText;
    [SerializeField] private TMP_Text subscribersText;
    [SerializeField] private TMP_Text commentsText;
    [SerializeField] private GameObject subscribersPanel;
    [SerializeField] private GameObject viralVisual;
    [SerializeField] private TMP_Text qualityText;
    int energyCost;
    [SerializeField] private TMP_Text energyCostText;

    [SerializeField] GameObject[] themeHolders;

    [SerializeField] private GameObject progressBarPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject skipButtonPanel;

    [SerializeField] private Image videoProgressBar;
    [SerializeField] private TMP_Text videoProgressBarCountText;
    [SerializeField] private TMP_Text videoProgressText;

    [SerializeField] private Button cancelButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private TMP_Text skipMoneyText;
    private float skipMoney;
    [SerializeField] private Button publishButton;
    [SerializeField] private GameObject moneyButtonPanel;
    [SerializeField] private Button moneyButton;

    [SerializeField] private GameObject energyCoinsAppearOrigin;

    // Start is called before the first frame update
    void Start ()
    {
        //moneyButton.onClick.AddListener (RecollectMoney);
        if (videoRef == null)
            StartRecordingVideo ();
        else
            InitialState ();

        publishButton.onClick.AddListener (AdBeforeVideoPublish);
        cancelButton.onClick.AddListener (CancelVideo);
        moneyButton.onClick.AddListener (RecollectMoney);
        skipButton.onClick.AddListener(SkipVideoProduction);

        hasBeenInitialized = true;
    }
    private void OnEnable()
    {
        if(hasBeenInitialized && videoRef==null)
            RestartProductionBar();
    }
    // Update is called once per frame
    void Update ()
    {

    }
    void InitialState ()
    {
        CheckVirality ();
        publishButton.gameObject.SetActive (false);
        skipButtonPanel.SetActive (false);
        cancelButton.gameObject.SetActive (false);
        publishButton.gameObject.SetActive (false);
        if (moneyButtonPanel != null)
            moneyButtonPanel.SetActive (true);
        statsPanel.SetActive (true);
        subscribersPanel.SetActive (true);
        progressBarPanel.SetActive (false);
        if (videoRef != null)
            SetThemes (videoRef.themes);
    }
    public string GetVideoName ()
    {
        return videoName;
    }
    void SetThemes (ThemeType[] themesRef)
    {
        for (int i = 0; i < themeHolders.Length; i++)
        {
            if (i < themesRef.Length)
            {
                themeHolders[i].GetComponentInChildren<TMP_Text> ().text = string.Concat (Enum.GetName (themesRef[i].GetType (), themesRef[i]).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
            }
            else
            {
                themeHolders[i].transform.GetChild(0).gameObject.SetActive (false);
            }
        }
    }
    public void SetReferences (SignalBus _signalBus, YouTubeVideoManager _youTubeVideoManager, EnergyManager _energyManager, AdsRewardsManager _adsRewardsManager)
    {
        signalBus = _signalBus;
        youTubeVideoManager = _youTubeVideoManager;
        energyManger = _energyManager;
        adsRewardsManager = _adsRewardsManager;
    }
    public void SetVideoInfoUp (string _name, float recordTime, ThemeType[] videoThemes, VideoQuality quality)
    {
        videoRef = null;
        videoName = _name;
        nameText.text = videoName;
        themeTypes = videoThemes;
        selectedQuality = quality;
        maxInternalRecordTime = recordTime;
        internalRecordTime = maxInternalRecordTime;
        qualityText.text =  string.Concat (Enum.GetName (quality.GetType (), quality).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
        energyCost = energyManger.GetVideoEnergyCost(quality);
        energyCostText.text =$"{energyCost}";

        SetThemes (themeTypes);
    }
    public void SetTimeLeftToPublish (DateTime videoCreationTime)
    {
        createdTime = videoCreationTime;
        float seconds = (float)(GameClock.Instance.Now - createdTime).TotalSeconds;
        internalRecordTime = Mathf.Max(maxInternalRecordTime - seconds, 0);
    }
    public void SetVideoInfoUp(Video video)
    {
        videoRef = video;
    }
    public void UpdateVideoInfo ()
    {
        if(videoRef!=null)
        {
            videoName = videoRef.name;
            nameText.text = videoName;
            moneyText.text = $"{videoRef.videoSoftCurrency}";
            viewsText.text = $"{videoRef.views}";
            likesText.text = $"{videoRef.likes}";
            subscribersText.text = $"+{videoRef.newSubscribers}"; 
            commentsText.text = $"{videoRef.comments}";
            qualityText.text = string.Concat (Enum.GetName (videoRef.selectedQuality.GetType (), videoRef.selectedQuality).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');
        }
        else
        {
            //It is recording
        }
       
    }
    void RecollectMoney ()
    {
        moneyText.text = "0";
        signalBus.Fire<GetMoneyFromVideoSignal> (new GetMoneyFromVideoSignal () { videoName = videoName});
        
        
    } 
    void StartRecordingVideo ()
    {
        skipButtonPanel.SetActive (true);
        progressBarPanel.SetActive (true);
        statsPanel.SetActive (false);
        cancelButton.gameObject.SetActive (true);
        if (gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(FillTheRecordImage(maxInternalRecordTime, internalRecordTime));
            signalBus.Fire<VideoStartedSignal>(new VideoStartedSignal { startedVideo = youTubeVideoManager.GetUnpublishedVideoByName(videoName) });
        }  
        else
            Debug.LogError($"Cant Start coroutine of gameobject {name}, because is deactivated");
    }
    void VideoReadyToPublish ()
    {
        videoProgressBarCountText.text = $"Completed";
        videoProgressText.text = $"This video is ready!";
        videoProgressBar.fillAmount = 1;
        skipButtonPanel.SetActive (false);
        cancelButton.gameObject.SetActive (false);
        publishButton.gameObject.SetActive (true);
        youTubeVideoManager.SetIsRecording (false);
    }
    private void AdBeforeVideoPublish ()
    {
        if (youTubeVideoManager.IsPlayerResting ())
            return;
        signalBus.Fire<OnHitPublishButtonSignal>();
        if (AdsManager.Instance.IsAdLoaded () && !AdsManager.Instance.AreAdsDeactive())
        {
            signalBus.Subscribe<FinishedAdVisualitationRewardSignal> (PublishVideo);
            signalBus.Fire<OpenDoubleViewsAdSignal> ();
        }
        else
        {
            PublishVideo ();
        }
    }
    void PublishVideo ()
    {
        signalBus.TryUnsubscribe<FinishedAdVisualitationRewardSignal> (PublishVideo);       
        signalBus.Fire<PublishVideoSignal> (new PublishVideoSignal () { videoName = videoName, videoThemes = themeTypes, videoSelectedQuality = selectedQuality});
        videoRef = youTubeVideoManager.GetVideoByName (videoName);
        //InitialState (); //This line makes an android build crash when being executed after watching a reward ad
        if (videoRef.isViral)
            signalBus.Fire<OpenViralPopUpSignal>();
        UpdateVideoInfo ();
    }

    void CancelVideo ()
    {
        //signalBus.Fire<CancelVideoRecordingSignal> (new CancelVideoRecordingSignal () { name= videoName });
        //int energyLeftToSpend = (int)(energyCost *(internalRecordTime / maxInternalRecordTime)); 
        //signalBus.Fire<AddEnergySignal>(new AddEnergySignal() { energyAddition = energyLeftToSpend });
        GetComponent<Animator>().Play("Cancel_Video");
        signalBus.Fire<VFX_CancelVideoAnimationSignal>(new VFX_CancelVideoAnimationSignal
        {
            anim = GetComponent<Animator>(),
            onEndAnimation = () =>
            {
                signalBus.Fire<CancelVideoRecordingSignal>(new CancelVideoRecordingSignal() { name = videoName });
                int energyLeftToSpend = (int)(energyCost * (internalRecordTime / maxInternalRecordTime));
                signalBus.Fire<AddEnergySignal>(new AddEnergySignal() { energyAddition = energyLeftToSpend });
                Destroy(gameObject);
            }
        });
    }
    

    void CheckVirality ()
    {
        if (videoRef!=null && videoRef.isViral)
        {
            viralVisual.SetActive(true);        
        }     
    }
    void SkipVideoProduction()
    {
        //if (TutorialManager.Instance != null)
        //{
        //    youTubeVideoManager.GetUnpublishedVideoByName(videoName).createdTime = new DateTime(2000, 1, 1);
        //    createdTime = new DateTime(2000, 1, 1); //Set to the past
        //    youTubeVideoManager.UpdateUnpublishedVideos();
        //    RestartProductionBar();
        //    GetComponent<Animator>().Play("Haste_Video");
        //    StartCoroutine(AutoFillProductionBar());
        //}
        //else 
        if (TutorialManager.Instance != null || youTubeVideoManager.ConsumeHardCurrency((int)skipMoney))
        {
            youTubeVideoManager.GetUnpublishedVideoByName(videoName).createdTime = new DateTime(2000, 1, 1);
            createdTime = new DateTime(2000, 1, 1); //Set to the past
            youTubeVideoManager.UpdateUnpublishedVideos();
            //RestartProductionBar();
            GetComponent<Animator>().Play("Haste_Video");
            StartCoroutine(AutoFillProductionBar());
        }
    }
    public void RestartProductionBar ()
    {
        float seconds = (float)(GameClock.Instance.Now - createdTime).TotalSeconds;
        if(internalRecordTime>0)
            internalRecordTime = Mathf.Max (maxInternalRecordTime - seconds, 0);
        StartRecordingVideo();
    }
    IEnumerator FillTheRecordImage (float maxTime, float internalTime)
    {  
        float tACC = maxTime - internalTime;
        while (tACC < maxTime)
        {
            int secondsLeft = (int)(maxTime - tACC);
            videoProgressBarCountText.text =$"{secondsLeft}s"; //Dummy
            yield return new WaitForEndOfFrame ();
            tACC += Time.deltaTime;
            videoProgressBar.fillAmount = tACC / maxTime;
            if (TutorialManager.Instance != null)
            {
                skipMoneyText.text = $"Free";
            }
            else
            {
                if (secondsLeft < 60)
                    skipMoney = secondsLeft / 10;
                else if (secondsLeft < 300)
                    skipMoney = secondsLeft / 6;
                else if (secondsLeft < 600)
                    skipMoney = secondsLeft / 5;
                else
                    skipMoney = secondsLeft / 4;

                skipMoneyText.text = $"{skipMoney}";
            }
            
        }
        videoProgressBarCountText.text = $"{0}s";
        videoProgressBar.fillAmount = 1;
        VideoReadyToPublish ();
    }

    public void StartMovingCoins()
    {
        signalBus.Fire<VFX_StartMovingCoinsSignal>(new VFX_StartMovingCoinsSignal { origin = energyCoinsAppearOrigin.GetComponent<RectTransform>().position});
    }

    IEnumerator AutoFillProductionBar()
    {
        float currentAmount = videoProgressBar.fillAmount;
        float lerp = 0;
        videoProgressBarCountText.text = $"{0}s";
        while (lerp < 1)
        {
            lerp += Time.deltaTime * 5;
            videoProgressBar.fillAmount = Mathf.Lerp(currentAmount, 1, lerp);
            yield return null;
        }
        videoProgressBar.fillAmount = 1;
        signalBus.TryFire<VideoSkippedSignal>(); //Tutorial
        RestartProductionBar();
    }
}