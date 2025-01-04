using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapData mapData;
    public GameObject mapPiecePrefab;
    public float resetScale = 5f;
    private Dictionary<Vector2Int, MapPiece> piceces = new();
    const float moveSpeed = 600f; // 600pix/s
    const float maxMoveSpeed = 1000f;

    public float Scale
    {
        get => transform.localScale.x;
        set => transform.localScale = new Vector3(value, value, 1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (piceces.Count == 0)
        {
            foreach (var piece in mapData.mapPieceDataList)
            {
                var obj = Instantiate(mapPiecePrefab, new Vector3(piece.mapRect.x, piece.mapRect.y, 0), Quaternion.identity, transform);
                var comp = obj.GetComponent<MapPiece>();
                comp.backRectTransform.sizeDelta = new Vector2(piece.mapRect.width, piece.mapRect.height);
                // comp.foreRectTransform.sizeDelta = new Vector2(piece.mapRect.width, piece.mapRect.height);
                comp.foreground.sprite = piece.sprite;
                comp.backRectTransform.localPosition = new Vector3(piece.mapRect.x, piece.mapRect.y, 0);
                piceces.Add(piece.index, comp);
            }
        }
        ResetScale();
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
        get => -transform.position;
        set => transform.position = -value;
    }

    public void Move(Vector2 direction, float speed=moveSpeed)
    {
        direction.Normalize();
        CenterPosition += speed * Time.deltaTime * direction;
    }

    public void MoveTo(Vector2 targetPos)
    {
        Vector2 diff = targetPos - CenterPosition;
        if (diff.magnitude < 10f)
        {
            CenterPosition = targetPos;
        }
        else
        {
            Move(diff.normalized, Mathf.Clamp01(diff.magnitude / 1000) * (maxMoveSpeed - moveSpeed) + moveSpeed);
        }
    }

    public void ChangeScale(float scaleSpeed)
    {
        Scale += scaleSpeed * Time.deltaTime;
    }
}
