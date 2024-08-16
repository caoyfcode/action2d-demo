using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // TODO Level 类与 currentLevel, 并且内部有矩形表示摄像机边界

    [SerializeField]
    BlueWitch player;
    [SerializeField]
    Camera cam;
    [SerializeField]
    DarkScreen darkScreen;
    [SerializeField]
    CinemachineConfiner2D machineConfiner;
    [SerializeField]
    Level currentLevel;

    void Start()
    {
        instance = this;
        SetCurrentLevel(currentLevel);
    }

    public static void StartChangeLevel(Vector2Int levelIndex)
    {
        instance.StartCoroutine(DarkScreen.WaitForLoading(instance.ChangeLevel(levelIndex)));
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

    IEnumerator ChangeLevel(Vector2Int levelIndex)
    {
        Destroy(currentLevel.gameObject);
        SetCurrentLevel(null);
        player.gameObject.SetActive(false);
        string level = LevelPath[levelIndex];
        var request = Resources.LoadAsync(level);
        yield return request;
        GameObject prefab = request.asset as GameObject;
        GameObject obj = Instantiate(prefab);
        SetCurrentLevel(obj.GetComponent<Level>());
        player.gameObject.SetActive(true);
    }
}
