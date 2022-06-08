using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region sound controller
    
    public float MasterVolume
    {
        set { AudioManager.Instance.MasterVolume = value; }
    }
    
    public float MusicVolume
    {
        set { AudioManager.Instance.MusicVolume = value;  }
    }
    
    public float EffectsVolume
    {
        set { AudioManager.Instance.EffectsVolume = value;  }
    }

    public bool BackgroundMusicToggle
    {
        set { AudioManager.Instance.MuteBackgroundMusic = !value;  }
    }

    public bool SoundToggle
    {
        set { AudioManager.Instance.GlobalMute = !value; }
    }

    #endregion

    #region scene controller

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ 1);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        GameManager.Instance.StopGame();
    }

    #endregion

    #region game flow controller

    public void Resume()
    {
        if (!GameManager.Instance.HasGameStarted)
        {
            GameManager.Instance.StartGame();
        }
        
        GameManager.Instance.IsGamePaused = false;
    }

    public void Pause()
    {
        GameManager.Instance.IsGamePaused = true;
    }

    #endregion

    #region timer settings

    public void TryDecrementTimer()
    {
        float timer = GameManager.Instance.Timer;
        if (timer > 0)
        {
            GameManager.Instance.TrySetTimer((int)(timer - 60f));
        }
    }
    
    public void TryIncrementTimer()
    {
        float timer = GameManager.Instance.Timer;
        if (timer <=  Config.MAX_GAME_DURATION)
        {
            GameManager.Instance.TrySetTimer((int)(timer + 60f));
        }
    }

    public void SetTimerWithInputValue (string seconds)
    {
        if (int.TryParse(seconds, out int secondsInt))
        {
            GameManager.Instance.TrySetTimer(secondsInt * 60);
        }
    }

    #endregion

    #region difficulty settings

    public void SetDiffcultyEasy()
    {
        GameManager.Instance.SetGameDifficulty(Difficulty.Easy);
    }

    public void SetDiffcultyNormal()
    {
        GameManager.Instance.SetGameDifficulty(Difficulty.Normal);
    }

    public void SetDiffcultyHard()
    {
        GameManager.Instance.SetGameDifficulty(Difficulty.Hard);
    }

    #endregion
    
}
