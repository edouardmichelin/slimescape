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
    
    private Dictionary<PlayerId, Player> m_playersStates;
    private GhostSheepBehavior m_slime = null;
    private bool m_isInitialized = false;
    private bool m_isGameStarted = false;
    private bool m_isGamePaused = false;
    private bool m_isSuddenDeathPhase = false;
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

        m_playersStates = new Dictionary<PlayerId, Player>();
        
        AudioManager.Instance.Init();

        Timer = Config.GAME_DURATION;

        IsGamePaused = true;

        m_isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isSuddenDeathPhase)
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
    }

    public void SlimeKidnapped()
    {
        if (!HasGameStarted)
            return;
        
        GameOver();
    }

    public void PlayerKidnapped(PlayerId playerId)
    {
        if (!HasGameStarted)
            return;
        
        if (!m_playersStates.ContainsKey(playerId))
            return;

        m_playersStates[playerId].IsKidnapped = true;
        IsGamePaused = true;
    }

    public void PlayerUnkidnapped(PlayerId playerId)
    {
        if (!HasGameStarted)
            return;
        
        if (!m_playersStates.ContainsKey(playerId))
            return;

        m_playersStates[playerId].IsKidnapped = false;
        
        if (m_playersStates.Values.All(p => !p.IsKidnapped))
            IsGamePaused = false;
    }

    private void ApplyGameDifficulty(Difficulty difficulty)
    {
        Debug.Log($"Launching game in {difficulty}");
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
        
        ResetScores();

        m_isGameStarted = true;
        
        SpawnManager.Instance.Enable();
        
        GlobalAnnouncer.Instance.Init();
        GlobalAnnouncer.Instance.Say("Let's go!");

        ApplyGameDifficulty(m_difficulty);
    }

    public void StopGame()
    {
        IsGamePaused = true;
        HasGameStarted = false;
        m_isSuddenDeathPhase = false;
        SpawnManager.Instance.Disable();
        Timer = Config.GAME_DURATION;
    }

    private void GameOver()
    {
        AudioManager.Instance.PlayGlobalEffect("gameOver");

        if (PlayersAreTied())
        {
            SuddenDeahtPhase();
        }
        else
        {
            if (m_gameOverObj != null)
            {
                m_gameOverObj.SetActive(true);
                m_gameOverObj = null;
            }
            
            StopGame();
        }

    }

    private bool PlayersAreTied()
    {
        Player p = m_playersStates.FirstOrDefault().Value;
        if (p == null)
            return false;

        int score = p.Score;

        return m_playersStates.Values.All(p => p.Score == score);
    }

    private void ResetScores()
    {
        foreach (Player player in m_playersStates.Values)
        {
            player.Score = 0;
        }
    }

    private void SuddenDeahtPhase()
    {
        IsGamePaused = true;
        m_isSuddenDeathPhase = true;
        Timer = 0f;
        ResetScores();
        AllMoveOnStone();
        m_slime.SuddenDeathMode();
        ApplyGameDifficulty(Difficulty.Hard);
        IsGamePaused = false;
    }

    public bool TryGetScoreOf(PlayerId playerId, out int score)
    {
        score = 0;

        Player actualPlayer = m_playersStates.Values.FirstOrDefault(f => f.Id == playerId);
        
        if (actualPlayer == null)
            return false;
        
        score = actualPlayer.Score;
        return true;
    }

    public bool TryUpdateScoreOf(GameObject go, int points)
    {
        if (go.TryGetComponent<MoveWithKeyboardBehavior>(out MoveWithKeyboardBehavior behavior))
        {
            if (m_playersStates.ContainsKey(behavior.id))
            {
                m_playersStates[behavior.id].Score += points;
                if (points < 0)
                    behavior.OnLosePoints();
                else
                    behavior.OnWinPoints();
                

                if (m_isSuddenDeathPhase)
                    GameOver();

                return true;
            }
        }
        
        return false;
    }

    public bool TryRegisterPlayer(MoveWithKeyboardBehavior behavior, PlayerId id, InputKeyboard controls)
    {
        if (m_playersStates.ContainsKey(id))
            return false;
        
        var player = new Player();
        player.IsReady = false;
        player.Score = 0;
        player.Behavior = behavior;
        player.Id = id;
        player.Controls = controls;

        m_playersStates.Add(id, player);

        return true;
    }

    public bool TryRegisterSlime(GhostSheepBehavior behavior)
    {
        if (m_slime != null)
            return false;

        m_slime = behavior;

        return true;
    }

    public bool TryUpdatePlayerColor(PlayerId id, Player.Colors color)
    {
        if (!m_playersStates.ContainsKey(id))
            return false;

        m_playersStates[id].Color = color;
        return true;
    }
    
    public bool TryUpdateReadyState(PlayerId id)
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag(Config.TAG_START_PROMPT);

        if (!m_playersStates.ContainsKey(id) || menus.Length == 0)
            return false;

        m_playersStates[id].IsReady = true;
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
    
    public bool TryTeleportPlayer(GameObject pobj)
    {
        if (pobj.TryGetComponent<MoveWithKeyboardBehavior>(out MoveWithKeyboardBehavior behavior))
        {
            if (m_playersStates.ContainsKey(behavior.id))
            {
                Player otherPlayer = m_playersStates
                    .Values
                    .FirstOrDefault(w => w.Behavior.id != behavior.id);

                if (otherPlayer == null)
                    return false;

                // swap ids, colors and controls
                behavior.TeleportTo(otherPlayer.Behavior);

                return true;
            }
        }

        return false;
    }

    public bool TrySetNewGemOwner(GameObject owner)
    {
        if (!owner.TryGetComponent<MoveWithKeyboardBehavior>(out MoveWithKeyboardBehavior behavior))
            return false;
        
        if (!m_playersStates.ContainsKey(behavior.id))
            return false;
        
        foreach (MoveWithKeyboardBehavior player in m_playersStates.Values.Select(s => s.Behavior))
        {
            player.IsGemOwner = player.id == behavior.id;
        }

        return true;
    }
}
