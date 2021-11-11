using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LeaderboardsPopUp_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private GameObject leaderboardPlayerInfoPrefab;
    [SerializeField] private GameObject leaderboardPlayerInfoHolder;
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<RecieveTop10Leaderboard> (CreateSubscribersLeaderBoard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateSubscribersLeaderBoard (RecieveTop10Leaderboard signal)
    {
        int rank = 1;
        foreach(KeyValuePair<string, ulong> entry in signal.players)
        {
            GameObject playerInfo = Instantiate (leaderboardPlayerInfoPrefab, leaderboardPlayerInfoHolder.transform);
            playerInfo.GetComponent<LeaderboardSubsInfo_VC> ().SetInfo (rank, 1, entry.Key, "NoTitle", entry.Value);
                rank++;
        }
    }
}
