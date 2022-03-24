using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<GameObject, int> m_scoreBoard = new Dictionary<GameObject, int>();
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Config.TAG_DOG))
        {
            m_scoreBoard.Add(player, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        print("--- Scoreboard ---");
        
        foreach (var entry in m_scoreBoard)
        {
            print($"{entry.Key.name} - {entry.Value} pts");
        }

        print("------------------");
    }

    public Dictionary<GameObject, int> ScoreBoard
    {
        get
        {
            return new Dictionary<GameObject, int>(m_scoreBoard);
        }
    }

    public bool TryUpdateScoreOf(GameObject player, int points)
    {
        if (!m_scoreBoard.ContainsKey(player))
            return false;
        
        m_scoreBoard[player] += points;
        
        return true;
    }
}
