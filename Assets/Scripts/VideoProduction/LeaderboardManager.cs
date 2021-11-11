using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LeaderboardManager : MonoBehaviour
{
    [Inject] private PlayfabLeaderboard leaderboardBackEnd;
    // Start is called before the first frame update
    void Start()
    {
        //UpdateLeaderboard (100);
        //GetLeaderboardPosition ();
        GetBest3Leaderboard ();
        GetTop10InLeaderboard ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateLeaderboard(int subscribers)
    {
        leaderboardBackEnd.SendLeaderboard (subscribers);
    }
    void GetBest3Leaderboard ()
    {
        leaderboardBackEnd.GetBest3InLeaderboard ();
    }
    void GetTop10InLeaderboard ()
    {
        leaderboardBackEnd.GetTop10InLeaderboard ();
    }
    void GetLeaderboardPosition ()
    {
        leaderboardBackEnd.GetPlayerPositionInLeaderboard ();
    }

}
