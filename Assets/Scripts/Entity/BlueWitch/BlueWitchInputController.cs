using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWitchInputController : MonoBehaviour
{
    private BlueWitch entity;
    private float afterWallSlideTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<BlueWitch>();
    }

    // Update is called once per frame
    void Update()
    {
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        float moveX = (left, right) switch
        {
            (true, false) => -1,
            (false, true) => 1,
            _ => 0,
        };
        var cmd = new BlueWitch.Command
        {
            // moveX = Input.GetAxis("Horizontal"),
            moveX = moveX,
            startJump = Input.GetButtonDown("Jump"),
            holdJump = Input.GetButton("Jump"),
            attack = Input.GetButtonDown("Attack"),
            dash = Input.GetButtonDown("Dash"),
            slideDown = Input.GetKey(KeyCode.S),
            summon = Input.GetKeyDown(KeyCode.H),
        };
        // 在墙上冲刺或跳跃时修改方向，并在跳跃一段时间内强制方向
        // TODO 这样有时无法正常跳跃, 改为在状态机里实现
        if (entity.stateMachine.CurrentState == entity.wallSlideState)
        {
            if (cmd.dash || cmd.startJump)
            {
                cmd.moveX = -entity.FacingDirection;
                afterWallSlideTimer = 0.3f;
            }
        }
        if (afterWallSlideTimer > 0)
        {
            afterWallSlideTimer -= Time.deltaTime;
            if (entity.velocity.y > 0 && entity.stateMachine.CurrentState == entity.holdJumpState)
            {
                if (afterWallSlideTimer > 0.1f || cmd.moveX == 0)
                    cmd.moveX = entity.FacingDirection;
            }
        }

        entity.AddCommand(cmd);
    }
}
