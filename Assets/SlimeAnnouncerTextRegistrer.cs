using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnnouncerTextRegistrer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SlimeAnnouncer.Instance.RegisterTextComponent(gameObject);
    }
}

