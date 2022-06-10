using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalAnnouncer : Singleton<GlobalAnnouncer>
{
    private GameObject container;
    private TextMeshProUGUI textComponent;
    
    public void Init()
    {
        HideMessage();
    }

    public void RegisterContainer(GameObject go)
    {
        container = go;
    }

    public void RegisterTextComponent(GameObject go)
    {
        textComponent = go.GetComponent<TextMeshProUGUI>();
        Say("Choisissez votre couleur et vos contrôles");
    }

    public void Say(string message)
    {
        textComponent.text = message;
        DisplayMessage();
    }

    private void DisplayMessage()
    {
        CancelInvoke();
        container.SetActive(true);
        Invoke(nameof(HideMessage), 5f);
    }

    private void HideMessage()
    {
        container.SetActive(false);
    }
}
