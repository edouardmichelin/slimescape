using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
	This class is the implementation of the timer used in the game and how it is handled in it
*/
public class Timer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    public Slider slider;

    // Start is called before the first frame update
    public void Start()
    {
        float time = GameManager.Instance.Timer;
        timerText = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void Update() {
        float time = GameManager.Instance.Timer;
        timerText.text = string.Format("{0:00}:{1:00}", Math.Max(Math.Floor(time / 60f), 0f), time % 60);
    }
}
