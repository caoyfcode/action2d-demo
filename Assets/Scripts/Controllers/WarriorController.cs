using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    private const float MAX_RUN_SPEED = 6f;

    private Animator animator;
    private Rigidbody2D rb;

    public bool IsAttacking => animator.GetBool("IsAttacking");

    public bool Flip
    {
        get => rb.transform.localScale.x < 0;
        private set
        {
            float scale = value ? -1 : 1;
            rb.transform.localScale = new Vector3(scale, 1, 1);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            animator.SetTrigger("Attack");
        }

        float move = Input.GetAxis("Horizontal");
        var velocity = rb.velocity;
        if (move != 0 && !IsAttacking)
        {
            animator.SetBool("IsRunning", true);
            Flip = move < 0;
            velocity.x = move * MAX_RUN_SPEED;
        }
        else
        {
            animator.SetBool("IsRunning", false);
            velocity.x = 0;
        }
        rb.velocity = velocity;

    }
}
