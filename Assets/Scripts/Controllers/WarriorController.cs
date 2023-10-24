using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            Debug.Log("Attack Down");
            animator.SetTrigger("Attack");
        }

        var move = Input.GetAxis("Horizontal");

        if (move != 0)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        if (move < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (move > 0)
        {
            transform.localScale = Vector3.one;
        }
    }
}
