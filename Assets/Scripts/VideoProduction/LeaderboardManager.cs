using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LeaderboardManager : MonoBehaviour
{
    [Inject] private PlayfabLeaderboard leaderboardBackEnd;

    public static LeaderboardManager Instance;
    // Start is called before the first frame update

    private void Awake ()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy (this);
    }
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
    public void UpdateLeaderboard(string leaderboardName, int subscribers)
    {
        leaderboardBackEnd.SendLeaderboard (leaderboardName, subscribers);
    }
    void GetBest3Leaderboard ()
    {
        leaderboardBackEnd.GetBest3InLeaderboard ();
    }
    public void GetTop10InLeaderboard ()
    {
        leaderboardBackEnd.GetTop10InLeaderboard ();
    }
    void GetLeaderboardPosition ()
    {
        leaderboardBackEnd.GetPlayerPositionInLeaderboard ();
    }

}
