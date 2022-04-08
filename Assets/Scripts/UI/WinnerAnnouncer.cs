using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerAnnouncer : MonoBehaviour
{
    private TextMeshProUGUI text;
    public GameObject player1, player2;

    private InputKeyboard id1, id2;
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        id1 = player1.GetComponent<MoveWithKeyboardBehavior>().inputKeyboard;
        id2 = player2.GetComponent<MoveWithKeyboardBehavior>().inputKeyboard;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.TryGetScoreOf(id1, out int score1)) ;

        if (GameManager.Instance.TryGetScoreOf(id2, out int score2)) ;
        
        DisplayWinner(score1, score2);
    }

    private void DisplayWinner(int score1, int score2)
    {
        if (score1 > score2)
        {
            text.SetText("Player 1 wins !");
        } else if (score2 > score1)
        {
            text.SetText("Player 2 wins !");
        }
        else
        {
            text.SetText("Tie !");
        }
    }
}
