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
}
