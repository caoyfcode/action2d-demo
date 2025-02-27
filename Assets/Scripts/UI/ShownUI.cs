using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 所有可隐藏的UI都继承这个类
public abstract class ShownUI : MonoBehaviour
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
