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

    // TODO 迁移到 PreloadResource
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

    // save 目的是传送后修改复活点(对于到传送点的传送需要，对于边界传送不需要)
    public static void TeleportCheckpoint(string levelId, bool saveCheckpoint=true)
    {
        if (saveCheckpoint)
        {
            if (!instance.currentLevel || instance.currentLevel.id != levelId)
            {
                Teleport(levelId, instance.levelData.GetCheckpointPosition(levelId));
            }
            else
            {
                ReloadCurrentLevel();
            }
            DarkScreen.instance.DarkScreenWait(() => {
                DataManager.instance.PlayerData.checkpoint = levelId;
                DataManager.instance.SaveCurrentSlot();
            });
        }
        else
        {
            Teleport(levelId, instance.levelData.GetCheckpointPosition(levelId));
        }   
    }

    
    public static void Teleport(string levelId, Vector2 position)
    {
        Player.teleportInvincibleTimer = 0.5f;
        GameManager.Pause = true;
        SoundManager.instance.StopBackgroundMusic();
        DarkScreen.instance.DarkScreenWait(() => instance.player.transform.position = position);
        if (!instance.currentLevel || instance.currentLevel.id != levelId)
        {
            DarkScreen.instance.DarkScreenWait(instance.LoadLevel(levelId));
        }
    }

    // 与 Teleport 非常相似
    public static void ReloadCurrentLevel()
    {
        if (instance.currentLevel)
        {
            Player.teleportInvincibleTimer = 0.5f;
            GameManager.Pause = true;
            SoundManager.instance.StopBackgroundMusic();
            string id = instance.currentLevel.id;
            GameObject obj = instance.currentLevel.gameObject;
            instance.currentLevel = null;
            DarkScreen.instance.DarkScreenWait(() => DestroyImmediate(obj));
            DarkScreen.instance.DarkScreenWait(instance.LoadLevel(id));
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
            if (DataManager.instance.CurrentLevelData == null)
            {
                DataManager.instance.saveSlot.levels.Add(new (){
                    id = levelId,
                });
            }
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
        if (ItemData.AttackItemIds.Contains(item.id) || ItemData.AbilityItemIds.Contains(item.id))
        {
            DataManager.instance.PlayerData.ability.SetById(item.id);
            UIManager.instance.inventoryMenu.inventory.RefreshAttackTable();
        }
        else
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
        DataManager.instance.SaveCurrentSlot();
    }

    public void ChangeAttackConfig(int attackIndex, string id)
    {
        if (!ItemData.AttackItemIds.Contains(id))
        {
            Debug.LogError("Not an attack item: id=" + id);
            return;
        }
        switch (attackIndex)
        {
            case 0:
                DataManager.instance.PlayerData.controlConfig.attack1 = id;
                if (DataManager.instance.PlayerData.controlConfig.attack2 == id)
                    DataManager.instance.PlayerData.controlConfig.attack2 = "";
                if (DataManager.instance.PlayerData.controlConfig.attack3 == id)
                    DataManager.instance.PlayerData.controlConfig.attack3 = "";
                UIManager.instance.inventoryMenu.inventory.RefreshAttackConfig();
                break;
            case 1:
                DataManager.instance.PlayerData.controlConfig.attack2 = id;
                if (DataManager.instance.PlayerData.controlConfig.attack1 == id)
                    DataManager.instance.PlayerData.controlConfig.attack1 = "";
                if (DataManager.instance.PlayerData.controlConfig.attack3 == id)
                    DataManager.instance.PlayerData.controlConfig.attack3 = "";
                UIManager.instance.inventoryMenu.inventory.RefreshAttackConfig();
                break;
            case 2:
                DataManager.instance.PlayerData.controlConfig.attack3 = id;
                if (DataManager.instance.PlayerData.controlConfig.attack1 == id)
                    DataManager.instance.PlayerData.controlConfig.attack1 = "";
                if (DataManager.instance.PlayerData.controlConfig.attack2 == id)
                    DataManager.instance.PlayerData.controlConfig.attack2 = "";
                UIManager.instance.inventoryMenu.inventory.RefreshAttackConfig();
                break;
            default:
                Debug.LogError("Invalid attack index: " + attackIndex);
                break;
        }
        DataManager.instance.SaveCurrentSlot();
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
