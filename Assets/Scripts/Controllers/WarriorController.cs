using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    private const float MAX_RUN_SPEED = 6f;
    private const float JUMP_START_SPEED = 10f;

    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    public ContactFilter2D groundCastFilter;
    public ContactFilter2D wallCastFilter;
    private RaycastHit2D[] castHits = new RaycastHit2D[5];

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

    private bool _isOnGround;
    public bool IsOnGround
    {
        get => _isOnGround;
        private set
        {
            animator.SetBool("IsOnGround", value);
            _isOnGround = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        groundCastFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        wallCastFilter.SetLayerMask(LayerMask.GetMask("Wall"));
    }

    // Update is called once per frame
    void Update()
    {
        // 由上一帧的状态得到下一帧的状态
        bool isOnLeftWall = boxCol.Cast(Vector2.left, wallCastFilter, castHits, 0.01f) > 0;
        bool isOnRightWall = boxCol.Cast(Vector2.right, wallCastFilter, castHits, 0.01f) > 0;
        IsOnGround = boxCol.Cast(Vector2.down, groundCastFilter, castHits, 0.01f) > 0;
        

        if (Input.GetButtonDown("Attack"))
        {
            animator.SetTrigger("Attack"); // 进行攻击, 同时将更新 IsAttacking
        }

        var velocity = rb.velocity;
        if (IsOnGround && Input.GetButtonDown("Jump") && !IsAttacking)
        {
            velocity.y = JUMP_START_SPEED;
            IsOnGround = false;
        }

        float move = Input.GetAxis("Horizontal");
        if (move != 0 && !IsAttacking)
        {
            Flip = move < 0;
            if ((move < 0 && isOnLeftWall) || (move > 0 && isOnRightWall))
            {
                velocity.x = 0;
            }
            else
            {
                velocity.x = move * MAX_RUN_SPEED;
            }
        }
        else
        {
            velocity.x = 0;
        }

        if (IsAttacking)
        {
            velocity.y = 0;
        }

        animator.SetBool("IsRising", velocity.y > 0);
        animator.SetBool("IsRunning", velocity.x != 0);

        rb.velocity = velocity;

    }

    private void FixedUpdate()
    {
        if (!IsAttacking)
        {
            var velocity = rb.velocity;
            if (Math.Abs(velocity.y) < 2f)
            {
                velocity.y -= 10f * Time.deltaTime;
            }
            else
            {
                velocity.y -= 25f * Time.deltaTime;
            }
            
            rb.velocity = velocity;
        }
    }
}
