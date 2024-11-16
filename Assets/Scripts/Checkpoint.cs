using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Canvas hint;
    public bool playerInCheckpoint = false;

    void Start()
    {
        hint.enabled = false;
    }

    void Update()
    {
        if (playerInCheckpoint)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Game.Player.data.heath = Game.Player.data.maxHeath;
                Game.Player.data.magic = Game.Player.data.maxMagic;
                playerInCheckpoint = false;
                hint.enabled = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        hint.enabled = true;
        playerInCheckpoint = true;
        Debug.Log("Player entered checkpoint.");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        hint.enabled = false;
        playerInCheckpoint = false;
        Debug.Log("Player exited checkpoint.");
    }
}
