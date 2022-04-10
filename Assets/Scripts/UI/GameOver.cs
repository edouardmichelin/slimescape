using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.GameOverMenu = gameObject;
        gameObject.SetActive(false);
    }

}
