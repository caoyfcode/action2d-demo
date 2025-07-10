using System;
using UnityEngine;

public abstract class MonoSingletonBase<T>: MonoBehaviour where T : MonoBehaviour
{
    public static T instance {get; protected set;} = null;


    protected virtual void Awake()
    {
        if (instance == null)
            instance = GetComponent<T>();
        else
            Destroy(gameObject);
    }
}

public abstract class SingletonBase<T> where T : class, new()
{
    private static T instance = null;

    public static T Instance
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