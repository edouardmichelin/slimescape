using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
        set { AudioManager.Instance.MuteSounds  = !value; }
    }

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
    
    public void TryDecrementTimer()
    {
        float timer = GameManager.Instance.Timer;
        if (timer > 0)
        {
            GameManager.Instance.TrySetTimer((int)(timer/Config.ONE_MIN_DURATION) - 1);
        }
    }
    
    public void TryIncrementTimer()
    {
        float timer = GameManager.Instance.Timer;
        if (timer <=  Config.MAX_MINUTES_DURATION * Config.ONE_MIN_DURATION)
        {
            GameManager.Instance.TrySetTimer((int)(timer/Config.ONE_MIN_DURATION) + 1);
        }
    }

    public void SetTimerWithInputValue (string minutes)
    {
        bool result = int.TryParse(minutes, out int minutesInt);
        if (result)
        {
            GameManager.Instance.TrySetTimer(minutesInt);
        }
    }
}
