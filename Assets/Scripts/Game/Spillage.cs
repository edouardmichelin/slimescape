using UnityEngine;

public class Spillage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.transform.parent.gameObject;
        
        if (obj.CompareTag(Config.TAG_DOG))
        {
            MoveWithKeyboardBehavior b = obj.GetComponent<MoveWithKeyboardBehavior>();
            if (b == null)
                return;

            b.MoveOnStone();
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject obj = other.transform.parent.gameObject;
        
        if (obj.CompareTag(Config.TAG_DOG))
        {
            MoveWithKeyboardBehavior b = obj.GetComponent<MoveWithKeyboardBehavior>();
            if (b == null)
                return;

            b.MoveNormally();
        }
    }
}