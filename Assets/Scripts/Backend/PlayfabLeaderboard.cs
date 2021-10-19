using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Zenject;

public class PlayfabLeaderboard : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlayerDataManager playerDataManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "SubscribersCount",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics (request, OnLeaderboardUpdate, OnLeaderboardError);
    }
    void OnLeaderboardUpdate (UpdatePlayerStatisticsResult result)
    {
        Debug.LogWarning ("Leaderboard Updated Successfully");
    }
    void OnLeaderboardError (PlayFabError error)
    {
        Debug.LogError ("Leaderboard Error");
        Debug.LogError (error.GenerateErrorReport ());
    }

    public void GetBest3InLeaderboard () 
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "SubscribersCount",
            StartPosition = 0,
            MaxResultsCount = 3
        };
        PlayFabClientAPI.GetLeaderboard (request, OnLeaderboardGet, OnLeaderboardError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        Dictionary<string, int> best3Players = new Dictionary<string, int> ();
        foreach(var item in result.Leaderboard)
        {
            best3Players.Add (item.DisplayName, item.StatValue);
        }
        signalBus.Fire<Recieve3BestLeaderboard> (new Recieve3BestLeaderboard ()
        {
            players = best3Players
        });    
    }

    public void GetPlayerPositionInLeaderboard ()
    {
        string playerID = playerDataManager.GetPlayerName ();

        GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest 
        {
            StatisticName = "SubscribersCount",
            PlayFabId = playerID,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer (request, OnLeaderboardGetAroundPlayer, OnLeaderboardError);
    }
    void OnLeaderboardGetAroundPlayer (GetLeaderboardAroundPlayerResult result)
    {
        signalBus.Fire<RecievePlayerLeaderboardPosition> (new RecievePlayerLeaderboardPosition ()
        {
            position = result.Leaderboard[0].Position
        });
    }
}
