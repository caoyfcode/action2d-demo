using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 单例，提供公共变量的访问点以及一些游戏系统函数
public class Game : SingletonBase<Game>
{
    public static BlueWitch Player => instance.player;
    public static Camera Camera => instance.cam;
    public static DarkScreen DarkScreen => instance.darkScreen;
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

    void Start()
    {
        Level level = levelContainer.GetComponentInChildren<Level>();
        SetCurrentLevel(level);
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

    // TODO 改名为 StartSwitchLevel
    public static void StartChangeLevel(Vector2Int levelIndex)
    {
        instance.InstanceStartChangeLevel(levelIndex);
    }

    void InstanceStartChangeLevel(Vector2Int levelIndex)
    {
        player.gameObject.SetActive(false);
        StartCoroutine(DarkScreen.WaitForLoading(LoadLevel(levelIndex)));
    }

    IEnumerator LoadLevel(Vector2Int levelIndex)
    {
        string level = LevelPath[levelIndex];
        var request = Resources.LoadAsync(level);
        yield return request;
        // yield return new WaitForSeconds(1);
        Destroy(currentLevel.gameObject);
        SetCurrentLevel(null);
        GameObject prefab = request.asset as GameObject; // TODO 在删除场景时把此资源卸载
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(levelContainer.transform);
        SetCurrentLevel(obj.GetComponent<Level>());
        player.gameObject.SetActive(true);
    }
}
