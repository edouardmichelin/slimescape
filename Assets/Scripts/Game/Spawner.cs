using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float m_timer = 0f;
    private float m_defaultMaxSpawnDeltaTime = 20f;

    public GemBehavior gem;

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
        if ((timer % Config.SPAWNER_GEMS_MAX_TIME_INTERVAL) < 0.1)
        {
            Instantiate(gem, GetRandomCoordinates(), Quaternion.identity);
        }
    }

    private Vector3 GetRandomCoordinates()
    {
        float posX = Random.Range(0f, Config.UNITY_MAP_DIMENSION_X);
        float posZ = Random.Range(0f, Config.UNITY_MAP_DIMENSION_Y);
        
        return new Vector3(posX, 0f, posZ);
    }
}