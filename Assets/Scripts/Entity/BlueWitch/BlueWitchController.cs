using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWitchController : EntityController
{
    private BlueWitch entity;

    public override void Init()
    {
        entity = GetComponent<BlueWitch>();
    }

    public override void UpdateCommand()
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
            attackHeroKnight = Input.GetButtonDown("Attack"),
            dash = Input.GetButtonDown("Dash"),
            attackArcaneArcher = Input.GetKeyDown(KeyCode.U),
            defendFireKnight = Input.GetKeyDown(KeyCode.H),
        };

        entity.SetCommand(cmd);
    }

    public override void OnAttacked(HitInfo hitInfo)
    {

    }

    public override void OnDeath()
    {

    }
}
