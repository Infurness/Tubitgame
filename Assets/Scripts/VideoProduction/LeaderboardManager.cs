using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LeaderboardManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlayfabLeaderboard leaderboardBackEnd;
    // Start is called before the first frame update
    void Start()
    {
        //UpdateLeaderboard (100);
        signalBus.Subscribe<RecievePlayerLeaderboardPosition> (RecieveMyLeaderboardPosition);
        signalBus.Subscribe<Recieve3BestLeaderboard> (RecieveBestLeaderboardPositions);
        GetLeaderboardPosition ();
        publicGetBest3Leaderboard ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateLeaderboard(int subscribers)
    {
        leaderboardBackEnd.SendLeaderboard (subscribers);
    }
    void publicGetBest3Leaderboard ()
    {
        leaderboardBackEnd.GetBest3InLeaderboard ();
    }
    void GetLeaderboardPosition ()
    {
        leaderboardBackEnd.GetPlayerPositionInLeaderboard ();
    }

    void RecieveMyLeaderboardPosition (RecievePlayerLeaderboardPosition signal)
    {
        Debug.Log ($"Player pos: {signal.position+1}");
    }
    void RecieveBestLeaderboardPositions(Recieve3BestLeaderboard signal)
    {
        int i = 0;
        foreach(KeyValuePair<string, int> pair in signal.players)
        {
            Debug.Log ($"Position {i+1} : Player {pair.Key} : Subscribers {pair.Value}");
            i++;
        }
    }
}
