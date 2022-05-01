using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<InputKeyboard, int> m_scoreBoard = new Dictionary<InputKeyboard, int>();
    private Dictionary<MoveWithKeyboardBehavior, bool> m_players = new  Dictionary<MoveWithKeyboardBehavior, bool>();
    private List<CelluloAgent> m_sleeping;
    private bool m_isInitialized = false;
    private bool m_isGameStarted = false;
    private bool m_isGamePaused = false;
    private GameObject m_gameOverObj;
    private IList m_pausables = new List<PausableAgent>();

    public GameObject GameOverMenu
    {
        set { m_gameOverObj = value; }
    }

    public float Timer
    {
        get;
        private set;
    }

    public bool HasGameStarted
    {
        get { return m_isGameStarted; }
        private set { m_isGameStarted = value; }
    }

    public bool IsGamePaused
    {
/*
        get { return m_isGamePaused; }
        set
        {
            m_isGamePaused = value;
            if (value)
            {
                PauseAll();
            }
            else
            {
                WakeUpAll();
            }
        }
*/
        get { return Time.timeScale == 0f; }
        set { Time.timeScale = value ? 0f : 1f; }
    }
    
    // Initialization
    public void Init()
    {
        if (m_isInitialized) return;

        AudioManager.Instance.Init();

        Timer = Config.GAME_DURATION;

        IsGamePaused = true;

        m_isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsGamePaused)
        {
            Timer -= Time.deltaTime;
        }
        
        if (Timer <= 0f)
        {
            GameOver();
        }
    }

    public void StartGame()
    {
        if (m_isGameStarted)
            return;

        m_isGameStarted = true;
        
        m_scoreBoard = new Dictionary<InputKeyboard, int>();
        m_players = m_players = new  Dictionary<MoveWithKeyboardBehavior, bool>();

        foreach (GameObject player in  GameObject.FindGameObjectsWithTag(Config.TAG_DOG))
        {
            MoveWithKeyboardBehavior playerBehavior = player.GetComponent<MoveWithKeyboardBehavior>();
            InputKeyboard playerId = playerBehavior.inputKeyboard;
            m_players.Add(playerBehavior, false);
            m_scoreBoard.Add(playerId, 0);
        }
    }

    public void StopGame()
    {
        Timer = Config.GAME_DURATION;
        IsGamePaused = true;
        m_isGameStarted = false;
    }

    public void WakeUpAll()
    {
        foreach (PausableAgent pausable in m_pausables)
        {
            pausable.Unpause();
        }
    }

    public void PauseAll()
    {
        foreach (PausableAgent pausable in m_pausables)
        {
            pausable.Pause();
        }
    }

    public bool TryUpdateReadyState(MoveWithKeyboardBehavior player)
    {
        if (!m_players.ContainsKey(player))
            return false;

        m_players[player] = true;
        if (!m_players.ContainsValue(false))
            StartGame();
        foreach (var keyValuePair in m_players)
        {
            print($"{keyValuePair.Key} is {keyValuePair.Value}");
        }
        return true;
    }

    private void GameOver()
    {
        AudioManager.Instance.PlayGlobalEffect("gameOver");

        StopGame();

        if (m_gameOverObj != null)
        {
            m_gameOverObj.SetActive(true);
            m_gameOverObj = null;
        }

    }

    public bool TryGetScoreOf(InputKeyboard playerId, out int score)
    {
        score = 0;
        if (!m_scoreBoard.ContainsKey(playerId))
            return false;
        
        score = m_scoreBoard[playerId];
        return true;
    }

    public bool TryUpdateScoreOf(GameObject player, int points)
    {
        InputKeyboard playerId = player.GetComponent<MoveWithKeyboardBehavior>().inputKeyboard;
        if (!m_scoreBoard.ContainsKey(playerId))
            return false;
        
        m_scoreBoard[playerId] += points;
        
        return true;
    }

    public void AllMoveOnStone()
    {
        foreach (MoveWithKeyboardBehavior player in m_players.Keys.ToList())
        {
            player.MoveOnStone();
        }
    }

    public void AllMoveNormally()
    {
        foreach (MoveWithKeyboardBehavior player in m_players.Keys.ToList())
        {
            player.MoveNormally();
        }
    }

    public void RegisterPausable(PausableAgent pausableAgent)
    {
        m_pausables.Add(pausableAgent);
        if (IsGamePaused)
            pausableAgent.Pause();
    }
}
