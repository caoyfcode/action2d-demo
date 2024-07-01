using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorInputController : MonoBehaviour
{
    private Warrior warrior;
    private float afterWallSlideTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        warrior = GetComponent<Warrior>();
    }

    // Update is called once per frame
    void Update()
    {
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        float moveX = (left, right) switch {
            (true, false) => -1,
            (false, true) => 1,
            _ => 0,
        };
        var cmd = new Warrior.Command
        {
            // moveX = Input.GetAxis("Horizontal"),
            moveX = moveX,
            startJump = Input.GetButtonDown("Jump"),
            holdJump = Input.GetButton("Jump"),
            attack = Input.GetButtonDown("Attack"),
            dash = Input.GetButtonDown("Dash"),
            slideDown = Input.GetKey(KeyCode.S),
        };
        // 在墙上冲刺或跳跃时修改方向，并在跳跃一段时间内强制方向
        if (warrior.stateMachine.CurrentState == warrior.wallSlideState)
        {
            if (cmd.dash || cmd.startJump)
            {
                cmd.moveX = -warrior.FacingDirection;
                afterWallSlideTimer = 0.3f;
            }
        }
        if (afterWallSlideTimer > 0)
        {
            afterWallSlideTimer -= Time.deltaTime;
            if (warrior.velocity.y > 0 && warrior.stateMachine.CurrentState == warrior.holdJumpState)
            {
                if (afterWallSlideTimer > 0.1f || cmd.moveX == 0)
                    cmd.moveX = warrior.FacingDirection;
            }
        }
        
        warrior.AddCommand(cmd);
    }
}
