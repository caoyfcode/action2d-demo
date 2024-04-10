using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfBehaviour : MonoBehaviour
{
    public bool destroy = false;

    // Update is called once per frame
    void Update()
    {
        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
