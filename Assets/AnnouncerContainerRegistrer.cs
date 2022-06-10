using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerContainerRegistrer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalAnnouncer.Instance.RegisterContainer(gameObject);
    }
}
