using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public float MasterVolume
    {
        set
        {
            m_music.volume = value;
            m_effect.volume = value;
            m_globalEffect.volume = value;
        }
    }
    
    public float MusicVolume
    {
        get { return m_music.volume;  }
        set { m_music.volume = value;  }
    }
    
    public float EffectsVolume
    {
        get { return m_effect.volume;  }
        set { m_effect.volume = value;  }
    }
    
    public float GlobalEffectVolume
    {
        get { return m_globalEffect.volume;  }
        set { m_globalEffect.volume = value;  }
    }
    
    public bool MuteBackgroundMusic
    {
        get { return m_music.mute;  }
        set { m_music.mute = value;  }
    }

    private AudioSource m_music;
    private AudioSource m_effect;
    private AudioSource m_globalEffect;
    private Dictionary<String, AudioClip> m_clips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        m_music = gameObject.AddComponent<AudioSource>();
        m_effect = gameObject.AddComponent<AudioSource>();
        m_globalEffect = gameObject.AddComponent<AudioSource>();
        
        m_music.loop = true;
        
        foreach (var clip in Resources.LoadAll<AudioClip>("Audio"))
        {
            m_clips.Add(clip.name, clip);
        }
        
        PlayBackgroundMusic("StreetLove");
    }
    
    public void PlaySoundEffect(String name)
    {
        Play(name, m_effect);
    }

    public void PlayGlobalEffect(String name)
    {
        Play(name, m_globalEffect);
    }
    
    public void PlayBackgroundMusic(String name)
    {
        Play(name, m_music);
    }

    private void Play(String name, AudioSource source)
    {
        if (m_clips.TryGetValue(name, out AudioClip clip))
        {
            source.Stop();
            source.clip = clip;
            source.Play();
        }
    }
}
