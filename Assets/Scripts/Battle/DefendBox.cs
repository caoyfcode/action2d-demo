using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DefendBox : MonoBehaviour
{
    public Entity entity;
    private bool isDefending;

    public Collider2D defendCollider;
    public Action<HitInfo> onSuccess;
    public Func<HitInfo, bool> defendCheck;

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

    public void Defend(HitInfo hitInfo)
    {
        onSuccess?.Invoke(hitInfo);
    }

    public bool DefendCheck(HitInfo hitInfo)
    {
        return isDefending && (defendCheck?.Invoke(hitInfo) ?? true);
    }
}