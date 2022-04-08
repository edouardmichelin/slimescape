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

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.CompareTag(Config.TAG_SHEEP))
        {
            GameObject nearest = GetNearestDogFrom(other.gameObject);
            print(nearest.name);
            GameManager.Instance.TryUpdateScoreOf(nearest, Config.POINTS_FOR_SHEEP_IN_RING);
            AudioManager.Instance.PlayGlobalEffect("winPoint");
        }
    }

    private GameObject GetNearestDogFrom(GameObject from)
    {
        float shortestDistance = float.PositiveInfinity;
        GameObject nearestDog = null;

        foreach (GameObject dog in m_dogs)
        {
            float candidate = Vector3.Distance(from.transform.position, dog.transform.position);
            if (candidate < shortestDistance)
            {
                nearestDog = dog;
                shortestDistance = candidate;
            }
        }

        return nearestDog;
    }
}
