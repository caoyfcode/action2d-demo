using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Canvas hint;

    void Start()
    {
        hint.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        hint.enabled = true;
        Debug.Log("Player entered checkpoint.");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        hint.enabled = false;
        Debug.Log("Player exited checkpoint.");
    }
}
