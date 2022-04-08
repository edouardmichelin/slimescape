using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<GameObject, int> m_scoreBoard = new Dictionary<GameObject, int>();
    private bool m_isInitialized = false;
    
    // Start is called before the first frame update
    public void Init()
    {
        if (m_isInitialized) return;
        
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Config.TAG_DOG))
        {
            m_scoreBoard.Add(player, 0);
        }

        AudioManager.Instance.Init();

        IsGamePaused = true;

        m_isInitialized = true;
    }

    public bool IsGamePaused
    {
        get { return Time.timeScale == 1; }
        set { Time.timeScale = value ? 0 : 1; }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        print("--- Scoreboard ---");
        
        foreach (var entry in m_scoreBoard)
        {
            print($"{entry.Key.name} - {entry.Value} pts");
        }

        print("------------------");
        */
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
