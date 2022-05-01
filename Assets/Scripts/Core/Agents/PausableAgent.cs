using UnityEngine;
using System.Collections.Generic;

/// <summary> 
/// This class represent a pausable object 
/// </summary>

public class PausableAgent : MonoBehaviour
{
    protected virtual void Start()
    {
        GameManager.Instance.RegisterPausable(this);
    }

    public virtual void Pause()
    {
        enabled = false;
    }

    public virtual void Unpause()
    {
        enabled = true;
    }
}