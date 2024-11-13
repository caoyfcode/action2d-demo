using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.rotation = Quaternion.identity;
    }
}
