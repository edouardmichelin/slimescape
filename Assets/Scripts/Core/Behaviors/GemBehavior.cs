using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBehavior : AgentBehaviour
{
    void Update()
    {
        // transform.Rotate(0, 90 * Time.deltaTime, 0);
    }
    
    void OnCollisionEnter(Collision collisionInfo)
    {
        GameObject collider = collisionInfo.collider.transform.parent.gameObject;
        if (collider.CompareTag(Config.TAG_DOG))
        {
            if (GameManager.Instance.TrySetNewGemOwner(collider))
                Destroy(this);
        }
    }
}