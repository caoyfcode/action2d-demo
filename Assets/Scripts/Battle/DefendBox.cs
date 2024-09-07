using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DefendBox : MonoBehaviour
{
    public Entity entity;
    public bool isDefending;

    public Collider2D defendBox;
    internal Action<HitInfo> onSuccess;

    protected virtual void Start()
    {
        isDefending = false;
    }

    public void StartDefend()
    {
        isDefending = true;
    }

    public void StopDefend()
    {
        isDefending = false;
    }

    public void OnAttacked(HitInfo hitInfo)
    {
        onSuccess?.Invoke(hitInfo);
    }
}