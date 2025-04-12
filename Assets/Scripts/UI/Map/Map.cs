using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO 改为 MapUI
public class Map : MonoBehaviour
{
    public GameObject mapPiecePrefab;
    public float resetScale = 5f;
    private Dictionary<string, MapPiece> piceces = new();
    const float moveSpeed = 600f; // 600pix/s
    const float maxMoveSpeed = 1000f;

    public float Scale
    {
        get => transform.localScale.x;
        set => transform.localScale = new Vector3(value, value, 1f);
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     if (piceces.Count == 0)
    //     {
    //         foreach (var level in WorldManager.instance.levelData.levelDataList)
    //         {
    //             var obj = Instantiate(mapPiecePrefab, transform);
    //             var comp = obj.GetComponent<MapPiece>();
    //             comp.backRectTransform.sizeDelta = new Vector2(level.rect.width, level.rect.height);
    //             comp.foreground.sprite = level.GetMapSprite();
    //             comp.backRectTransform.localPosition = new Vector3(level.rect.x, level.rect.y, 0);
    //             piceces.Add(level.id, comp);
    //         }
    //     }
    //     ResetScale();
    // }

    void OnEnable()
    {
        RefreshMapPiceces();
        ResetScale();
        WorldPosition = WorldManager.Player.transform.position;
    }

    public void ResetScale()
    {
        Scale = resetScale;
    }
    public void ResetPosition()
    {
        CenterPosition = Vector2.zero;
    }

    public Vector2 CenterPosition
    {
        get => -transform.localPosition / Scale;
        set => transform.localPosition = -value * Scale;
    }

    public Vector2 WorldPosition
    {
        get => -transform.localPosition * 10;
        set => transform.localPosition = -value / 10;
    }

    // 移动速度单位为屏幕像素
    public void Move(Vector2 direction, float speed=moveSpeed)
    {
        direction.Normalize();
        CenterPosition += speed / Scale * Time.deltaTime * direction;
    }

    // 移动速度单位为地图中单位1
    public void MoveTo(Vector2 targetPos)
    {
        Vector2 diff = targetPos - CenterPosition;
        if (diff.magnitude < 10f)
        {
            CenterPosition = targetPos;
        }
        else
        {
            Move(diff.normalized, Scale * (Mathf.Clamp01(diff.magnitude / 1000) * (maxMoveSpeed - moveSpeed) + moveSpeed));
        }
    }


    public void ChangeScale(float scaleSpeed)
    {
        Scale += scaleSpeed * Time.deltaTime;
    }

    void RefreshMapPiceces()
    {
        List<string> openMaps = new();
        foreach (var level in DataManager.instance.saveSlot.levels)
        {
            if (level.open)
            {
                openMaps.Add(level.id);
            }
        }
        int childCount = transform.childCount;
        for (int i = 0; i < openMaps.Count && i < childCount; i++)
        {
            var child = transform.GetChild(i);
            var piece = child.GetComponent<MapPiece>();
            var levelData = WorldManager.instance.levelData.GetLevelData(openMaps[i]);
            piece.backRectTransform.sizeDelta = new Vector2(levelData.rect.width, levelData.rect.height);
            piece.foreground.sprite = levelData.GetMapSprite();
            piece.backRectTransform.localPosition = new Vector3(levelData.rect.x, levelData.rect.y, 0);
        }
        for (int i = childCount; i < openMaps.Count; i++)
        {
            var obj = Instantiate(mapPiecePrefab, transform);
            var piece = obj.GetComponent<MapPiece>();
            var levelData = WorldManager.instance.levelData.GetLevelData(openMaps[i]);
            piece.backRectTransform.sizeDelta = new Vector2(levelData.rect.width, levelData.rect.height);
            piece.foreground.sprite = levelData.GetMapSprite();
            piece.backRectTransform.localPosition = new Vector3(levelData.rect.x, levelData.rect.y, 0);
        }
        for (int i = openMaps.Count; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
