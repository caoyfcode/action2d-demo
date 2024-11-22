using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 单例，提供公共变量的访问点以及一些游戏系统函数
public class Game : SingletonBase<Game>
{
    public static BlueWitch Player => instance.player;
    public static Camera Camera => instance.cam;
    public static Dictionary<Vector2Int, string> LevelPath = new()
    {
        {new Vector2Int(-100, 0), "SampleLevel_0"}, // 用 -100 是为了与正常场景区分开
        {new Vector2Int(-100, 1), "SampleLevel_1"},
        {new Vector2Int(0, 0), "Level_00_00"},
        {new Vector2Int(0, -1), "Level_00_m01"},
    };

    public static Level CurrentLevel => instance.currentLevel;
    public static CameraController CameraController => instance.cameraController;
    public static VFXLoader VFXLoader => instance.vfxLoader;
    public static BossSystem BossSystem => instance.bossSystem;
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

    [SerializeField]
    BlueWitch player;
    [SerializeField]
    Camera cam;
    [SerializeField]
    DarkScreen darkScreen;
    [SerializeField]
    CameraController cameraController;
    [SerializeField]
    GameObject levelContainer;
    [SerializeField]
    Level currentLevel;
    [SerializeField]
    VFXLoader vfxLoader;
    [SerializeField]
    BossSystem bossSystem;
    public bool debugControlPause = false; // 手动在 Inspector 里勾选，用于调试
    public bool debugPause = false;
    public float debugShowFPS = 0f;
    float debugShowFPSLastTime = 0f;
    private bool isLoadingLevel = false;

    void Start()
    {
        Level level = levelContainer.GetComponentInChildren<Level>();
        SetCurrentLevel(level);
        // Application.targetFrameRate = 144;
    }

    void Update()
    {
        if (debugControlPause)
            Pause = debugPause;
        debugShowFPS = 1 / (Time.realtimeSinceStartup - debugShowFPSLastTime);
        debugShowFPSLastTime = Time.realtimeSinceStartup;
    }

    void SetCurrentLevel(Level level)
    {
        if (level != null)
        {
            currentLevel = level;
        }
        else
        {
            currentLevel = null;
        }
    }

    public static void PauseForSeconds(float seconds)
    {
        instance.StartCoroutine(instance.PauseForSecondsCoroutine(seconds));
    }

    public static void StartSwitchLevel(Vector2Int levelIndex)
    {
        if (instance.isLoadingLevel)
        {
            return;
        }
        instance.isLoadingLevel = true;
        instance.player.gameObject.SetActive(false);
        instance.StartCoroutine(DarkScreen.instance.WaitForLoading(instance.LoadLevel(levelIndex)));
    }

    IEnumerator LoadLevel(Vector2Int levelIndex)
    {
        string level = LevelPath[levelIndex];
        var request = Resources.LoadAsync(level);
        yield return request;
        Destroy(currentLevel.gameObject);
        SetCurrentLevel(null);
        GameObject prefab = request.asset as GameObject;
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(levelContainer.transform);
        SetCurrentLevel(obj.GetComponent<Level>());
        yield return Resources.UnloadUnusedAssets();
        player.gameObject.SetActive(true);
        isLoadingLevel = false;
    }

    IEnumerator PauseForSecondsCoroutine(float seconds)
    {
        Pause = true;
        float elapsedTime = 0f;
        while (elapsedTime < seconds)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        Pause = false;
    }
}
