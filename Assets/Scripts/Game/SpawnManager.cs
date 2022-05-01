using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    private float m_timer = 0f;
    private float m_defaultMaxSpawnDeltaTime = 20f;
    private bool m_isEnabled = false;
    private float m_gemSpawnerDeltaTime;
    
    public GameObject gem;

    void Update()
    {
        if (m_isEnabled)
        {
            m_timer += Time.deltaTime;
        
            /// call spawners
            SpawnGem();
        }
    }

    public void Enable()
    {
        SetRandomDeltaTimes();
        m_timer = 0f;
        m_isEnabled = true;
    }

    public void Disable()
    {
        m_isEnabled = false;
    }

    private void SpawnGem()
    {
        if ((m_timer % m_gemSpawnerDeltaTime) < Time.deltaTime)
        {
            AudioManager.Instance.PlaySoundEffect("gemSpawn");
            Instantiate(gem, GetRandomCoordinates(), Quaternion.identity);
        }
    }

    private Vector3 GetRandomCoordinates()
    {
        float posX = Random.Range(1f, (Config.UNITY_MAP_DIMENSION_Y - 1f));
        float posZ = Random.Range(-1f, -1f * (Config.UNITY_MAP_DIMENSION_X - 1f));
        
        return new Vector3(posX * 2f, 0.25f, posZ * 2f);
    }

    private void SetRandomDeltaTimes()
    {
        m_gemSpawnerDeltaTime = Random.Range(10f, Config.SPAWNER_GEMS_MAX_TIME_INTERVAL);
    }
}