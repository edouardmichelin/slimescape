using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region properties declaration
    
    private Dictionary<GameObject, Player> m_playersStates;
    private GhostSheepBehavior m_slime;
    private bool m_isInitialized = false;
    private bool m_isGameStarted = false;
    private bool m_isGamePaused = false;
    private GameObject m_gameOverObj;
    private Difficulty m_difficulty;

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

    #endregion
    
    // Initialization
    public void Init()
    {
        if (m_isInitialized) return;

        m_playersStates = new Dictionary<GameObject, Player>();
        
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

    private void ApplyGameDifficulty(Difficulty difficulty)
    {
        SpawnManager.Instance.SetTimersWithDifficulty(difficulty);
        switch (difficulty)
        {
            case Difficulty.Easy:
                m_slime.SetLowVelocity();
                break;
            case Difficulty.Normal:
                m_slime.SetNormalVelocity();
                break;
            case Difficulty.Hard:
                m_slime.SetHighVelocity();
                break;
            default: break;
        }
    }

    public void SetGameDifficulty(Difficulty difficulty)
    {
        m_difficulty = difficulty;
    }

    public void StartGame()
    {
        if (m_isGameStarted)
            return;

        ApplyGameDifficulty(m_difficulty);

        m_isGameStarted = true;
        
        SpawnManager.Instance.Enable();
    }

    public void StopGame()
    {
        IsGamePaused = true;
        HasGameStarted = false;
        SpawnManager.Instance.Disable();
        Timer = Config.GAME_DURATION;
        m_playersStates = new Dictionary<GameObject, Player>();
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
        Player player = m_playersStates.Values.Where(w => w.Id == playerId).FirstOrDefault();

        if (player == null)
            return false;
        
        if (!m_playersStates.ContainsKey(player.Behavior.gameObject))
            return false;
        
        score = m_playersStates[player.Behavior.gameObject].Score;
        return true;
    }

    public bool TryUpdateScoreOf(GameObject player, int points)
    {
        if (!m_playersStates.ContainsKey(player))
            return false;

        m_playersStates[player].Score += points;

        return true;
    }

    public bool TryRegisterPlayer(MoveWithKeyboardBehavior behavior, InputKeyboard id)
    {
        if (m_playersStates.ContainsKey(behavior.gameObject))
            return false;
        
        var player = new Player();
        player.IsReady = false;
        player.Score = 0;
        player.Behavior = behavior;
        player.Id = id;

        m_playersStates.Add(behavior.gameObject, player);

        return true;
    }

    public bool TryRegisterSlime(GhostSheepBehavior behavior)
    {
        if (m_slime)
            return false;

        m_slime = behavior;

        return true;
    }

    public bool TryUpdatePlayerColor(MoveWithKeyboardBehavior behavior, Player.Colors color)
    {
        if (!m_playersStates.ContainsKey(behavior.gameObject))
            return false;

        m_playersStates[behavior.gameObject].Color = color;
        return true;
    }
    
    public bool TryUpdateReadyState(MoveWithKeyboardBehavior player)
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag(Config.TAG_START_PROMPT);

        if (!m_playersStates.ContainsKey(player.gameObject) || menus.Length == 0)
            return false;

        m_playersStates[player.gameObject].IsReady = true;
        if (m_playersStates.Values.All(a => a.IsReady))
        {
            Array.ForEach(menus, f => f.SetActive(true));

            StartGame();
            IsGamePaused = false;
        }
        
        return true;
    }
    
    public void AllMoveOnStone()
    {
        foreach (Player b in m_playersStates.Values)
        {
            b.Behavior.MoveOnStone();
        }
    }

    public void AllMoveNormally()
    {
        foreach (Player b in m_playersStates.Values)
        {
            b.Behavior.MoveNormally();
        }
    }

    public bool TrySetTimer(int seconds)
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
        if (!m_playersStates.ContainsKey(owner))
            return false;
        
        if (!TryGetPlayerIdFromGameObject(owner, out InputKeyboard ownerId))
            return false;
        
        foreach (MoveWithKeyboardBehavior player in m_playersStates.Values.Select(s => s.Behavior))
        {
            player.IsGemOwner = player.inputKeyboard == ownerId;
        }

        return true;
    }
    
    public bool TryGetPlayerIdFromGameObject(GameObject obj, out InputKeyboard playerId)
    {
        playerId = InputKeyboard.arrows;
        
        MoveWithKeyboardBehavior player = obj.GetComponent<MoveWithKeyboardBehavior>();

        if (player == null)
            return false;

        playerId = player.inputKeyboard;
        
        return true;
    }
}
