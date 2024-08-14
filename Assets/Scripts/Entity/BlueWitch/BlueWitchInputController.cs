using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueWitchInputController : MonoBehaviour
{
    private BlueWitch entity;

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

        entity.AddCommand(cmd);
    }
}
