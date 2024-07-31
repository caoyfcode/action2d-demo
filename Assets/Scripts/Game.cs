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

    Level currentLevel;

    void Start()
    {
        instance = this;
    }

    public static void SetCurrentLevel(Level level)
    {
        if (instance == null)
        {
            return;
        }
        instance.currentLevel = level;
        instance.machineConfiner.m_BoundingShape2D = level.bounds;
    }
}
