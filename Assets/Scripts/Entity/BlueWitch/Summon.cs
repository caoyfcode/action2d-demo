using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    public string[] animNames;

    public SpriteRenderer sr;
    public Animator animator;

    [HideInInspector]
    public BlueWitch player;

    #region Status
    public bool isEnd = false;
    public bool isCancelable = true;
    public float comboTimer = 0;
    [SerializeField]
    protected int nextIndex = 0;
    #endregion

    protected virtual bool ComponentsEnabled
    {
        set
        {
            animator.enabled = value;
            sr.enabled = value;
        }
    }

    protected virtual void Awake()
    {
        player = GetComponentInParent<BlueWitch>();
    }

    protected virtual void Start()
    {
        ComponentsEnabled = false;
    }

    protected virtual void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            comboTimer = 0;
        }
    }

    public virtual void StartSummon()
    {
        if (comboTimer <= 0)
        {
            nextIndex = 0;
        }
        ComponentsEnabled = true;
        animator.Play(animNames[nextIndex]);
        nextIndex = (nextIndex + 1) % animNames.Length;
        isCancelable = true;
        isEnd = false;
    }

    public virtual void OnSummonEnd()
    {
        ComponentsEnabled = false;
        isEnd = true;
        comboTimer = player.data.attackComboTime;
    }
}
