using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public PlayerId playerId;
    
    private TextMeshProUGUI timerText;

    // Start is called before the first frame update
    public void Start()
    {
        timerText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (GameManager.Instance.TryGetScoreOf(playerId, out int score))
        {
            timerText.text = Convert.ToString(Convert.ToString(score));
        }
    }
}