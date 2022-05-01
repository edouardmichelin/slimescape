using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float m_timer = 0f;
    private float m_defaultMaxSpawnDeltaTime = 20f;
    private float m_gemSpawnDeltaTime = 10f;

    public void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        /// call spawners
        SpawnGem();
    }

    void SpawnGem()
    {
        if ((timer % m_gemSpawnDeltaTime) < 0.1)
        {
            Random.Range(0f, Config.);
        }
    }
}