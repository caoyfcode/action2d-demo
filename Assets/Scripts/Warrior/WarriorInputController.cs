using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorInputController : MonoBehaviour
{
    private Warrior warrior;

    // Start is called before the first frame update
    void Start()
    {
        warrior = GetComponent<Warrior>();
    }

    // Update is called once per frame
    void Update()
    {
        var cmd = new Warrior.Command
        {
            moveX = Input.GetAxis("Horizontal"),
            startJump = Input.GetButtonDown("Jump"),
            holdJump = Input.GetButton("Jump"),
            attack = Input.GetButtonDown("Attack"),
            dash = Input.GetButtonDown("Dash"),
        };
        warrior.AddCommand(cmd);
    }
}
