using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerAnnouncer : MonoBehaviour
{
    private TextMeshProUGUI text;
    public PlayerId player1, player2;
    
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        DisplayWinner();
    }
    

    public void DisplayWinner()
    {
        bool x = GameManager.Instance.TryGetScoreOf(player1, out int score1);

        bool y = GameManager.Instance.TryGetScoreOf(player2, out int score2);
        
        if (score1 > score2)
        {
            text.text = "Player 1 wins!";
        } 
        else if (score1 < score2)
        {
            text.text = "Player 2 wins!";
        }
        else
        {
            text.text = "Tie!";
        }
    }
}
