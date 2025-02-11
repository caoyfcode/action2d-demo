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
        // TODO 在完成主场景后将
        if (DataManager.instance.CheckSaveSlotExits(0))
        {
            DataManager.instance.LoadSaveSlot(0);
        }
        else
        {
            DataManager.instance.InitSaveSlot(0);
        }
        // Game.instance.isMainMenuScene = false;
        Level level = levelContainer.GetComponentInChildren<Level>();
        if (level != null)
        {
            currentLevel = level;
            cameraController.FitRoomSize();
            cameraController.SetRoomTriggerTarget();
            cameraController.SetBoundariesAndMoveInstantly();
        }
        // else
        // {
        //     Teleport(new(0, 0));
        // }
        TeleportCheckpoint(DataManager.instance.PlayerData.checkpoint);
    }

    public static void TeleportCheckpoint(Vector2Int levelIndex, bool save=true)
    {
        Teleport(levelIndex, instance.levelData.GetCheckpointPosition(levelIndex));
        if (save)
            DarkScreen.instance.DarkScreenWait(() => {
                DataManager.instance.PlayerData.checkpoint = levelIndex;
                DataManager.instance.SaveCurrentSlot();
            });
    }

    public static void Teleport(Vector2Int levelIndex, Vector2 position)
    {
        GameManager.Pause = true;
        SoundManager.instance.StopBackgroundMusic();
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
        if (currentLevel && currentLevel.bgm != null)
        {
            SoundManager.instance.PlayBackgroundMusic(currentLevel.bgm);
        }
    }

    // [Serializable]
    // public class Preload
    // {
    //     public LevelData levelData;
    //     public ItemData itemData;
    //     public GameObject smokeRingPrefab;
    //     public GameObject afterImagePrefab;
    //     public GameObject bloodEffectPrefab;
    //     public GameObject explosionPrefab;
    // }
}
