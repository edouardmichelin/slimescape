using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBehavior : AgentBehaviour
{
    private const int POINTS_FOR_BEING_CAUGHT_BY_GHOST = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.transform.parent.gameObject.tag == "Ghost")
        {
            GameManager.Instance.TryUpdateScoreOf(this.gameObject, Config.POINTS_FOR_PLAYER_CAUGHT_BY_GHOST);
        }
    }
}
