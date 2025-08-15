using System;
using UnityEngine;

public abstract class MonoSingletonBase<T>: MonoBehaviour where T : MonoBehaviour
{
    public static T Ins {get; protected set;} = null;


    protected virtual void Awake()
    {
        if (Ins == null)
            Ins = GetComponent<T>();
        else
            Destroy(gameObject);
    }
}

public abstract class SingletonBase<T> where T : class, new()
{
    private static T instance = null;

    public static T Ins
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}