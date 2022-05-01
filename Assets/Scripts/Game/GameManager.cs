using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<InputKeyboard, int> m_scoreBoard;
    private Dictionary<InputKeyboard, bool> m_readyStates;
    private Dictionary<InputKeyboard, MoveWithKeyboardBehavior> m_playerBehaviors;
    
    private List<CelluloAgent> m_sleeping;
    private bool m_isInitialized = false;
    private bool m_isGameStarted = false;
    private bool m_isGamePaused = false;
    private GameObject m_gameOverObj;

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
        m_readyStates = new Dictionary<InputKeyboard, bool>();
        m_playerBehaviors = new Dictionary<InputKeyboard, MoveWithKeyboardBehavior>();


        foreach (GameObject player in  GameObject.FindGameObjectsWithTag(Config.TAG_DOG))
        {
            if (TryGetPlayerIdFromGameObject(player, out InputKeyboard playerId))
            {

                m_readyStates.Add(playerId, false);
                m_scoreBoard.Add(playerId, 0);
                m_playerBehaviors.Add(playerId, player.GetComponent<MoveWithKeyboardBehavior>());
            }
        }
    }

    public void StopGame()
    {
        Timer = Config.GAME_DURATION;
        IsGamePaused = true;
        m_isGameStarted = false;
    }
    
    public bool TryUpdateReadyState(MoveWithKeyboardBehavior player)
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag(Config.TAG_START_PROMPT);
        if (!m_readyStates.ContainsKey(player.inputKeyboard) || menus.Length == 0)
            return false;

        m_readyStates[player.inputKeyboard] = true;
        if (!m_readyStates.ContainsValue(false))
        {
            foreach (GameObject menu in menus)
            {
                menu.SetActive(false);
            }

            if (!HasGameStarted)
            {
                StartGame();
                IsGamePaused = false;
            }
        }
        
        foreach (var keyValuePair in m_readyStates)
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
        if (!TryGetPlayerIdFromGameObject(player, out InputKeyboard playerId))
            return false;
        
        if (!m_scoreBoard.ContainsKey(playerId))
            return false;
        
        m_scoreBoard[playerId] += points;
        
        return true;
    }
    
    public void AllMoveOnStone()
    {
        foreach (MoveWithKeyboardBehavior player in m_playerBehaviors.Values)
        {
            player.MoveOnStone();
        }
    }

    public void AllMoveNormally()
    {
        foreach (MoveWithKeyboardBehavior player in m_playerBehaviors.Values)
        {
            player.MoveNormally();
        }
    }

    public bool TrySetTimer (int seconds)
    {
        if (seconds <= 0)
        {
            return false;
        }
        else
        {
            Timer = seconds;
            return true;
        }
    }

    public bool TrySetNewGemOwner(GameObject owner)
    {
        if (!TryGetPlayerIdFromGameObject(owner, out InputKeyboard playerId))
            return false;
        
        if (!m_scoreBoard.ContainsKey(playerId))
            return false;
        
        foreach (MoveWithKeyboardBehavior player in m_playerBehaviors.Values)
        {
            player.IsGemOwner = player.inputKeyboard == playerId;
        }

        return true;
    }
    
    public bool TryGetPlayerIdFromGameObject(GameObject obj, out InputKeyboard playerId)
    {
        playerId = InputKeyboard.NULL;
        
        MoveWithKeyboardBehavior player = obj.GetComponent<MoveWithKeyboardBehavior>();

        if (player == null)
            return false;

        playerId = player.inputKeyboard;
        
        return true;
    }
}
