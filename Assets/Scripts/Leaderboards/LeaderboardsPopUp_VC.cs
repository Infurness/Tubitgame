using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class LeaderboardsPopUp_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private GameObject leaderboardPlayerInfoPrefab;
    [SerializeField] private GameObject leaderboardPlayerInfoHolder;
    [SerializeField] private GameObject subsLeaderboard;
    [SerializeField] private GameObject topCreatorsLeaderboard;
    [SerializeField] private Button channelsTabButton;
    [SerializeField] private Button topCreatorsTabButton;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;
    [SerializeField] private Sprite[] rankIcons;

    private List<GameObject> playerInfoPanels = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<RecieveTop10Leaderboard> (UpdateSubcribersLeaderboard);
        channelsTabButton.onClick.AddListener (() => TabSelected (true));
        topCreatorsTabButton.onClick.AddListener (() => TabSelected (false));
    }

    void TabSelected (bool channelTab)
    {
        if (channelTab)
        {
            channelsTabButton.GetComponent<Image> ().sprite = selectedSprite;
            channelsTabButton.GetComponentInChildren<TMP_Text> ().color = selectedColor;

            topCreatorsTabButton.GetComponent<Image> ().sprite = unselectedSprite;
            topCreatorsTabButton.GetComponentInChildren<TMP_Text> ().color = unselectedColor;
            subsLeaderboard.SetActive(true);
            topCreatorsLeaderboard.SetActive(false);
        }
        else
        {
            topCreatorsTabButton.GetComponent<Image> ().sprite = selectedSprite;
            topCreatorsTabButton.GetComponentInChildren<TMP_Text> ().color = selectedColor;

            channelsTabButton.GetComponent<Image> ().sprite = unselectedSprite;
            channelsTabButton.GetComponentInChildren<TMP_Text> ().color = unselectedColor;
            topCreatorsLeaderboard.SetActive(true);
            subsLeaderboard.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update ()
    {

    }
    void UpdateSubcribersLeaderboard (RecieveTop10Leaderboard signal)
    {
        Debug.Log ($"UpdateSubsLeaderboard: {signal.players.Count}");
        int rank = 1;
        foreach (KeyValuePair<string, ulong> entry in signal.players)
        {
            if (rank > playerInfoPanels.Count)
            {
                GameObject playerInfo = Instantiate (leaderboardPlayerInfoPrefab, leaderboardPlayerInfoHolder.transform);
                playerInfoPanels.Add (playerInfo);
            }
            Sprite rankIcon = rank <= rankIcons.Length ? rankIcons[rank - 1] : null;
            playerInfoPanels[rank-1].GetComponent<LeaderboardSubsInfo_VC> ().SetInfo (rank, rankIcon, 1, entry.Key, "NoTitle", entry.Value);

            rank++;
        }
    }
}
