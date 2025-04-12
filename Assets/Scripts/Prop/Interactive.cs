using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactive : MonoBehaviour
{   
    public string id; // 用于在同一 Level 内区分不同的同一类物体
    public Canvas hint;
    public bool playerIn;
    public bool HintShown
    {
        get 
        { 
            if (hint)
                return hint.gameObject.activeSelf;
            else
                return false;
        }
        set 
        {
            if (hint)
                hint.gameObject.SetActive(value);
        }
    }

    protected virtual void Start()
    {
        HintShown = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        playerIn = true;
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        playerIn = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        playerIn = false;
    }
}
