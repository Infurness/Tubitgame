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
    [SerializeField] private Button publishButton;
    [SerializeField] private GameObject moneyButtonPanel;
    [SerializeField] private Button moneyButton;
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
        energyCostText.text =$"{energyManger.GetVideoEnergyCost (quality)}";

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
        StartCoroutine (FillTheRecordImage (maxInternalRecordTime,internalRecordTime));  
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

        if (AdsManager.Instance.IsAdLoaded ())
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

        UpdateVideoInfo ();
    }

    void CancelVideo ()
    {
        signalBus.Fire<CancelVideoRecordingSignal> (new CancelVideoRecordingSignal () { name= videoName });
        Destroy (gameObject);
    }

    void CheckVirality ()
    {
        if (videoRef!=null && videoRef.isViral)
            viralVisual.SetActive (true);
    }
    public void RestartProductionBar ()
    {
        float seconds = (float)(GameClock.Instance.Now - createdTime).TotalSeconds;
        internalRecordTime = Mathf.Max (maxInternalRecordTime - seconds, 0);
        StartRecordingVideo ();
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
        }
        VideoReadyToPublish ();
    }
}