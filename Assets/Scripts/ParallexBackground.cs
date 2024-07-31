using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallexBackground : MonoBehaviour
{
    float startX, startY;
    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x;
        startY = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(
            (Game.Camera.transform.position.x - startX) / (1 + transform.position.z) + startX,
            (Game.Camera.transform.position.y - startY) / (1 + transform.position.z) + startY,
            transform.position.z
        );
    }
}
