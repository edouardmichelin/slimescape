using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    
    public int position = 0;
    public int samplerate = 44100;
    public float frequency = 440;

    public float Volume
    {
        get { return source.volume;  }
        set { source.volume = value;  }
    }

    private AudioSource source;
    private Dictionary<String, AudioClip> m_clips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        source = gameObject.AddComponent<AudioSource>();
        foreach (var clip in Resources.LoadAll<AudioClip>("Audio"))
        {
            m_clips.Add(clip.name, clip);
        }
    }
    
    public void Play(String name)
    {
        if (m_clips.TryGetValue(name, out AudioClip clip))
        {
            source.clip = clip;
            source.Play();
        }
    }
}
