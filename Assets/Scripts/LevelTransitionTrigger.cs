using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionTrigger : MonoBehaviour
{
    public Vector2Int targetLevelIndex;
    bool loading = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!loading)
        {
            loading = true;

        }
        Game.StartChangeLevel(targetLevelIndex);
        Debug.Log("trigger enter");
    }

}
