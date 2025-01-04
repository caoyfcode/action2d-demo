using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObject/MapData", order = 2)]
public class MapData : ScriptableObject
{
    [System.Serializable]
    public class MapPieceData
    {
        [Tooltip("唯一索引")]
        public Vector2Int index;
        [Tooltip("(x, y) 中心点坐标, (W, H) 地图宽高")]
        public Rect mapRect;
        public Sprite sprite;
    }

    public List<MapPieceData> mapPieceDataList = new();
    public Dictionary<Vector2Int, MapPieceData> mapPieces = new();

    void OnEnable()
    {
        foreach (var item in mapPieceDataList)
        {
            mapPieces.Add(item.index, item);
        }
    }
}
