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

    public GamePreload gamePreload;

    public float pauseTimer = 0f;
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
        DataManager.instance.LoadSettings();
    }

    void Update()
    {
        if (debugControlPause)
            Pause = debugPause;
        debugShowFPS = 1 / (Time.realtimeSinceStartup - debugShowFPSLastTime);
        debugShowFPSLastTime = Time.realtimeSinceStartup;

        // PauseForSecons 使用
        if (pauseTimer > 0)
        {
            Pause = true;
            pauseTimer -= Time.unscaledDeltaTime;
            if (pauseTimer <= 0)
            {
                Pause = false;
            }
        }
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
        instance.pauseTimer = seconds;
        Pause = true;
    }

    [Serializable]
    public class GamePreload
    {
        public InputIconData inputIconMap;
        public Material whiteMaterial;
    }

    public void OnApplicationQuit()
    {
        if (!IsMainMenuScene && WorldManager.instance != null)
        {
            if (WorldManager.CurrentLevel != null)
            {
                DestroyImmediate(WorldManager.CurrentLevel.gameObject); // 立即释放场景以执行其中的OnDestory的保存与清理
            }
        }
        Debug.Log("Application quit");       
    }

}
