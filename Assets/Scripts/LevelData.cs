using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObject/LevelData", order = 2)]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        [Tooltip("整数坐标作为唯一索引")]
        public Vector2Int index;
        [Tooltip("关卡预制件的资源路径, 用于加载")]
        public string resourcePath;
        [Tooltip("(x, y) 中心点坐标, (W, H) 地图宽高, 用于地图绘制")]
        public Rect rect;
        [Tooltip("地图块的 Sprite, 用于地图绘制")]
        public Sprite mapSprite;
        public bool hasCheckpoint;
        public Vector2 checkpointPosition;
        public string checkpointName = "默认名称";
    }

    public class CheckpointData
    {
        public Vector2Int index;
        public string name;
        public Vector2 position;
    }

    public List<Data> levelDataList = new();
    [System.NonSerialized]
    public Dictionary<Vector2Int, Data> levelDataMap = new();
    [System.NonSerialized]
    public List<CheckpointData> checkpointList = new();


    void OnEnable()
    {
        levelDataMap.Clear();
        foreach (var item in levelDataList)
        {
            levelDataMap.Add(item.index, item);
            if (item.hasCheckpoint)
            {
                checkpointList.Add(new CheckpointData
                {
                    index = item.index,
                    name = item.checkpointName,
                    position = item.checkpointPosition
                });
            }
        }
    }

    public string GetResourcePath(Vector2Int index)
    {
        return levelDataMap.ContainsKey(index) ? levelDataMap[index].resourcePath : null;
    }

    public Vector2 GetCheckpointPosition(Vector2Int index)
    {
        if (levelDataMap.ContainsKey(index) && levelDataMap[index].hasCheckpoint)
        {
            return levelDataMap[index].checkpointPosition;
        }
        return Vector2.zero;
    }
}
