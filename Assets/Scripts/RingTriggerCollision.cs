using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTriggerCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.transform.parent.gameObject.GetComponent<CelluloAgent>().SetVisualEffect(VisualEffect.VisualEffectConstAll, new Color(1, 0, 0, 1), 255);
        if (other.transform.parent is CelluloAgent)
        {
            Debug.Log("test");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
