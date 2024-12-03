using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TDOD 改为 UIBase 或 ShownUI
public abstract class Menu : MonoBehaviour
{
    public virtual bool Shown
    {
        get { return gameObject.activeSelf; }
        set
        {
            gameObject.SetActive(value);
        }
    }
}
