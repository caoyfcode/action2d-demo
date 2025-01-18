using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShineSpotLight : MonoBehaviour
{
    public Light2D light2D;
    public float minOuterRadius = 0.75f;
    public float maxOuterRadius = 1;
    public float loopTime = 1;
    private float currentPhase = 0;


    // Update is called once per frame
    void Update()
    {
        currentPhase = (currentPhase + Time.deltaTime / loopTime * 2 * Mathf.PI) % (2 * Mathf.PI);
        light2D.pointLightOuterRadius = (minOuterRadius + maxOuterRadius) / 2 + (maxOuterRadius - minOuterRadius) / 2 * Mathf.Sin(currentPhase);
    }
}
