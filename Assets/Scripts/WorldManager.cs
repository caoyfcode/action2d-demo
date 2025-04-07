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
        TeleportCheckpoint(DataManager.instance.PlayerData.checkpoint);
    }

    public static void TeleportCheckpoint(string levelId, bool save=true)
    {
        Teleport(levelId, instance.levelData.GetCheckpointPosition(levelId));
        if (save)
            DarkScreen.instance.DarkScreenWait(() => {
                DataManager.instance.PlayerData.checkpoint = levelId;
                DataManager.instance.SaveCurrentSlot();
            });
    }

    public static void Teleport(string levelId, Vector2 position)
    {
        GameManager.Pause = true;
        SoundManager.instance.StopBackgroundMusic();
        DarkScreen.instance.DarkScreenWait(() => instance.player.transform.position = position);
        if (!instance.currentLevel || instance.currentLevel.id != levelId)
        {
            DarkScreen.instance.DarkScreenWait(instance.LoadLevel(levelId));
        }
    }
    
    // 为了不发生冲突, 此协程只能通过 DarkScreen 启动
    IEnumerator LoadLevel(string levelId)
    {
        GameManager.Pause = true;
        string levelPath = levelData.GetResourcePath(levelId);
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
            currentLevel.id = levelId;
            yield return Resources.UnloadUnusedAssets();
        }
        else
        {
            Debug.LogError("Level not found: " + levelId);
        }
        isLoadingLevel = false;
        GameManager.Pause = false;
        yield return null; // 将后面设置推迟至帧末，保证新加载的对象的 Start 的调用
        cameraController.FitRoomSize();
        cameraController.SetRoomTriggerTarget();
        cameraController.MoveInstantlyAndSetBoundaries();
        if (currentLevel && currentLevel.bgm != null)
        {
            SoundManager.instance.PlayBackgroundMusic(currentLevel.bgm);
        }
    }


    public void InventoryAddItem(InventoryItem item)
    {
        var inventory = DataManager.instance.PlayerData.inventory;
        bool found = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == item.id)
            {
                inventory[i].quantity += item.quantity;
                found = true;
                break;
            }
        }
        if (!found)
        {
            inventory.Add(item);
        }
        UIManager.instance.inventoryMenu.inventory.RefreshItemTable();
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
