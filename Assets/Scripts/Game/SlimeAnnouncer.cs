using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlimeAnnouncer : Singleton<SlimeAnnouncer>
{
    private GameObject container;
    private TextMeshProUGUI textComponent;
    private float m_displayDuration = 1f;

    public void Init()
    {
        HideMessage();
    }
    
    public void Destroy()
    {
        HideMessage();
    }

    public void RegisterContainer(GameObject go)
    {
        container = go;
        if (AllLoaded())
            HideMessage();
    }

    public void RegisterTextComponent(GameObject go)
    {
        textComponent = go.GetComponent<TextMeshProUGUI>();
        if (AllLoaded())
            HideMessage();
    }

    public void Say(string message)
    {
        if (GameManager.Instance.IsGamePaused)
            return;
        
        textComponent.text = message;
        DisplayMessage();
    }

    private bool AllLoaded()
    {
        return container != null && textComponent != null;
    }

    private void DisplayMessage()
    {
        CancelInvoke();
        container.SetActive(true);
        Invoke(nameof(HideMessage), m_displayDuration);
    }

    private void HideMessage()
    {
        container.SetActive(false);
    }
}