using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBehavior : AgentBehaviour
{
    void Start()
    {
        if (SpawnManager.Instance.gem == null)
            SpawnManager.Instance.gem = gameObject;
    }
    
    void OnTriggerEnter(Collider other)
    {
        GameObject collider = other.transform.parent.gameObject;
        
        if (collider.CompareTag(Config.TAG_DOG))
        {
            /*
            if (GameManager.Instance.TrySetNewGemOwner(collider))
            {
                AudioManager.Instance.PlaySoundEffect("gemTake");
                Destroy(gameObject);
            }
            */
            if (GameManager.Instance.TryTeleportPlayer(collider))
            {
                AudioManager.Instance.PlaySoundEffect("gemTake");
                Destroy(gameObject);
            }
        }
    }
}