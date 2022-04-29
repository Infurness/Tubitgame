using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class Leaderboard_VC : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] PlayerDataManager playerDataManager;

    [SerializeField] private TMP_Text firstPlayerName;
    [SerializeField] private TMP_Text secondPlayerName;
    [SerializeField] private TMP_Text thirdPlayerName;

    [SerializeField] private TMP_Text currentPlayerPosition;
    [SerializeField] private TMP_Text currentPlayerName;


    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<RecievePlayerLeaderboardPosition> (RecieveMyLeaderboardPosition);
        signalBus.Subscribe<Recieve3BestLeaderboard> (RecieveBestLeaderboardPositions);


    }

    void RecieveBestLeaderboardPositions (Recieve3BestLeaderboard signal)
    {
        int i = 0;
        foreach (KeyValuePair<string, ulong> pair in signal.players)
        {
            SetBestPlayers (i, pair.Key);
            //Debug.Log ($"Position {i + 1} : Player {pair.Key} : Subscribers {pair.Value}");
            i++;
        }
    }

    void RecieveMyLeaderboardPosition (RecievePlayerLeaderboardPosition signal)
    {
        Debug.Log ($"Player pos: {signal.position+1}");
        SetCurrentPlayer (signal.position+1);
    }
    void SetBestPlayers (int leaderboardPos, string playerName)
    {
        switch(leaderboardPos)
        {
            case 0:
                 firstPlayerName.text = playerName;
                break;
            case 1:
                secondPlayerName.text = playerName;
                break;
            case 2:
                thirdPlayerName.text = playerName;
                break;
        }
    }

    void SetCurrentPlayer (int _position)
    {
        currentPlayerPosition.text = _position.ToString ();
        currentPlayerName.text = playerDataManager.GetPlayerName ();
    }

}
