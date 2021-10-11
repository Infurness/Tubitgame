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

    private string videoName;
    [SerializeField] private Button videoButton;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text viewsText;
    [SerializeField] private TMP_Text likesText;
    [SerializeField] private TMP_Text subscribersText;
    [SerializeField] private TMP_Text commentsText;


    // Start is called before the first frame update
    void Start ()
    {
        videoButton.onClick.AddListener (RecollectMoney);
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
    public void SetVideoInfoUp (string _name)
    {
        Video video = youTubeVideoManager.GetVideoByName (_name);
        nameText.text = _name;
        videoName = _name;
        moneyText.text = $"{video.money}$";
        viewsText.text = video.views.ToString ();
        likesText.text = video.likes.ToString ();
        subscribersText.text = video.newSubscribers.ToString ();
        commentsText.text = video.comments.ToString ();
    }
    void RecollectMoney ()
    {
        moneyText.text = "0$";
        signalBus.Fire<GetMoneyFromVideo> (new GetMoneyFromVideo () { videoName = videoName});
    } 
}