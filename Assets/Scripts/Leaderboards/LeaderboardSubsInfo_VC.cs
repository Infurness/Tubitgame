using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardSubsInfo_VC : MonoBehaviour
{
    [SerializeField] private TMP_Text rankNumber;
    [SerializeField] private Image rankImage;
    [SerializeField] private TMP_Text playerLevels;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text rankTitle;
    [SerializeField] private TMP_Text subscribersNumber;

    [SerializeField] private Image profilePicture;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInfo(int rank ,Sprite rankIcon, int levels, string name, string title, ulong subscribers)
    {
        rankNumber.text = rank.ToString();
        if (rankImage != null)
            rankImage.sprite = rankIcon;
        else
            rankImage.gameObject.SetActive (false);
        playerLevels.text = levels.ToString ();
        playerName.text = name;
        rankTitle.text = title;
        subscribersNumber.text = subscribers.ToString ();
    }
}
