using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Collider2D triggerArea; // 若为CameraBound，则需要是 PolygonCollider2D 或 CompositeCollider2D
    public TriggerType triggerType;
    public Vector2 cameraPosition;

    public enum TriggerType
    {
        CameraBound,
        CameraPosition,
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerType == TriggerType.CameraBound)
            {
                Game.CameraController.SetBound(triggerArea);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerType == TriggerType.CameraBound)
            {
                Game.CameraController.SetBound(Game.CurrentLevel.bound);
            }
        }
    }
}
