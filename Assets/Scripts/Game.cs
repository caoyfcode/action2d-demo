using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 单例，提供公共变量的访问点以及一些游戏系统函数
public class Game : MonoBehaviour
{
    static Game instance; // 单例
    public static BlueWitch Player => instance.player;
    public static Camera Camera => instance.cam;
    public static DarkScreen DarkScreen => instance.darkScreen;
    public static Dictionary<Vector2Int, string> LevelPath = new()
    {
        {new Vector2Int(-100, 0), "SampleLevel_0"}, // 用 -100 是为了与正常场景区分开
        {new Vector2Int(-100, 1), "SampleLevel_1"},
    };
    public static Level CurrentLevel => instance.currentLevel;
    public static VirtualCameraController VirtualCameraController => instance.virtualCameraController;
    public static VFXLoader VFXLoader => instance.vfxLoader;

    [SerializeField]
    BlueWitch player;
    [SerializeField]
    Camera cam;
    [SerializeField]
    DarkScreen darkScreen;
    [SerializeField]
    CinemachineConfiner2D machineConfiner;
    [SerializeField]
    VirtualCameraController virtualCameraController;
    [SerializeField]
    Level currentLevel;
    [SerializeField]
    VFXLoader vfxLoader;

    void Start()
    {
        instance = this;
        SetCurrentLevel(currentLevel);
    }

    void SetCurrentLevel(Level level)
    {
        if (level != null)
        {
            currentLevel = level;
            machineConfiner.m_BoundingShape2D = level.GetComponent<Collider2D>();
        }
        else
        {
            currentLevel = null;
            machineConfiner.m_BoundingShape2D = null;
        }
        machineConfiner.InvalidateCache(); // 清除缓存
    }

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
        yield return new WaitForSeconds(1);
        Destroy(currentLevel.gameObject);
        SetCurrentLevel(null);
        GameObject prefab = request.asset as GameObject;
        GameObject obj = Instantiate(prefab);
        SetCurrentLevel(obj.GetComponent<Level>());
        player.gameObject.SetActive(true);
    }
}
