using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class Leaderboard_VC : MonoBehaviour
{
    [Inject] PlayerDataManager playerDataManager;

    [SerializeField] private TMP_Text firstPlayerName;
    [SerializeField] private TMP_Text secondPlayerName;
    [SerializeField] private TMP_Text thirdPlayerName;

    [SerializeField] private TMP_Text currentPlayerPosition;
    [SerializeField] private TMP_Text currentPlayerName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetBestPlayers (string[] _playersNames)
    {
        firstPlayerName.text = _playersNames[0];
        secondPlayerName.text = _playersNames[1];
        thirdPlayerName.text = _playersNames[2];
    }

    void SetCurrentPlayer (int _position)
    {
        currentPlayerPosition.text = _position.ToString ();
        currentPlayerName.text = playerDataManager.GetPlayerName ();
    }
}
