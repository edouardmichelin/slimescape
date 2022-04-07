using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public float MasterVolume
    {
        set { AudioManager.Instance.Volume = value;  }
    }
}