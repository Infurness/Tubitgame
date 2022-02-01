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
    [SerializeField] private GameObject SCBillsAppearOrigin;

    [SerializeField] private ParticleSystem viewsParticles;
    [SerializeField] private ParticleSystem commentsParticles;
    [SerializeField] private ParticleSystem likesParticles;
    [SerializeField] private GameObject subsIconHolder;

   private EnergyManager energyManager;
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
        energyManager = FindObjectOfType<EnergyManager>();
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
    public void SetViralCheck()
    {
        signalBus.Subscribe<VFX_ActivateViralAnimation>(ActivateVirality);
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

        if(youTubeVideoManager.GetVideoByName(_name) != null) //IF the video that we want to record already exists, abort
        {
            CancelVideo();
        }
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

            //VFX
            ParticleSystem.EmissionModule emissionModule;
            int viewsEarned = (int)videoRef.views - int.Parse(viewsText.text);
            if (viewsEarned > 0)
            {
                emissionModule = viewsParticles.emission;
                emissionModule.rateOverTime = Mathf.Min(viewsEarned, 20);
                viewsParticles.Play();
            }
            int commentsEarned = (int)videoRef.comments - int.Parse(commentsText.text);
            if (commentsEarned > 0)
            {
                emissionModule = commentsParticles.emission;
                emissionModule.rateOverTime = Mathf.Min(commentsEarned, 20);
                commentsParticles.Play();
            }
            int likesEarned = (int)videoRef.likes - int.Parse(likesText.text);
            if (likesEarned > 0)
            {
                emissionModule = likesParticles.emission;
                emissionModule.rateOverTime = Mathf.Min(likesEarned, 20);
                likesParticles.Play();
            }
            int subsEarned = (int)videoRef.newSubscribers - int.Parse(subscribersText.text);
            if (subsEarned > 0)
            {
                subsIconHolder.GetComponent<Animator>().Play("NewSubscriber_Stack");
            }

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
        int moneyAvailable = int.Parse(moneyText.text);
        if (moneyAvailable <= 0)
            return;
        StartMovingSCBills(moneyAvailable);
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
        if (energyManager.GetPlayerIsResting())
        {
            
            signalBus.Fire(new OpenDefaultMessagePopUpSignal()
            {
                message = "Can't Publish Video While Resting"
            });
            return;
            ;
        }
      
        
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
            signalBus.Fire<OpenViralPopUpSignal>(new OpenViralPopUpSignal { videoName = videoRef.name });
        UpdateVideoInfo ();
    }

    void CancelVideo ()
    {
        if (TutorialManager.Instance != null)
            return;
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
        signalBus.Fire(new VideoStartedSignal()
        {
            startedVideo = null
        });

    }
    

    void CheckVirality ()
    {
        if (videoRef!=null && videoRef.isViral)
        {
            viralVisual.SetActive(true);
        }     
    }
    void ActivateVirality(VFX_ActivateViralAnimation signal)
    {
        if(videoName == signal.videoName)
        {
            viralVisual.SetActive(true);
            viralVisual.GetComponent<Animator>().Play("Viral_Appear");
            signalBus.TryUnsubscribe<VFX_ActivateViralAnimation>(ActivateVirality);
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
            if (TutorialManager.Instance != null) //If we are in the tutorial
            {
                if (tACC >= maxTime - 1)
                {
                    tACC = maxTime - 1;
                }
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
    void StartMovingSCBills(int quantity)
    {
        signalBus.Fire<VFX_StartMovingSCBillsSignal>(new VFX_StartMovingSCBillsSignal { origin = SCBillsAppearOrigin.GetComponent<RectTransform>().position, quantity = quantity});
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