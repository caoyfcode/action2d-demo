using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoSingletonBase<WorldManager>
{
    public static BlueWitch Player => Ins.player;
    public static Camera Camera => Ins.cam;
    public static CameraController CameraController => Ins.cameraController;
    public static Level CurrentLevel => Ins.currentLevel;

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
        if (DataManager.Ins.CheckSaveSlotExits(0))
        {
            DataManager.Ins.LoadSaveSlot(0);
        }
        else
        {
            DataManager.Ins.InitSaveSlot(0);
        }
        TeleportCheckpoint(DataManager.Ins.PlayerData.checkpoint);
    }

    // save 目的是传送后修改复活点(对于到传送点的传送需要，对于边界传送不需要)
    public static void TeleportCheckpoint(string levelId, bool saveCheckpoint=true)
    {
        if (saveCheckpoint)
        {
            Teleport(levelId, Ins.levelData.GetCheckpointPosition(levelId), true);
            DarkScreen.Ins.DarkScreenWait(() => {
                DataManager.Ins.PlayerData.checkpoint = levelId;
                DataManager.Ins.SaveCurrentSlot();
            });
        }
        else
        {
            Teleport(levelId, Ins.levelData.GetCheckpointPosition(levelId), false);
        }   
    }

    
    public static void Teleport(string levelId, Vector2 position, bool reloadIfSameLevel = false)
    {
        Player.teleportInvincibleTimer = 0.5f;
        GameManager.Pause = true;
        SoundManager.Ins.StopBackgroundMusic();
        DarkScreen.Ins.DarkScreenWait(() => Ins.player.transform.position = position);
        if (Ins.currentLevel && Ins.currentLevel.id == levelId && reloadIfSameLevel) // should reload
        {
            GameObject obj = Ins.currentLevel.gameObject;
            Ins.currentLevel = null;
            DarkScreen.Ins.DarkScreenWait(() => DestroyImmediate(obj));
        }
        if (!Ins.currentLevel || Ins.currentLevel.id != levelId)
        {
            DarkScreen.Ins.DarkScreenWait(Ins.LoadLevel(levelId));
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
            if (DataManager.Ins.CurrentLevelData == null)
            {
                DataManager.Ins.saveSlot.levels.Add(new (){
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
            SoundManager.Ins.PlayBackgroundMusic(currentLevel.bgm);
        }
    }


    public void InventoryAddItem(InventoryItem item)
    {
        if (ItemData.AttackItemIds.Contains(item.id) || ItemData.AbilityItemIds.Contains(item.id))
        {
            DataManager.Ins.PlayerData.ability.SetById(item.id);
            UIManager.Ins.inventoryMenu.inventory.RefreshAttackTable();
        }
        else
        {
            var inventory = DataManager.Ins.PlayerData.inventory;
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
            UIManager.Ins.inventoryMenu.inventory.RefreshItemTable();
        }
        DataManager.Ins.SaveCurrentSlot();
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
                DataManager.Ins.PlayerData.controlConfig.attack1 = id;
                if (DataManager.Ins.PlayerData.controlConfig.attack2 == id)
                    DataManager.Ins.PlayerData.controlConfig.attack2 = "";
                if (DataManager.Ins.PlayerData.controlConfig.attack3 == id)
                    DataManager.Ins.PlayerData.controlConfig.attack3 = "";
                UIManager.Ins.inventoryMenu.inventory.RefreshAttackConfig();
                break;
            case 1:
                DataManager.Ins.PlayerData.controlConfig.attack2 = id;
                if (DataManager.Ins.PlayerData.controlConfig.attack1 == id)
                    DataManager.Ins.PlayerData.controlConfig.attack1 = "";
                if (DataManager.Ins.PlayerData.controlConfig.attack3 == id)
                    DataManager.Ins.PlayerData.controlConfig.attack3 = "";
                UIManager.Ins.inventoryMenu.inventory.RefreshAttackConfig();
                break;
            case 2:
                DataManager.Ins.PlayerData.controlConfig.attack3 = id;
                if (DataManager.Ins.PlayerData.controlConfig.attack1 == id)
                    DataManager.Ins.PlayerData.controlConfig.attack1 = "";
                if (DataManager.Ins.PlayerData.controlConfig.attack2 == id)
                    DataManager.Ins.PlayerData.controlConfig.attack2 = "";
                UIManager.Ins.inventoryMenu.inventory.RefreshAttackConfig();
                break;
            default:
                Debug.LogError("Invalid attack index: " + attackIndex);
                break;
        }
        DataManager.Ins.SaveCurrentSlot();
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
