using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vm;
    private CinemachineBasicMultiChannelPerlin perlin;
    private float shakeTimer = 0;

    private void Start()
    {
        perlin = vm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        shakeTimer -= Time.unscaledDeltaTime;
        if (shakeTimer <= 0)
        {
            StopShake();
        }
    }

    public void StartShake(float duration = 0.25f, float amplitude = 1f, float frequency = 1f)
    {
        shakeTimer = duration;
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;
    }

    void StopShake()
    {
        shakeTimer = 0;
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }
}
