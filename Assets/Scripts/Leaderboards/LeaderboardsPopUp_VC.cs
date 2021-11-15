using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LeaderboardsPopUp_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private GameObject leaderboardPlayerInfoPrefab;
    [SerializeField] private GameObject leaderboardPlayerInfoHolder;

    private List<GameObject> playerInfoPanels = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<RecieveTop10Leaderboard> (UpdateSubcribersLeaderboard);
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
                playerInfo.GetComponent<LeaderboardSubsInfo_VC> ().SetInfo (rank, 1, entry.Key, "NoTitle", entry.Value);
                playerInfoPanels.Add (playerInfo);
            }
            else
            {
                playerInfoPanels[rank-1].GetComponent<LeaderboardSubsInfo_VC> ().SetInfo (rank, 1, entry.Key, "NoTitle", entry.Value);
            }
            rank++;
        }
    }
}
