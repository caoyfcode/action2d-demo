using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObject/LevelData", order = 2)]
public class LevelData : ScriptableObject
{
    [Serializable]
    public class Data
    {
        [Tooltip("唯一标识")]
        public string id;
        [Tooltip("(x, y) 中心点坐标, (W, H) 地图宽高, 用于地图绘制")]
        public Rect rect;
        
        public bool hasCheckpoint;
        public Vector2 checkpointPosition;
        public string checkpointName = "默认名称";


        private Sprite mapSprite;
        public Sprite GetMapSprite()
        {
            if (mapSprite == null)
            {
                mapSprite = Resources.Load<Sprite>("LevelMaps/Level_" + id);
            }
            return mapSprite;
        }
    }

    public class CheckpointData
    {
        public string levelId;
        public string name;
        public Vector2 position;
    }

    public List<Data> levelDataList = new();
    [NonSerialized]
    public Dictionary<string, Data> levelDataMap = new();
    [NonSerialized]
    public List<CheckpointData> checkpointList = new();


    void OnEnable()
    {
        levelDataMap.Clear();
        foreach (var item in levelDataList)
        {
            levelDataMap.Add(item.id, item);
            if (item.hasCheckpoint)
            {
                checkpointList.Add(new CheckpointData
                {
                    levelId = item.id,
                    name = item.checkpointName,
                    position = item.checkpointPosition
                });
            }
        }
    }

    public string GetResourcePath(string id)
    {
        return levelDataMap.ContainsKey(id) ? "Level_" + id : null;
    }

    public Vector2 GetCheckpointPosition(string id)
    {
        if (levelDataMap.ContainsKey(id) && levelDataMap[id].hasCheckpoint)
        {
            return levelDataMap[id].checkpointPosition;
        }
        return Vector2.zero;
    }

    public Data GetLevelData(string id)
    {
        levelDataMap.TryGetValue(id, out var data);
        return data;
    }
}
