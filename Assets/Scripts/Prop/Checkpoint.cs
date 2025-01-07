using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Canvas hint;
    public bool playerInCheckpoint = false;

    void Start()
    {
        hint.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInCheckpoint)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                playerInCheckpoint = false;
                hint.gameObject.SetActive(false);
                UIManager.CheckpointMenu.Shown = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        hint.gameObject.SetActive(true);
        playerInCheckpoint = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        hint.gameObject.SetActive(false);
        playerInCheckpoint = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        hint.gameObject.SetActive(true);
        playerInCheckpoint = true;
    }
}
