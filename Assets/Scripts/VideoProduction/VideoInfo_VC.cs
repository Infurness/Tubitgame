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

    private Video videoRef;
    private string videoName;
    private ThemeType[] themeTypes;
    private float internalRecordTime;
    
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text viewsText;
    [SerializeField] private TMP_Text likesText;
    [SerializeField] private TMP_Text subscribersText;
    [SerializeField] private TMP_Text commentsText;
    [SerializeField] private GameObject subscribersPanel;
    [SerializeField] private GameObject viralVisual;
    [SerializeField] private TMP_Text qualityText;

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

        publishButton.onClick.AddListener (PublishVideo);
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
        moneyButtonPanel.SetActive (true);
        statsPanel.SetActive (true);
        subscribersPanel.SetActive (true);
        progressBarPanel.SetActive (false);
        SetThemes (videoRef.themes);
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
    public void SetReferences (SignalBus _signalBus, YouTubeVideoManager _youTubeVideoManager)
    {
        signalBus = _signalBus;
        youTubeVideoManager = _youTubeVideoManager;
    }
    public void SetVideoInfoUp (string _name, float recordTime, ThemeType[] videoThemes, VideoQuality quality)
    {
        videoRef = null;
        videoName = _name;
        nameText.text = videoName;
        themeTypes = videoThemes;
        internalRecordTime = recordTime;
        qualityText.text =  string.Concat (Enum.GetName (quality.GetType (), quality).Select (x => char.IsUpper (x) ? " " + x : x.ToString ())).TrimStart (' ');

        SetThemes (themeTypes);
    }
    public void SetVideoInfoUp(Video video)
    {
        videoRef = video;
    }
    public void UpdateVideoInfo ()
    {
        if(videoRef!=null)
        {
            moneyText.text = $"{videoRef.videoSoftCurrency}";
            viewsText.text = $"{videoRef.views}";
            likesText.text = $"{videoRef.likes}";
            subscribersText.text = $"+{videoRef.newSubscribers}"; 
            commentsText.text = $"{videoRef.comments}"; 
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
        StartCoroutine (FillTheRecordImage (internalRecordTime));  
    }
    void VideoReadyToPublish ()
    {
        videoProgressBarCountText.text = $"Completed";
        videoProgressText.text = $"This video is ready!";
        skipButtonPanel.SetActive (false);
        cancelButton.gameObject.SetActive (false);
        publishButton.gameObject.SetActive (true);
    }
    void PublishVideo ()
    {
        signalBus.Fire<PublishVideoSignal> (new PublishVideoSignal () { videoName = videoName, videoThemes = themeTypes });
        videoRef = youTubeVideoManager.GetVideoByName (videoName);
        InitialState ();

        UpdateVideoInfo ();
    }

    void CancelVideo ()
    {
        signalBus.Fire<CancelVideoRecordingSignal> (new CancelVideoRecordingSignal () { name= videoName });
        Destroy (gameObject);
    }

    void CheckVirality ()
    {
        if (videoRef.isViral)
            viralVisual.SetActive (true);
    }

    IEnumerator FillTheRecordImage (float time)
    {  
        float tACC = 0;
        while (tACC < time)
        {
            int secondsLeft = (int)(time - tACC);
            videoProgressBarCountText.text =$"0{secondsLeft}s"; //Dummy
            yield return new WaitForEndOfFrame ();
            tACC += Time.deltaTime;
            videoProgressBar.fillAmount = tACC / time;
        }
        VideoReadyToPublish ();
    }
}