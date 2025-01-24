using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : SingletonBase<DataManager>
{
    public SaveSlotData saveSlot;
    public SettingsData settings;

    public PlayerData PlayerData => saveSlot.player;

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

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        string dataPath = Path.Combine(Application.persistentDataPath, "Settings.json");
        File.WriteAllText(dataPath, json, System.Text.Encoding.UTF8);
    }

    public void LoadSaveSlot(int index)
    {
        currentSaveSlotIndex = index;
        string dataPath = Path.Combine(Application.persistentDataPath, string.Format("Save_{0}.json", currentSaveSlotIndex));
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            saveSlot = JsonUtility.FromJson<SaveSlotData>(json);
        }
        else
        {
            Debug.LogError(dataPath + " not exits.");
        }
    }

    public void LoadSettings()
    {
        string dataPath = Path.Combine(Application.persistentDataPath, "Settings.json");
        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            settings = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            Debug.LogError(dataPath + " not exits.");
        }
    }

    public void InitSaveSlot(int index)
    {
        currentSaveSlotIndex = index;
        saveSlot = new SaveSlotData();
        saveSlot.player.maxHealth = 5;
        saveSlot.player.maxMagic = 5;
        saveSlot.player.money = 0;
        saveSlot.player.checkpoint = new Vector2Int(0, 0);
        SaveCurrentSlot();
    }

    public bool CheckSaveSlotExits(int index)
    {
        string dataPath = Path.Combine(Application.persistentDataPath, string.Format("Save_{0}.json", index));
        return File.Exists(dataPath);
    }
}


[System.Serializable]
public class InventoryItem
{
    public string id;
    public int quantity;
}

[System.Serializable]
public class PlayerData
{
    public float maxHealth;
    public float maxMagic;
    public int money;
    public AbilityData ability;
    public Vector2Int checkpoint;
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
    public List<BossSaveData> boss = new();
    public List<CheckpointSaveData> checkpoint = new();
    public List<LevelSaveData> level = new();


    [System.Serializable]
    public class BossSaveData
    {
        public string id;
        public bool dead;
    }

    [System.Serializable]
    public class CheckpointSaveData
    {
        Vector2Int index;
        public bool open;
    }

    [System.Serializable]
    public class LevelSaveData
    {
        public Vector2Int index;
        public bool open;
        // TODO mechanism data list or interactive data list
    }
}

[System.Serializable]
public class SettingsData
{
    public GameSetting game = new();
    public VolumeSetting volume = new();
    public GraphicsSetting graphics = new();
    public InputMap keyboard = new();
    public InputMap controller = new();

    public SettingsData()
    {
        keyboard.Init(GameInput.InputType.Keyoard);
        controller.Init(GameInput.InputType.XboxController);
    }

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


