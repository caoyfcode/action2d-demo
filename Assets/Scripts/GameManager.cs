using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// 单例，提供公共变量的访问点以及一些游戏系统函数
public class GameManager : SingletonBase<GameManager>
{
    public static bool Pause
    {
        get => Time.timeScale == 0;
        set
        {
            if (value)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public static bool IsMainMenuScene => MainMenu.instance != null;

    public bool debugControlPause = false; // 手动在 Inspector 里勾选，用于调试
    public bool debugPause = false;
    public float debugShowFPS = 0f;
    float debugShowFPSLastTime = 0f;
    // public bool isMainMenuScene = true;
    private bool isLoadingLevel = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(instance);
    }

    void Start()
    {
        // Application.targetFrameRate = 144;
        // isMainMenuScene = true;
    }

    void Update()
    {
        if (debugControlPause)
            Pause = debugPause;
        debugShowFPS = 1 / (Time.realtimeSinceStartup - debugShowFPSLastTime);
        debugShowFPSLastTime = Time.realtimeSinceStartup;
    }

    public static void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public static void PauseForSeconds(float seconds)
    {
        instance.StartCoroutine(instance.PauseForSecondsCoroutine(seconds));
    }


    IEnumerator PauseForSecondsCoroutine(float seconds)
    {
        Pause = true;
        yield return new WaitForSecondsRealtime(seconds);
        Pause = false;
    }

    // [Serializable]
    // public class GamePreload
    // {
    //     public InputIconMap inputIconMap;
    //     public Material whiteMaterial;
    // }
}
