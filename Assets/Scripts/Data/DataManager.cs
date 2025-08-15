using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// TODO 重命名为 SaveManager/SaveDataManager
public class DataManager : MonoSingletonBase<DataManager>
{
    public SaveSlotData saveSlot;
    public SettingsData settings;
    private static int _newInstanceId = 0; // 用于生成唯一 instanceId, 目前只用在动态生成的拾取物
    public static int NewInstanceId()
    {
        return _newInstanceId++;
    }
    

    public PlayerData PlayerData => saveSlot.player;
    public SaveSlotData.LevelSaveData CurrentLevelData
    {
        get
        {
            if (WorldManager.CurrentLevel != null)
            {
                return saveSlot.levels.Find(x => x.id == WorldManager.CurrentLevel.id);
            }
            return null;
        }
    }

    private int currentSaveSlotIndex = 0;

    void Start()
    {
        string json = JsonUtility.ToJson(saveSlot.player);
        Debug.Log(json);
    }

    public void SaveCurrentSlot()
    {
        string json = JsonUtility.ToJson(saveSlot, true);
        string dataPath = Path.Combine(Application.persistentDataPath, string.Format("Save_{0}.json", currentSaveSlotIndex));
        File.WriteAllText(dataPath, json, System.Text.Encoding.UTF8);
        Debug.Log("save to " + dataPath);
    }

    public void LoadSaveSlot(int index)
    {
        currentSaveSlotIndex = index;
        string dataPath = Path.Combine(Application.persistentDataPath, string.Format("Save_{0}.json", currentSaveSlotIndex));
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            saveSlot = JsonUtility.FromJson<SaveSlotData>(json);
            _newInstanceId = 0;
            foreach (var pickup in saveSlot.dynamicPickupItems)
            {
                if (pickup.instanceId >= _newInstanceId)
                {
                    _newInstanceId = pickup.instanceId + 1;
                }
            }
        }
        else
        {
            Debug.LogError(dataPath + " not exits.");
        }
    }

    public SaveSlotData.BossSaveData GetBossData(string entityId)
    {
        return saveSlot.bosses.Find(x => x.id== entityId);
    }

    public void SaveBossData(string entityId, bool dead)
    {
        var bossData = GetBossData(entityId);
        if (bossData == null)
        {
            bossData = new()
            {
                id = entityId
            };
            saveSlot.bosses.Add(bossData);
        }
        bossData.dead = dead;
        SaveCurrentSlot();
    }

    public void LoadSettings()
    {
        string dataPath = Path.Combine(Application.persistentDataPath, "Settings.json");
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            settings = JsonUtility.FromJson<SettingsData>(json);
            GameInputManager.Ins.LoadSettings(settings.keyboardSettings, settings.controllerSettings);
        }
        else
        {
            Debug.LogError(dataPath + " not exits.");
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        string dataPath = Path.Combine(Application.persistentDataPath, "Settings.json");
        settings.keyboardSettings = GameInputManager.Ins.keyboardSettings;
        settings.controllerSettings = GameInputManager.Ins.controllerSettings;
        File.WriteAllText(dataPath, json, System.Text.Encoding.UTF8);
    }

    public void InitSaveSlot(int index)
    {
        currentSaveSlotIndex = index;
        saveSlot = new SaveSlotData();
        saveSlot.player.maxHealth = 5;
        saveSlot.player.maxMagic = 5;
        saveSlot.player.money = 0;
        saveSlot.player.checkpoint = "001";
        SaveCurrentSlot();
    }

    public bool CheckSaveSlotExits(int index)
    {
        string dataPath = Path.Combine(Application.persistentDataPath, string.Format("Save_{0}.json", index));
        return File.Exists(dataPath);
    }
}


[System.Serializable]
public class PlayerData
{
    public float maxHealth;
    public float maxMagic;
    public int money;
    public AbilityData ability;
    public string checkpoint;
    public ControlConfigData controlConfig;
    public List<InventoryItem> inventory;


    [System.Serializable]
    public class AbilityData
    {
        // skill
        public bool attack;
        public bool defend;
        public bool arrow;
        // ability
        public bool dash;
        public bool doubleJump;
        public bool wallSlide;

        public void SetById(string id, bool value=true)
        {
            switch (id)
            {
                case ItemData.HeroKnightItemId:
                    attack = value;
                    break;
                case ItemData.FireKnightItemId:
                    defend = value;
                    break;
                case ItemData.ArcaneArcherItemId:
                    arrow = value;
                    break;
                case ItemData.DashItemId:
                    dash = value;
                    break;
                case ItemData.DoubleJumpItemId:
                    doubleJump = value;
                    break;
                case ItemData.WallSlideItemId:
                    wallSlide = value;
                    break;
            }
        }

        public List<InventoryItem> GetAttackItems()
        {
            List<InventoryItem> items = new();
            if (attack)
            {
                items.Add(new InventoryItem { id = ItemData.HeroKnightItemId, quantity = 1 });
            }
            if (arrow)
            {
                items.Add(new InventoryItem { id = ItemData.ArcaneArcherItemId, quantity = 1 });
            }
            if (defend)
            {
                items.Add(new InventoryItem { id = ItemData.FireKnightItemId, quantity = 1 });
            }
            // todo golem
            return items;
        }

        public List<InventoryItem> GetAbilityItems()
        {
            List<InventoryItem> items = new();
            if (dash)
            {
                items.Add(new InventoryItem { id = ItemData.DashItemId, quantity = 1 });
            }
            if (doubleJump)
            {
                items.Add(new InventoryItem { id = ItemData.DoubleJumpItemId, quantity = 1 });
            }
            if (wallSlide)
            {
                items.Add(new InventoryItem { id = ItemData.WallSlideItemId, quantity = 1 });
            }
            return items;
        }
    }

    [System.Serializable]
    public class ControlConfigData
    {
        public string attack1; // id
        public string attack2; // id
        public string attack3; // id
    }
}

[System.Serializable]
public class SaveSlotData
{
    public PlayerData player = new();
    public List<BossSaveData> bosses = new();
    public List<CheckpointSaveData> checkpoints = new();
    public List<LevelSaveData> levels = new();
    public List<DynamicPickupItemSaveData> dynamicPickupItems = new(); // 动态生成的拾取物，譬如怪物掉落等，在 Level 处统一进行对象的生成与删除，当拾取后删除项

    [System.Serializable]
    public class BossSaveData
    {
        public string id;
        public bool dead;
    }

    [System.Serializable]
    public class CheckpointSaveData
    {
        public string id;
        public bool open;
    }

    [System.Serializable]
    public class LevelSaveData
    {
        public string id;
        public bool open;
        public List<LevelPickupItemSaveData> pickupItems = new();
        public List<LevelDoorSaveData> doors = new();
    }
}

[System.Serializable]
public class SettingsData
{
    public GameSetting game = new();
    public VolumeSetting volume = new();
    public GraphicsSetting graphics = new();
    public KeyboardSettings keyboardSettings = new();
    public ControllerSettings controllerSettings = new();

    [System.Serializable]
    public class GameSetting
    {
        // public string language;
        public int fps = 60;
    }

    [System.Serializable]
    public class VolumeSetting
    {
        public float totalVolume = 1;
        public float backgroundVolume = 1;
        public float effectVolume = 1;
    }

    [System.Serializable]
    public class GraphicsSetting
    {
        public string resolution = "1920x1080";
    }

}


