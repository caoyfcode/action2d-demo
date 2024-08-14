using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallexBackground : MonoBehaviour
{
    const float MAX_DEPTH = 50f; // 最大深度处与摄像机相对静止
    float startX, startY;
    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x;
        startY = transform.position.y;
    }

    void Update()
    {
       float factor = Mathf.Clamp01(transform.position.z / MAX_DEPTH);
        transform.position = new Vector3(
            factor * (Game.Camera.transform.position.x - startX) + startX,
            factor * (Game.Camera.transform.position.y - startY) + startY,
            transform.position.z
        );
    }
}
