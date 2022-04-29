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

    public void SendLeaderboard(string leaderboardName, int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics (request, OnLeaderboardUpdate, OnLeaderboardError);
    }
    void OnLeaderboardUpdate (UpdatePlayerStatisticsResult result)
    {
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

    public void GetTop10InLeaderboard ()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "SubscribersCount",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard (request, OnLeaderboardGet, OnLeaderboardError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        Dictionary<string, ulong> bestPlayers = new Dictionary<string, ulong> ();
        foreach(var item in result.Leaderboard)
        {
            if(item.DisplayName!=null)
                bestPlayers.Add (item.DisplayName, (ulong)item.StatValue);
        }
        if(bestPlayers.Count == 3)
        {
            signalBus.Fire<Recieve3BestLeaderboard> (new Recieve3BestLeaderboard ()
            {
                players = bestPlayers
            });
        }
        signalBus.Fire<RecieveTop10Leaderboard>(new RecieveTop10Leaderboard()
        {
            players = bestPlayers
        });   
    }

    public void GetPlayerPositionInLeaderboard ()
    {
        string playerID = PlayerDataManager.Instance.GetPlayerName ();

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
