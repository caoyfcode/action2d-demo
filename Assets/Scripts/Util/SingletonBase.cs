using System;
using UnityEngine;

public class SingletonBase<T>: MonoBehaviour where T : MonoBehaviour
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