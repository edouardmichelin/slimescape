using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTrigger : MonoBehaviour
{
    private GameObject[] m_dogs;

    // Start is called before the first frame update
    void Start()
    {
        m_dogs = GameObject.FindGameObjectsWithTag(Config.TAG_DOG);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.tag == Config.TAG_SHEEP)
        {
            GameObject nearest = GetNearestDogFrom(other.gameObject);
            GameManager.Instance.TryUpdateScoreOf(nearest, Config.POINTS_FOR_SHEEP_IN_RING);
        }
    }

    private GameObject GetNearestDogFrom(GameObject from)
    {
        float shortestDistance = Config.UNITY_MAP_DIMENSION_X;
        GameObject nearestDog = null;

        foreach (GameObject dog in m_dogs)
        {
            float candidate = Vector3.Distance(this.transform.position, dog.transform.position);
            if (candidate < shortestDistance)
                nearestDog = dog;
        }

        return nearestDog;
    }
}
