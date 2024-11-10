using System;
using UnityEngine;

public class SingletonBase<T>: MonoBehaviour where T : MonoBehaviour
{
    public static T instance {get; private set;}


    protected virtual void Awake()
    {
        instance = GetComponent<T>();
    }
}