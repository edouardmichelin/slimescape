using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    private float m_timer = 0f;
    private bool m_isEnabled = false;
    private TimerInterval m_gemSpawnerTimer;
    
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
        InitTimers();
        SetRandomDeltaTimes();
        m_timer = 0f;
        m_isEnabled = true;
    }

    public void Disable()
    {
        m_isEnabled = false;
    }

    public void SetTimersWithDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                m_gemSpawnerTimer.IncreaseOffset();
                break;
            case Difficulty.Normal:
                m_gemSpawnerTimer.DefaultOffset();
                break;
            case Difficulty.Hard:
                m_gemSpawnerTimer.DecreaseOffset();
                break;
            default: break;
        }
    }

    private void SpawnGem()
    {
        if (m_timer > m_gemSpawnerTimer.Interval)
        {
            AudioManager.Instance.PlaySoundEffect("gemSpawn");
            Instantiate(gem, GetRandomCoordinates(), transform.rotation * Quaternion.Euler (-90f, 0f, 0f));
            m_gemSpawnerTimer.Randomize();
            m_timer = 0f;
        }
    }

    private Vector3 GetRandomCoordinates()
    {
        float posX = Random.Range(1f, (Config.UNITY_MAP_DIMENSION_Y - 1f));
        float posZ = Random.Range(-1f, -1f * (Config.UNITY_MAP_DIMENSION_X - 1f));
        
        return new Vector3(posX * 2f, 0f, posZ * 2f);
    }

    private void InitTimers()
    {
        m_gemSpawnerTimer = new TimerInterval(
            Config.SPAWNER_GEMS_MIN_TIME_INTERVAL,
            Config.SPAWNER_GEMS_MAX_TIME_INTERVAL,
            Config.SPAWNER_GEMS_DIFFUCULTY_DELTA_TIME_INTERVAL);
    }

    private void SetRandomDeltaTimes()
    {
        m_gemSpawnerTimer.Randomize();
    }

    private class TimerInterval
    {
        private float m_defaultMin;
        private float m_defaultMax;
        private float m_delta;
        public float Min { get; private set; }
        public float Max { get; private set; }
        public float Interval { get; private set; }

        public void Randomize()
        {
            Interval = Random.Range(Min, Max);
        }

        public void IncreaseOffset()
        {
            Min = m_defaultMin + m_delta;
            Max = m_defaultMax + m_delta;
            Randomize();
        }

        public void DefaultOffset()
        {
            Min = m_defaultMin;
            Max = m_defaultMax;
            Randomize();
        }

        public void DecreaseOffset()
        {
            Min = m_defaultMin - m_delta;
            Max = m_defaultMax - m_delta;
            Randomize();
        }

        public TimerInterval(float min, float max, float delta)
        {
            m_defaultMin = min;
            Min = min;
            m_defaultMax = max;
            Max = max;
            m_delta = delta;
        }
    }
}