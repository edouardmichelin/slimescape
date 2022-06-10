using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerTextRegistrer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalAnnouncer.Instance.RegisterTextComponent(gameObject);
    }
}
