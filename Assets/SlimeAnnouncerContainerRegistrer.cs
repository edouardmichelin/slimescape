using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnnouncerContainerRegistrer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SlimeAnnouncer.Instance.RegisterContainer(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
