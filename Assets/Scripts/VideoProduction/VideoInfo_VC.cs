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

    [SerializeField] private GameObject progressBarPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject skipButtonPanel;

    [SerializeField] private Image videoProgressBar;

    [SerializeField] private Button cancelButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button publishButton;
    [SerializeField] private Button moneyButton;
    // Start is called before the first frame update
    void Start ()
    {
        //moneyButton.onClick.AddListener (RecollectMoney);
        if(videoRef==null)
            StartRecordingVideo ();
    }

    // Update is called once per frame
    void Update ()
    {

    }
    public void SetReferences (SignalBus _signalBus, YouTubeVideoManager _youTubeVideoManager)
    {
        signalBus = _signalBus;
        youTubeVideoManager = _youTubeVideoManager;
    }
    public void SetVideoInfoUp (string _name, float recordTime, ThemeType[] videoThemes)
    {
        videoRef = null;
        videoName = _name;
        nameText.text = videoName;
        themeTypes = videoThemes;
        internalRecordTime = recordTime;
        UpdateVideoInfo ();
    }
    public void SetVideoInfoUp(Video video)
    {
        videoRef = video;
    }
    public void UpdateVideoInfo ()
    {
        if(videoRef!=null)
        {
            moneyText.text = $"{videoRef.money}$";
            viewsText.text = videoRef.views.ToString ();
            likesText.text = videoRef.likes.ToString ();
            subscribersText.text = videoRef.newSubscribers.ToString ();
            commentsText.text = videoRef.comments.ToString ();
        }
        else
        {
            //It is recording
        }
       
    }
    void RecollectMoney ()
    {
        moneyText.text = "0$";
        signalBus.Fire<GetMoneyFromVideoSignal> (new GetMoneyFromVideoSignal () { videoName = videoName});
    } 
    void StartRecordingVideo ()
    {
        skipButtonPanel.SetActive (true);
        progressBarPanel.SetActive (true);
        cancelButton.gameObject.SetActive (true);
        StartCoroutine (FillTheRecordImage (internalRecordTime));  
    }
    void VideoReadyToPublish ()
    {
        skipButtonPanel.SetActive (false);
        cancelButton.gameObject.SetActive (false);
        publishButton.gameObject.SetActive (true);
    }
    void PublishVideo ()
    {
        signalBus.Fire<PublishVideoSignal> (new PublishVideoSignal () { videoName = videoName, videoThemes = themeTypes });
    }

    IEnumerator FillTheRecordImage (float time)
    {  
        float tACC = 0;
        while (tACC < time)
        {
            yield return new WaitForEndOfFrame ();
            tACC += Time.deltaTime;
            videoProgressBar.fillAmount = tACC / time;
        }
        VideoReadyToPublish ();
    }
}