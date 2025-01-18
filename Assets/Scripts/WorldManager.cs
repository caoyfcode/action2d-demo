using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : SingletonBase<WorldManager>
{
    public static BlueWitch Player => instance.player;
    public static Camera Camera => instance.cam;
    public static CameraController CameraController => instance.cameraController;
    public static Level CurrentLevel => instance.currentLevel;

    public BlueWitch player;
    public Camera cam;
    public CameraController cameraController;
    public GameObject levelContainer;
    public BossSystem bossSystem;

    public LevelData levelData;
    public ItemData itemData;

    public Level currentLevel;

    private bool isLoadingLevel = false;

    void Start()
    {
        // Game.instance.isMainMenuScene = false;
        Level level = levelContainer.GetComponentInChildren<Level>();
        if (level != null)
        {
            currentLevel = level;
            cameraController.FitRoomSize();
            cameraController.SetRoomTriggerTarget();
            cameraController.SetBoundariesAndMoveInstantly();
        }
        else
        {
            Teleport(new(0, 0), levelData.GetCheckpointPosition(new(0, 0)));
        }
    }

    public static void Teleport(Vector2Int levelIndex, Vector2 position)
    {
        GameManager.Pause = true;
        DarkScreen.instance.DarkScreenWait(() => instance.player.transform.position = position);
        if (!instance.currentLevel || instance.currentLevel.levelIndex != levelIndex)
        {
            DarkScreen.instance.DarkScreenWait(instance.LoadLevel(levelIndex));
        }
    }
    
    // 为了不发生冲突, 此协程只能通过 DarkScreen 启动
    IEnumerator LoadLevel(Vector2Int levelIndex)
    {
        GameManager.Pause = true;
        string levelPath = levelData.GetResourcePath(levelIndex);
        if (levelPath != null && !isLoadingLevel)
        {
            isLoadingLevel = true;
            var request = Resources.LoadAsync(levelPath);
            yield return request;
            if (currentLevel != null)
                Destroy(currentLevel.gameObject);
            currentLevel = null;
            GameObject prefab = request.asset as GameObject;
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(levelContainer.transform);
            currentLevel = obj.GetComponent<Level>();
            yield return Resources.UnloadUnusedAssets();
        }
        else
        {
            Debug.LogError("Level not found: " + levelIndex);
        }
        isLoadingLevel = false;
        GameManager.Pause = false;
        cameraController.FitRoomSize();
        cameraController.SetRoomTriggerTarget();
        cameraController.SetBoundariesAndMoveInstantly();
    }
}
